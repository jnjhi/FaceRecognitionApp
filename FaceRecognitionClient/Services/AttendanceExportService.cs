using FaceRecognitionClient.MVVMStructures.ViewModels.PersonProfile;
using System.IO;

namespace FaceRecognitionClient.Services
{
    public class AttendanceExportService
    {
        public void Export(IEnumerable<AttendanceRecord> records, string filePath)
        {
            if (records == null || string.IsNullOrWhiteSpace(filePath))
                return;

            try
            {
                // Define column widths (add spacing buffer between columns)
                int firstNameWidth = 16;     // 15 + 1 space
                int lastNameWidth = 16;
                int govIdWidth = 14;         // 12 + 2 spaces
                int timeWidth = 20;

                string header = string.Format("{0,-16}{1,-16}{2,-14}{3,-20}",
                    "FirstName", "LastName", "GovernmentID", "AttendanceTime");

                var lines = new List<string> { header };

                foreach (var r in records)
                {
                    string row = string.Format("{0,-16}{1,-16}{2,-14}{3,-20}",
                        r.FirstName,
                        r.LastName,
                        r.GovernmentId,
                        r.AttendanceTime.ToString("dd/MM/yyyy HH:mm"));

                    lines.Add(row);
                }

                File.WriteAllLines(filePath, lines);
            }
            catch (Exception ex)
            {
                ClientLogger.ClientLogger.LogException(ex, "Failed to export attendance records.");
            }
        }

    }
}
