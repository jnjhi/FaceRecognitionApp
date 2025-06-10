using Newtonsoft.Json;

namespace DataProtocols.GalleryMessages.Models
{
    public class FaceRecordWithProfilePictureDTO  : FaceRecordDTO
    {

        [JsonProperty]
        public string Image { get; set; }

        [JsonProperty]
        public DateTime CaptureTime { get; set; }
    }
}
