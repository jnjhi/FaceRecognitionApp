using DataProtocols.GalleryMessages;
using FaceRecognitionClient.InternalDataModels;
using FaceRecognitionClient.MVVMStructures.Views;
using FaceRecognitionClient.Services.GalleryService;
using FaceRecognitionClient.Utils;

namespace FaceRecognitionClient.MVVMStructures.Models.Gallery
{
    /// <summary>
    /// Manages gallery image records for the current user.
    /// Interacts with the GalleryService to load, update, and delete images.
    /// </summary>
    public class GalleryModel
    {
        private readonly IGalleryService _galleryService;
        private readonly UserSession _userSession;

        private List<GalleryImage> Images { get; } = new List<GalleryImage>();

        public GalleryModel(IGalleryService galleryService, UserSession userSession)
        {
            _galleryService = galleryService;
            _userSession = userSession;
        }

        /// <summary>
        /// Retrieves all gallery images for the current user from the server.
        /// </summary>
        public async Task<List<GalleryImage>> LoadGalleryAsync()
        {
            Images.Clear();
            var results = await _galleryService.GetGalleryAsync();
            
            /*foreach (var person in results) TODO : get rid of the bag and delete this part of the code 
            {
                var view = new DebugView(person.Person.ProfileImage);
                view.Show();
            }*/
            Images.AddRange(results);
            return Images;
        }
    }
}
