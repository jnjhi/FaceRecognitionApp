using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;


namespace FaceRecognitionClient.Utils
{
    public static class ImageProcessingUtils
    {
        public static BitmapImage ConvertBitmapToBitmapImage(System.Drawing.Bitmap bitmap)
        {
            using var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            memoryStream.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

        public static BitmapImage CropFaceToBitmapImage(BitmapImage sourceImage, Rectangle rectangle)
        {
            const double marginFactor = 0.3;  // increase to pad more around the face

            // Re-encode the BitmapImage into a System.Drawing.Bitmap
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(sourceImage));
            using var fullImageStream = new MemoryStream();
            encoder.Save(fullImageStream);
            fullImageStream.Position = 0;
            using var sourceBitmap = new Bitmap(fullImageStream);

            // Compute padding
            int padX = (int)(rectangle.Width * marginFactor);
            int padY = (int)(rectangle.Height * marginFactor);

            // Build expanded rectangle, then clamp to image bounds
            int x = Math.Max(0, rectangle.X - padX);
            int y = Math.Max(0, rectangle.Y - padY);
            int w = Math.Min(sourceBitmap.Width - x, rectangle.Width + padX * 2);
            int h = Math.Min(sourceBitmap.Height - y, rectangle.Height + padY * 2);
            var expanded = new Rectangle(x, y, w, h);

            // Clone the padded region
            using var croppedBitmap = sourceBitmap.Clone(expanded, sourceBitmap.PixelFormat);

            // Convert back to BitmapImage
            using var croppedStream = new MemoryStream();
            croppedBitmap.Save(croppedStream, System.Drawing.Imaging.ImageFormat.Png);
            croppedStream.Position = 0;
            var result = new BitmapImage();
            result.BeginInit();
            result.CacheOption = BitmapCacheOption.OnLoad;
            result.StreamSource = croppedStream;
            result.EndInit();
            result.Freeze();
            return result;
        }


        public static BitmapImage ConvertWriteableBitmapToBitmapImage(WriteableBitmap writeableBitmap)
        {
            using var memoryStream = new MemoryStream();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(writeableBitmap));
            encoder.Save(memoryStream);
            memoryStream.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

        public static Bitmap ConvertBitmapImageToBitmap(BitmapImage bitmapImage)
        {
            using var memoryStream = new MemoryStream();
            var encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            encoder.Save(memoryStream);
            memoryStream.Position = 0;

            using var tempBitmap = new Bitmap(memoryStream);
            return new Bitmap(tempBitmap); // create a deep copy to release the stream
        }

        public static string EncodeBitmapToBase64(Bitmap bitmap)
        {
            if (bitmap == null)
                return null;

            try
            {
                using var ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Jpeg); // Use JPEG for compact encoding
                byte[] imageBytes = ms.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
            catch (Exception ex)
            {
                ClientLogger.ClientLogger.LogException(ex, "Failed to encode bitmap to Base64.");
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
                using var original = new Bitmap(ms); // still tied to stream

                // Fully detach by cloning into a new Bitmap
                var detached = new Bitmap(original.Width, original.Height);
                using (Graphics graphics = Graphics.FromImage(detached))
                {
                    graphics.DrawImage(original, 0, 0);
                }

                return detached;
            }
            catch (Exception ex)
            {
                ClientLogger.ClientLogger.LogException(ex, "Failed to decode Base64 string to Bitmap.");
                return null;
            }
        }
    }
}
