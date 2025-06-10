using DataProtocols.FaceRecognitionMessages.Models;
using Newtonsoft.Json;

namespace DataProtocols.FaceRecognitionMessages
{
    [Serializable]
    public class PreRecognitionFaceDataDTO : Data
    {
        [JsonProperty]
        public List<PreRecognitionDataDTO> Faces { get; set; }

        public PreRecognitionFaceDataDTO()
        {
            DataType = DataType.RecognizeAndLogFaceRequest;
            Faces = new List<PreRecognitionDataDTO>();
        }

        public PreRecognitionFaceDataDTO(List<PreRecognitionDataDTO> faces)
        {
            DataType = DataType.RecognizeAndLogFaceRequest;
            Faces = faces ?? new List<PreRecognitionDataDTO>();
        }
    }
}
