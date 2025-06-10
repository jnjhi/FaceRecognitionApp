using DataProtocols;
using DataProtocols.GalleryMessages.Models;
using DataProtocols.RetrievingPersonDataMessages;
using FaceRecognitionServer.Services.DataBases.ConnectionToTables;
using FaceRecognitionServer.Services.DataBases.Models;
using FaceRecognitionServer.Utils;

namespace FaceRecognitionServer.Services.RetrievingPersonDataService
{
    public class RetrievingPersonDataHandler : ITypedMessageHandler<GetAdvancedPersonDataWithProfilePictureByIdRequestDTO>
    {
        private readonly ISecureNetworkManager m_SecureNetworkManager;
        private readonly GalleryStorageSystem m_GalleryStorageSystem;

        public RetrievingPersonDataHandler(ISecureNetworkManager secureNetworkManager, GalleryStorageSystem galleryStorageSystem)
        {
            m_SecureNetworkManager = secureNetworkManager;
            m_GalleryStorageSystem = galleryStorageSystem;
        }

        public async Task HandleMessageAsync(GetAdvancedPersonDataWithProfilePictureByIdRequestDTO message, string ip)
        {
            Logger.LogInfo($"Received request to fetch person with ID {message.PersonId} from {ip}.");

            var result = TryFetchPerson(message.PersonId);

            var response = result ?? new GetAdvancedPersonDataWithProfilePictureByIdResponseDTO(false, null, "Person not found or failed to load.");

            var payload = ConvertUtils.Serialize(response);
            m_SecureNetworkManager.SendMessage(payload, ip);
        }

        private GetAdvancedPersonDataWithProfilePictureByIdResponseDTO TryFetchPerson(int personId)
        {
            AdvancedFaceDataWithProfilePicture person = null;
            FaceRecordWithProfilePictureDTO dto = null;

            try
            {
               person = m_GalleryStorageSystem.GetGallery().FirstOrDefault(p => p.Id == personId);//TODO : add a separate function for that
               dto = new FaceRecordWithProfilePictureDTO
               {
                   Id = person.Id,
                   GovernmentID = person.GovernmentID,
                   FirstName = person.FirstName,
                   LastName = person.LastName,
                   HeightCm = person.HeightCm,
                   Sex = person.Sex,
                   FaceEmbedding = person.FaceEmbedding,
                   Notes = person.Notes,
                   CaptureTime = person.CaptureDate,
                   Image = ImageConversionUtils.EncodeBitmapToBase64(person.profilePicture) // JPEG Base64
               };
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, $"Database failure while retrieving person with ID {personId}.");
                return null;
            }

            if (person == null)
            {
                Logger.LogCustomError($"Person with ID {personId} not found in gallery.");
                return null;
            }

            Logger.LogInfo($"Successfully retrieved person {personId}: {person.FirstName} {person.LastName}");

            return new GetAdvancedPersonDataWithProfilePictureByIdResponseDTO(true, dto);
        }
    }
}
