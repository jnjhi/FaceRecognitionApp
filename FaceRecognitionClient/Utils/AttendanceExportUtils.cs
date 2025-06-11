using FaceRecognitionClient.ClientLogger;
using System;
using System.Collections.Generic;
using System.IO;
using Forms = System.Windows.Forms;

namespace FaceRecognitionClient.Utils
{
    public static class AttendanceExportUtils
    {
        public static string? PromptForDirectory()
        {
            using var folderDialog = new Forms.FolderBrowserDialog();
            return folderDialog.ShowDialog() == Forms.DialogResult.OK ? folderDialog.SelectedPath : null;
        }

        public static void Export(IEnumerable<AttendanceRecord> records, string directory)
        {
            try
            {
                if (records == null || string.IsNullOrWhiteSpace(directory))
                    return;
                Directory.CreateDirectory(directory);
                string path = Path.Combine(directory, $"attendance_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

                using var writer = new StreamWriter(path, false);
                foreach (var record in records)
                {
                    writer.WriteLine($"{record.FirstName} {record.LastName} ({record.GovernmentId}) - {record.AttendanceTime:dd/MM/yyyy HH:mm}");
                }
            }
            catch (Exception ex)
            {
                ClientLogger.LogException(ex, "Failed to export attendance records.");
            }
        }
    }
}
