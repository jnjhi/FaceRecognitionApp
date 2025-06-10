using System.Windows.Media.Imaging;

namespace FaceRecognitionClient
{
    /// <summary>
    /// A shared container for temporarily storing the image captured by the camera.
    /// This allows different ViewModels (e.g., CameraCaptureViewModel and FaceRecognitionViewModel)
    /// to share access to the same image without directly referencing each other.
    /// </summary>
    public class SharedImageStore
    {
        /// <summary>
        /// The BitmapImage that was most recently captured from the camera.
        /// This is updated in CameraCaptureViewModel and later consumed in FaceRecognitionViewModel.
        /// </summary>
        public BitmapImage CapturedImage { get; set; }
    }
}
