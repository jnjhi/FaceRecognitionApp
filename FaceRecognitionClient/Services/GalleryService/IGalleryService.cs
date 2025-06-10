using FaceRecognitionClient.InternalDataModels;
using System.Drawing;

namespace FaceRecognitionClient.Services.GalleryService
{
    public interface IGalleryService
    {
        Task<List<GalleryImage>> GetGalleryAsync();
    }
}
