namespace FaceRecognitionServer
{
    /// <summary>
    /// Simple logger class for development-time diagnostics.
    /// Writes exception and message details to the console.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Logs a message to the console (info level).
        /// </summary>
        public static void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            Console.ResetColor();
        }

        /// <summary>
        /// Logs an exception with optional context message.
        /// </summary>
        public static void LogException(Exception ex, string contextMessage = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            if (!string.IsNullOrWhiteSpace(contextMessage))
            {
                Console.WriteLine($"Context: {contextMessage}");
            }
            Console.WriteLine($"Exception: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            Console.ResetColor();
        }

        public static void LogCustomError(string contextMessage)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[WARNING] {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Context: {contextMessage}");
            Console.ResetColor();
        }

    }
}
