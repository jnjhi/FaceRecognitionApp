using DlibDotNet;

namespace FaceRecognitionClient.Services.FaceRecognitionService
{
    public class DetectedFace
    {
        public Matrix<float> Embedding { get; set; }
        public System.Drawing.Rectangle BoundingBox { get; set; }
    }
}
