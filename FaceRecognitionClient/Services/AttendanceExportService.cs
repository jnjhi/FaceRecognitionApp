using FaceRecognitionClient.MVVMStructures.ViewModels.PersonProfile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                var lines = new List<string>
                {
                    "FirstName,LastName,GovernmentID,AttendanceTime"
                };

                foreach (var r in records)
                {
                    lines.Add($"{r.FirstName},{r.LastName},{r.GovernmentId},{r.AttendanceTime:dd/MM/yyyy HH:mm}");
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
