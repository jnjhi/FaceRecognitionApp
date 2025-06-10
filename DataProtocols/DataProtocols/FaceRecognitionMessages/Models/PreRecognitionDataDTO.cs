using Newtonsoft.Json;
using System.Drawing;

namespace DataProtocols.FaceRecognitionMessages.Models
{
    [Serializable]
    public class PreRecognitionDataDTO
    {
        [JsonProperty]
        public string ProfilePictureInString64 { get; set; }

        [JsonProperty]
        public float[] Embedding { get; set; }

        [JsonProperty]
        public Rectangle Rectangle { get; set; }
      
        [JsonProperty]
        public DateTime CaptureTime { get; set; }
    }
}
