using DataProtocols.GalleryMessages.Models;
using Newtonsoft.Json;

namespace DataProtocols.GalleryMessages
{
    public class GetGalleryResponseDTO : Data
    {
        [JsonProperty]
        public List<FaceRecordWithProfilePictureDTO> Persons { get; set; }

        public GetGalleryResponseDTO()
        {
            DataType = DataType.GetGalleryResponse;
        }
    }
}
