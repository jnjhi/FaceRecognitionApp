using System.Windows.Media.Imaging;

namespace FaceRecognitionClient.InternalDataModels
{
    public class GalleryImage
    {
        public DateTime CaptureTime { get; set; }
        public AdvancedPersonDataWithImage Person { get; set; }

        public GalleryImage()
        {
            
        }

        public GalleryImage(DateTime captureTime, AdvancedPersonDataWithImage persons)
        {
            CaptureTime = captureTime;
            Person = persons;
        }
    }
}
