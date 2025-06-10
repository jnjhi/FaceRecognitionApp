using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using OpenCvSharp;
using Microsoft.SqlServer.Server;

namespace FaceRecognitionClient.ClientLogger
{
    public static class ClientLogger
    {
        private const string k_LogDirectory = @"C:\Users\denis\source\repos\FaceRecognition\FaceRecognitionClient\Logger\";
        private const string k_LogFile = "client-log.txt";

        private static readonly string s_LogPath = Path.Combine(k_LogDirectory, k_LogFile);

        static ClientLogger()
        {
            try
            {
                Directory.CreateDirectory(k_LogDirectory);
                if (!File.Exists(s_LogPath))
                {
                    File.WriteAllText(s_LogPath, $"[START] Log initialized at {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n");
                }
            }
            catch
            {
                // If logging fails, fail silently – don't crash the app
            }
        }

        public static void LogInfo(string message)
        {
            Log($"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        public static void LogException(Exception ex, string contextMessage = null)
        {
            Log($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss}\nContext: {contextMessage}\nException: {ex.GetType().Name} - {ex.Message}\nStack Trace: {ex.StackTrace}");
        }

        public static void LogWarning(string message)
        {
            Log($"[WARNING] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        private static void Log(string fullMessage)
        {
            try
            {
                File.AppendAllText(s_LogPath, fullMessage + Environment.NewLine + Environment.NewLine);
            }
            catch
            {
                // Avoid secondary failures during logging
            }
        }
    }
}
