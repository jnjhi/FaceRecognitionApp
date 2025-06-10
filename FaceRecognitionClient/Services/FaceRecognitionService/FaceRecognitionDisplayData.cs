using DataProtocols;
using DataProtocols.FaceRecognitionMessages;
using FaceRecognitionClient.InternalDataModels;
using System.Windows.Media.Imaging;

namespace FaceRecognitionClient.Services.FaceRecognitionService
{
    public class FaceRecognitionDisplayData
    {
        public BitmapImage AnnotatedImage { get; set; }
        public List<AdvancedPersonDataWithImage> RecognitionData { get; set; }
    }
}
