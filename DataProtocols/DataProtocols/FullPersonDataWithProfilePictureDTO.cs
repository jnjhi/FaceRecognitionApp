using Newtonsoft.Json;
using System.Drawing;

namespace DataProtocols
{
    [Serializable]
    public class FullPersonDataWithProfilePictureDTO
    {
        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        public Rectangle Rectangle { get; set; }

        [JsonProperty]
        public string ProfilePicture { get; set; }

        [JsonProperty]
        public string GovernmentID { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string FirstName { get; set; }

        [JsonProperty]
        public string LastName { get; set; }

        [JsonProperty]
        public int? HeightCm { get; set; }

        [JsonProperty]
        public string Sex { get; set; }

        [JsonProperty]
        public float[] FaceEmbedding { get; set; }

        [JsonProperty]
        public string Notes { get; set; }
    }
}
