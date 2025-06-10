using FaceRecognitionServer.Services.DataBases.Models;
using System.Data;
using System.Data.SqlClient;

namespace FaceRecognitionServer.Services.DataBases.ConnectionToTables
{
    public class AttendanceStorageSystem : IDisposable
    {
        private const string k_ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\denis\source\repos\FaceRecognition\FaceRecognitionServer\Services\DataBases\FaceRecognitionDB.mdf;Integrated Security=True";

        public AttendanceStorageSystem()
        {
            // No additional setup required for attendances
        }

        public void InsertAttendance(int recognizedPersonId, DateTime attendanceTime)
        {
            try
            {
                using var connection = new SqlConnection(k_ConnectionString);
                using var command = new SqlCommand(@"
                    INSERT INTO Attendances (RecognizedPersonId, AttendanceTime)
                    VALUES (@personId, @time)", connection);

                command.Parameters.AddWithValue("@personId", recognizedPersonId);
                command.Parameters.AddWithValue("@time", attendanceTime);

                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, $"Failed to insert attendance for person ID {recognizedPersonId} at {attendanceTime:O}.");
            }
        }

        public List<AttendanceRecord> GetAllAttendancesByUserId(int recognizedPersonId)
        {
            try
            {
                using var connection = new SqlConnection(k_ConnectionString);
                connection.Open();

                const string query = @"
                    SELECT Id, RecognizedPersonId, AttendanceTime
                    FROM Attendances
                    WHERE RecognizedPersonId = @personId
                    ORDER BY AttendanceTime ASC";

                var parameters = new[]
                {
                    new SqlParameter("@personId", SqlDbType.Int) { Value = recognizedPersonId }
                };

                return LoadAttendanceRecords(connection, query, parameters);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, $"Failed to fetch all attendances for person ID {recognizedPersonId}.");
                return new List<AttendanceRecord>();
            }
        }

        public List<AttendanceRecord> GetDayAttendance(DateTime day)
        {
            try
            {
                using var connection = new SqlConnection(k_ConnectionString);
                connection.Open();

                // Compare only the date component
                const string query = @"
                    SELECT Id, RecognizedPersonId, AttendanceTime
                    FROM Attendances
                    WHERE CAST(AttendanceTime AS DATE) = @date
                    ORDER BY RecognizedPersonId, AttendanceTime";

                var parameters = new[]
                {
                    new SqlParameter("@date", SqlDbType.Date) { Value = day.Date }
                };

                return LoadAttendanceRecords(connection, query, parameters);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, $"Failed to fetch attendances for date {day:yyyy-MM-dd}.");
                return new List<AttendanceRecord>();
            }
        }

        public List<AttendanceRecord> GetAllAttendances()
        {
            try
            {
                using var connection = new SqlConnection(k_ConnectionString);
                connection.Open();

                const string query = @"
            SELECT Id, RecognizedPersonId, AttendanceTime
            FROM Attendances
            ORDER BY AttendanceTime ASC";

                return LoadAttendanceRecords(connection, query, Array.Empty<SqlParameter>());
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Failed to fetch all attendance records.");
                return new List<AttendanceRecord>();
            }
        }


        private List<AttendanceRecord> LoadAttendanceRecords(SqlConnection connection, string sql, SqlParameter[] parameters)
        {
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddRange(parameters);

            using var reader = command.ExecuteReader();
            var results = new List<AttendanceRecord>();

            while (reader.Read())
            {
                var record = new AttendanceRecord
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    RecognizedPersonId = reader.GetInt32(reader.GetOrdinal("RecognizedPersonId")),
                    AttendanceTime = reader.GetDateTime(reader.GetOrdinal("AttendanceTime"))
                };
                results.Add(record);
            }

            return results;
        }

        public void Dispose()
        {
            // Nothing to dispose explicitly
        }
    }
}
