using DataProtocols.GalleryMessages.Models;

namespace DataProtocols.RetrievingPersonDataMessages
{
    public class GetAdvancedPersonDataWithProfilePictureByIdResponseDTO : Data
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public FaceRecordWithProfilePictureDTO Person { get; set; }

        public GetAdvancedPersonDataWithProfilePictureByIdResponseDTO()
        {
            DataType = DataType.GetAdvancedPersonDataWithProfilePictureByIdResponse;
        }

        public GetAdvancedPersonDataWithProfilePictureByIdResponseDTO(bool success, FaceRecordWithProfilePictureDTO person, string errorMessage = null) : this()
        {
            Success = success;
            Person = person;
            ErrorMessage = errorMessage;
        }
    }
}
