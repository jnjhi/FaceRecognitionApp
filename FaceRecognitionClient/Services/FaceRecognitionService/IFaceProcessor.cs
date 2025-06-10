using DlibDotNet;
using System.Windows.Media.Imaging;

namespace FaceRecognitionClient.Services.FaceRecognitionService
{
    public interface IFaceProcessor : IDisposable
    {
        List<DetectedFace> GetFaceEmbedding(BitmapImage image);

        Matrix<float> GetEmbeddingForStorage(BitmapImage image);
    }
}
