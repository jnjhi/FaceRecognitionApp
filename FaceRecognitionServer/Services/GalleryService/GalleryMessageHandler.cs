using DataProtocols.GalleryMessages;
using DataProtocols.GalleryMessages.Models;
using FaceRecognitionServer.Services;
using FaceRecognitionServer.Services.DataBases.ConnectionToTables;
using FaceRecognitionServer.Utils;
using Newtonsoft.Json;

namespace FaceRecognitionServer
{
    public class GalleryMessageHandler : ITypedMessageHandler<GetGalleryRequestDTO>
    {
        private readonly ISecureNetworkManager _networkManager;
        private readonly GalleryStorageSystem _galleryDatabase;

        public GalleryMessageHandler(ISecureNetworkManager networkManager, GalleryStorageSystem galleryStorageSystem)
        {
            _networkManager = networkManager;
            _galleryDatabase = galleryStorageSystem;
        }

        public async Task HandleMessageAsync(GetGalleryRequestDTO message, string ip)
        {
            var gallery = _galleryDatabase.GetGallery();

            var output = new GetGalleryResponseDTO
            {
                Persons = new List<FaceRecordWithProfilePictureDTO>()
            };

            foreach (var image in gallery)
            {
                var dto = new FaceRecordWithProfilePictureDTO
                {
                    Id = image.Id,
                    GovernmentID = image.GovernmentID,
                    FirstName = image.FirstName,
                    LastName = image.LastName,
                    HeightCm = image.HeightCm,
                    Sex = image.Sex,
                    FaceEmbedding = image.FaceEmbedding,
                    Notes = image.Notes,
                    CaptureTime = image.CaptureDate,
                    Image = ImageConversionUtils.EncodeBitmapToBase64(_galleryDatabase.GetProfileImageById(image.Id)) // JPEG Base64
                };

                output.Persons.Add(dto);
            }

            string jsonResponse = JsonConvert.SerializeObject(output);
            _networkManager.SendMessage(jsonResponse, ip);
        }
    }
}

