using DataProtocols.FaceRecognitionMessages.Models;
using Newtonsoft.Json;

namespace DataProtocols.FaceRecognitionMessages
{
    [Serializable]
    public class UpdatePersonDataResponseDTO : Data
    {
        [JsonProperty]
        public bool Success;

        [JsonProperty]
        public PersonDataValidationResultDTO ValidationResult;

        public UpdatePersonDataResponseDTO()
        {
            DataType = DataType.UpdatePersonDataResponse;
        }

        public UpdatePersonDataResponseDTO(bool success, PersonDataValidationResultDTO validationResult = null)
        {
            DataType = DataType.UpdatePersonDataResponse;
            Success = success;
            ValidationResult = validationResult;
        }
    }
}

