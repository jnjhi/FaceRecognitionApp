using Newtonsoft.Json;

namespace DataProtocols.GalleryMessages.Models
{
    public class FaceRecordDTO
    {
        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        public string GovernmentID { get; set; }

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

        public FaceRecordDTO()
        {

        }

        public FaceRecordDTO(int id, string governmentID, string firstName, string lastName, int? heightCm, string sex, float[] faceEmbedding, string notes)
        {
            Id = id;
            GovernmentID = governmentID;
            FirstName = firstName;
            LastName = lastName;
            HeightCm = heightCm;
            Sex = sex;
            FaceEmbedding = faceEmbedding;
            Notes = notes;
        }
    }
}
