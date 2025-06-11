using FaceRecognitionClient.ClientLogger;
using System.IO;
using FaceRecognitionClient.MVVMStructures.ViewModels.PersonProfile;
using System.Text;

namespace FaceRecognitionClient.Services.AttendanceExportService
{
    public class AttendanceExportService
    {
        public Task ExportAsync(IEnumerable<AttendanceRecord> records, string filePath)
        {
            return Task.Run(() =>
            {
                try
                {
                    var directory = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrEmpty(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    var sb = new StringBuilder();
                    foreach (var record in records)
                    {
                        sb.AppendLine($"{record.FirstName}\t{record.LastName}\t{record.GovernmentId}\t{record.AttendanceTime:dd/MM/yyyy HH:mm}");
                    }

                    File.WriteAllText(filePath, sb.ToString());
                }
                catch (Exception ex)
                {
                    ClientLogger.ClientLogger.LogException(ex, "Failed to export attendance records");
                }
            });
        }
    }
}
