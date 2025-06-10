using FaceRecognitionServer.Services.DataBases.Models;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;

namespace FaceRecognitionServer.Services.DataBases.ConnectionToTables
{
    public class GalleryStorageSystem : IDisposable
    {
        private const string k_ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\denis\source\repos\FaceRecognition\FaceRecognitionServer\Services\DataBases\FaceRecognitionDB.mdf;Integrated Security=True";
        private const string k_FaceDataBasePath = @"C:\Users\denis\source\repos\FaceRecognition\FaceRecognitionServer\Services\DataBases\DataBaseFiles\";

        private const string k_ProfilePicturesFolderName = "ProfilePictures";
        private const string k_ProfilePictureFileNameFormat = "{0}_profile.jpg";

        private readonly string m_ProfilePicturesDirectory;

        public GalleryStorageSystem()
        {
            m_ProfilePicturesDirectory = Path.Combine(k_FaceDataBasePath, k_ProfilePicturesFolderName);
            Directory.CreateDirectory(m_ProfilePicturesDirectory);
        }

        public List<AdvancedFaceDataWithProfilePicture> GetGallery()
        {
            try
            {
                using var connection = new SqlConnection(k_ConnectionString);
                connection.Open();
                return LoadAllFaceRecords(connection);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Failed to fetch gallery from database.");
                return new List<AdvancedFaceDataWithProfilePicture>();
            }
        }

