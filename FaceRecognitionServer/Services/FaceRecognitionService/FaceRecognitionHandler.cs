using DataProtocols;
using DataProtocols.FaceRecognitionMessages;
using DataProtocols.FaceRecognitionMessages.Models;
using FaceRecognitionServer.Services.DataBases.ConnectionToTables;
using FaceRecognitionServer.Services.DataBases.Models;
using FaceRecognitionServer.Utils;
using System.Drawing;

namespace FaceRecognitionServer.Services.FaceRecognitionService
{
    public class FaceRecognitionHandler : ITypedMessageHandler<PreRecognitionFaceDataDTO>
    {
        private readonly ISecureNetworkManager _secureNetworkManager;
        private readonly GalleryStorageSystem _GalleryStorageSystem;
        private readonly AttendanceStorageSystem _AttendanceStorageSystem;
        
        private const double DistanceThreshold = 0.6;

        public FaceRecognitionHandler(ISecureNetworkManager secureNetworkManager, GalleryStorageSystem galleryStorageSystem, AttendanceStorageSystem attendanceStorageSystem)
        {
            _secureNetworkManager = secureNetworkManager;
            _GalleryStorageSystem = galleryStorageSystem;
            _AttendanceStorageSystem = attendanceStorageSystem;
        }

        public async Task HandleMessageAsync(PreRecognitionFaceDataDTO message, string ip)
        {
            var results = new List<FullPersonDataWithProfilePictureDTO>();

            foreach (var face in message.Faces)
            {
                var result = ProcessFaceRecognition(face);
                results.Add(result);
            }

            var response = new FaceRecognitionResultDTO(results);
            var payload = ConvertUtils.Serialize(response);
            _secureNetworkManager.SendMessage(payload, ip);
        }

        private FullPersonDataWithProfilePictureDTO ProcessFaceRecognition(PreRecognitionDataDTO face)
        {
            var matchResult = FindBestMatchingFace(face.Embedding);

            if (matchResult.FaceRecord != null && matchResult.Distance <= DistanceThreshold)
            {
                InsertNewAttendanceRecord(matchResult.FaceRecord.Id, face.CaptureTime);
                return MapMatchToResultDTO(matchResult.FaceRecord, face, GetProfileImage(matchResult.FaceRecord.Id));
            }

            var unknownUser = InsertUnknownUserToDatabase(face.Embedding);
            InsertProfilePictureForUnknown(unknownUser.Id, face.ProfilePictureInString64, face.CaptureTime);
            InsertNewAttendanceRecord(unknownUser.Id, face.CaptureTime);

            return MapUnknownToResultDTO(unknownUser, face, GetProfileImage(unknownUser.Id));
        }

        private Bitmap GetProfileImage(int id)
        {
            using var GalleryStorageSystem = new GalleryStorageSystem();
            return GalleryStorageSystem.GetProfileImageById(id);
        }

        private FullPersonDataWithProfilePictureDTO MapMatchToResultDTO(AdvancedFaceData record, PreRecognitionDataDTO face, Bitmap profileImage)
        {
            return new FullPersonDataWithProfilePictureDTO
            {
                Id = record.Id,
                ProfilePicture = ImageConversionUtils.EncodeBitmapToBase64(profileImage),
                Name = $"{record.FirstName} {record.LastName}",
                FirstName = record.FirstName,
                LastName = record.LastName,
                GovernmentID = record.GovernmentID,
                HeightCm = record.HeightCm,
                Sex = record.Sex,
                Notes = record.Notes,
                Rectangle = face.Rectangle,
                FaceEmbedding = face.Embedding
            };
        }

        private FullPersonDataWithProfilePictureDTO  MapUnknownToResultDTO(AdvancedFaceData unknownUser, PreRecognitionDataDTO face, Bitmap profileImage)
        {
            return new FullPersonDataWithProfilePictureDTO
            {
                Id = unknownUser.Id,
                ProfilePicture = ImageConversionUtils.EncodeBitmapToBase64(profileImage),
                Name = "UNKNOWN",
                FirstName = "UNKNOWN",
                LastName = null,
                GovernmentID = unknownUser.GovernmentID,
                HeightCm = null,
                Sex = null,
                Notes = null,
                Rectangle = face.Rectangle,
                FaceEmbedding = unknownUser.FaceEmbedding
            };
        }

        private AdvancedFaceData InsertUnknownUserToDatabase(float[] embedding)
        {
            using var db = new ConnectionToFaceTable();
            return db.InsertUnknownUser(embedding);
        }

        private void InsertProfilePictureForUnknown(int userId, string base64ProfilePicture, DateTime captureTime)
        {
            var bitmap = ImageConversionUtils.DecodeBase64ToBitmap(base64ProfilePicture);
            if (bitmap == null)
            {
                Logger.LogCustomError("Failed to decode profile picture for unknown user.");
                return;
            }

            using var db = new GalleryStorageSystem();
            var profilePicture = new ProfilePicture(userId, bitmap, captureTime);
            db.InsertProfilePicture(profilePicture);
        }

        private void InsertNewAttendanceRecord(int recognizedPersonId, DateTime date) => _AttendanceStorageSystem.InsertAttendance(recognizedPersonId, date);

        // Scans all stored records to find the one with minimal distance
        private FaceMatchResult FindBestMatchingFace(float[] probeEmbedding)
        {
            List<AdvancedFaceData> faceRecords;
            bool isMatch = false;

            using (var db = new ConnectionToFaceTable())
            {
                faceRecords = db.GetAllFaceRecords();
            } 

            AdvancedFaceData bestRecord = null;
            double bestDistance = double.MaxValue;

            foreach (var storedRecord in faceRecords)
            {
                var storedEmbedding = storedRecord.FaceEmbedding.ToArray();
                double distance = CalculateEuclideanDistance(storedEmbedding, probeEmbedding);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestRecord = storedRecord;
                }
            }

            if (bestDistance <= DistanceThreshold)
            {
                isMatch = true;
            }

            return new FaceMatchResult
            {
                IsAMatch = isMatch,
                FaceRecord = bestRecord,
                Distance = bestDistance
            };
        }

        // Computes Euclidean distance between two embeddings of equal length
        private static double CalculateEuclideanDistance(float[] firstEmbedding, float[] secondEmbedding)
        {
            if (firstEmbedding.Length != secondEmbedding.Length)
                throw new ArgumentException("Embedding lengths must match", nameof(secondEmbedding));

            double sumOfSquares = 0;
            for (int i = 0; i < firstEmbedding.Length; i++)
            {
                double diff = firstEmbedding[i] - secondEmbedding[i];
                sumOfSquares += diff * diff;
            }

            return Math.Sqrt(sumOfSquares);
        }
    }
}

