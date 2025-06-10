using Newtonsoft.Json;

namespace DataProtocols.GalleryMessages.Models
{
    public class ProfilePictureDTO : Data
    {
        [JsonProperty]
        public int IdentifiedPersonId { get; set; }

        [JsonProperty]
        public string ImageInString64 { get; set; }

        [JsonProperty]
        public DateTime CaptureTime { get; set; }
    }
}
