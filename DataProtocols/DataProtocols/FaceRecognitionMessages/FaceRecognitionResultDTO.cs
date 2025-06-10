using Newtonsoft.Json;

namespace DataProtocols.FaceRecognitionMessages
{
    [Serializable]
    public class FaceRecognitionResultDTO : Data
    {
        [JsonProperty]
        public List<FullPersonDataWithProfilePictureDTO> Results { get; set; }

        public FaceRecognitionResultDTO()
        {
            DataType = DataType.FaceRecognitionResult;
            Results = new List<FullPersonDataWithProfilePictureDTO>();
        }

        public FaceRecognitionResultDTO(List<FullPersonDataWithProfilePictureDTO> results)
        {
            DataType = DataType.FaceRecognitionResult;
            Results = results ?? new List<FullPersonDataWithProfilePictureDTO>();
        }
    }
}