        public void InsertProfilePicture(ProfilePicture recognizedPerson)
        {
            try
            {
                string filePath = SaveProfileBitmap(recognizedPerson.Image, recognizedPerson.IdentifiedPersonId);
                if (filePath == null)
                    return;

                using var connection = new SqlConnection(k_ConnectionString);
                using var command = new SqlCommand(@"
                    INSERT INTO ProfilePictures (RecognizedPersonId, ProfilePictureFilePath, CaptureTime) 
                    VALUES (@recognizedPersonId, @profilePictureFilePath, @captureTime)", connection);

                command.Parameters.AddWithValue("@recognizedPersonId", recognizedPerson.IdentifiedPersonId);
                command.Parameters.AddWithValue("@profilePictureFilePath", filePath);
                command.Parameters.AddWithValue("@captureTime", recognizedPerson.CaptureDate);

                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Failed to insert profile picture.");
            }
        }

        public void ClearGallery()
        {
            try
            {
                // Step 1: Delete all records from the ProfilePictures table
                using (var connection = new SqlConnection(k_ConnectionString))
                using (var command = new SqlCommand("DELETE FROM ProfilePictures", connection))
                {
                    connection.Open();
                    int affectedRows = command.ExecuteNonQuery();
                    Logger.LogInfo($"Deleted {affectedRows} records from ProfilePictures table.");
                }

                // Step 2: Delete all files in the profile pictures directory
                var files = Directory.GetFiles(m_ProfilePicturesDirectory);
                int deletedFiles = 0;

                foreach (var file in files)
                {
                    try
                    {
                        File.Delete(file);
                        deletedFiles++;
                    }
                    catch (Exception fileEx)
                    {
                        Logger.LogException(fileEx, $"Failed to delete profile picture file: {file}");
                    }
                }

                Logger.LogInfo($"Deleted {deletedFiles} profile picture files from: {m_ProfilePicturesDirectory}");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Failed to clear gallery.");
            }
        }

        public Bitmap GetProfileImageById(int personId)
        {
            try
            {
                using var connection = new SqlConnection(k_ConnectionString);
                using var command = new SqlCommand(@"
                    SELECT ProfilePictureFilePath
                    FROM ProfilePictures
                    WHERE RecognizedPersonId = @id", connection);

                command.Parameters.AddWithValue("@id", personId);
                connection.Open();

                using var reader = command.ExecuteReader();
                if (reader.Read() && !reader.IsDBNull(0))
                {
                    string relativePath = reader.GetString(0);
                    return LoadProfilePicture(relativePath);
                }
                else
                {
                    Logger.LogCustomError($"No profile picture found for person ID: {personId}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, $"Failed to load profile image for person ID: {personId}");
                return null;
            }
        }

        private List<AdvancedFaceDataWithProfilePicture> LoadAllFaceRecords(SqlConnection connection)
        {
            var results = new List<AdvancedFaceDataWithProfilePicture>();

            using var command = new SqlCommand(@"
                SELECT f.Id, f.GovernmentID, f.FirstName, f.LastName, f.HeightCm, f.Sex, f.EmbeddingFilePath, f.NotesFilePath,
                p.ProfilePictureFilePath, p.CaptureTime FROM Faces AS f INNER JOIN ProfilePictures AS p ON f.Id = p.RecognizedPersonId;", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var faceRecord = TryParseRecord(reader);
                if (faceRecord != null)
                    results.Add(faceRecord);
            }

            return results;
        }

        private AdvancedFaceDataWithProfilePicture TryParseRecord(SqlDataReader reader)
        {
            try
            {
                int id = reader.GetInt32(reader.GetOrdinal("Id"));
                string governmentId = reader.IsDBNull(reader.GetOrdinal("GovernmentID")) ? null : reader.GetString(reader.GetOrdinal("GovernmentID"));
                string firstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString(reader.GetOrdinal("FirstName"));
                string lastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName"));
                int? heightCm = reader.IsDBNull(reader.GetOrdinal("HeightCm")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("HeightCm"));
                string sex = reader.IsDBNull(reader.GetOrdinal("Sex")) ? null : reader.GetString(reader.GetOrdinal("Sex"));

                string embeddingRelPath = reader.IsDBNull(reader.GetOrdinal("EmbeddingFilePath")) ? null : reader.GetString(reader.GetOrdinal("EmbeddingFilePath"));
                string notesRelPath = reader.IsDBNull(reader.GetOrdinal("NotesFilePath")) ? null : reader.GetString(reader.GetOrdinal("NotesFilePath"));
                string profileRelPath = reader.IsDBNull(reader.GetOrdinal("ProfilePictureFilePath")) ? null : reader.GetString(reader.GetOrdinal("ProfilePictureFilePath"));
                DateTime captureTime = reader.IsDBNull(reader.GetOrdinal("CaptureTime")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("CaptureTime"));

                Logger.LogInfo($"Parsing Face Record: Id={id}, GovernmentID={governmentId ?? "NULL"}, FirstName={firstName ?? "NULL"}, LastName={lastName ?? "NULL"}, " +
                               $"HeightCm={(heightCm.HasValue ? heightCm.ToString() : "NULL")}, Sex={sex ?? "NULL"}, " +
                               $"EmbeddingFileExists={File.Exists(Path.Combine(k_FaceDataBasePath, embeddingRelPath ?? ""))}, " +
                               $"NotesFileExists={File.Exists(Path.Combine(k_FaceDataBasePath, notesRelPath ?? ""))}, " +
                               $"ProfilePictureExists={File.Exists(Path.Combine(m_ProfilePicturesDirectory, profileRelPath ?? ""))}");

                return new AdvancedFaceDataWithProfilePicture
                {
                    Id = id,
                    GovernmentID = governmentId,
                    FirstName = firstName,
                    LastName = lastName,
                    HeightCm = heightCm,
                    Sex = sex,
                    Notes = LoadNotes(notesRelPath),
                    FaceEmbedding = LoadEmbedding(embeddingRelPath),
                    profilePicture = LoadProfilePicture(profileRelPath),
                    CaptureDate = captureTime
                };
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Failed to parse a face record from reader.");
                return null;
            }
        }

        private float[] LoadEmbedding(string relativePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(relativePath))
                {
                    Logger.LogCustomError("Embedding path is null or empty.");
                    return null;
                }

                string fullPath = Path.Combine(k_FaceDataBasePath, relativePath);
                if (!File.Exists(fullPath))
                {
                    Logger.LogCustomError($"Embedding file does not exist: {fullPath}");
                    return null;
                }

                string base64 = File.ReadAllText(fullPath).Trim();
                byte[] bytes = Convert.FromBase64String(base64);

                if (bytes.Length % sizeof(float) != 0)
                {
                    Logger.LogCustomError($"Decoded byte array from {fullPath} is not a multiple of float size.");
                    return null;
                }

                float[] result = new float[bytes.Length / sizeof(float)];
                Buffer.BlockCopy(bytes, 0, result, 0, bytes.Length);

                if (result.Length != 128)
                {
                    Logger.LogCustomError($"Embedding file {fullPath} decoded to {result.Length} floats instead of 128.");
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Failed to load and decode face embedding.");
                return null;
            }
        }

        private string LoadNotes(string relativePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(relativePath))
                    return null;

                string fullPath = Path.Combine(k_FaceDataBasePath, relativePath);
                if (!File.Exists(fullPath))
                {
                    Logger.LogCustomError($"Notes file missing: {fullPath}");
                    return null;
                }

                return File.ReadAllText(fullPath);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Failed to load notes.");
                return null;
            }
        }

        private Bitmap LoadProfilePicture(string relativePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(relativePath))
                    return null;
                //C:\Users\denis\source\repos\FaceRecognition\FaceRecognitionServer\Services\DataBases\DataBaseFiles\ProfilePictures\20_profile.jpg
                string fullPath = Path.Combine(m_ProfilePicturesDirectory, relativePath);
                if (!File.Exists(fullPath))
                {
                    Logger.LogCustomError($"Profile picture file does not exist: {fullPath}");
                    return null;
                }

                return new Bitmap(fullPath);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Failed to load profile picture bitmap.");
                return null;
            }
        }

        private string SaveProfileBitmap(Bitmap bitmap, int id)
        {
            if (bitmap == null)
            {
                Logger.LogCustomError($"Cannot save bitmap — input bitmap is null for ID {id}.");
                return null;
            }

            try
            {
                string fileName = string.Format(k_ProfilePictureFileNameFormat, id);
                string filePath = Path.Combine(m_ProfilePicturesDirectory, fileName);

                using var safeBitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format24bppRgb);
                using (Graphics g = Graphics.FromImage(safeBitmap))
                {
                    g.Clear(Color.White);
                    g.DrawImage(bitmap,
                        new Rectangle(0, 0, safeBitmap.Width, safeBitmap.Height),
                        new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        GraphicsUnit.Pixel); // ✅ fixes cropped output
                }

                safeBitmap.Save(filePath, ImageFormat.Jpeg);

                if (!File.Exists(filePath))
                {
                    Logger.LogCustomError($"Bitmap save reported success, but file not found afterward: {filePath}");
                }

                return fileName;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Failed to save profile bitmap.");
                return null;
            }
        }


        public void Dispose() { }
    }
}
