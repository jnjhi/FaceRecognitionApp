using System.Drawing.Imaging;
using System.Drawing;

namespace FaceRecognitionServer.Utils
{
    public static class ImageConversionUtils
    {
        public static string EncodeBitmapToBase64(Bitmap bitmap)
        {
            if (bitmap == null)
                return null;

            try
            {
                using var ms = new MemoryStream();

                // Convert to safe pixel format
                using var safeBitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format24bppRgb);
                using (Graphics g = Graphics.FromImage(safeBitmap))
                {
                    g.Clear(Color.White); // Prevent transparent or uninitialized areas from becoming black
                    g.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
                }

                // Save as JPEG
                safeBitmap.Save(ms, ImageFormat.Jpeg);
                byte[] imageBytes = ms.ToArray();

                return Convert.ToBase64String(imageBytes);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Failed to encode bitmap to Base64.");
                return null;
            }
        }

        public static Bitmap DecodeBase64ToBitmap(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64))
                return null;

            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64);
                using var ms = new MemoryStream(imageBytes);
                using var original = new Bitmap(ms); // May contain transparency

                var detached = new Bitmap(original.Width, original.Height, PixelFormat.Format24bppRgb); // No alpha channel
                using (Graphics graphics = Graphics.FromImage(detached))
                {
                    graphics.Clear(Color.White); // Ensure no black background
                    graphics.DrawImage(original, 0, 0, original.Width, original.Height);
                }

                return detached;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Failed to decode Base64 string to Bitmap.");
                return null;
            }
        }

    }
}
