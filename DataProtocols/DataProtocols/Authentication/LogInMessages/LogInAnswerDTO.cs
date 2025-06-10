using DataProtocols.Authentication.ErrorMessages;
using DataProtocols.Authentication.Models;
using Newtonsoft.Json;

namespace DataProtocols.Authentication.LogInMessages
{
    public class LogInAnswerDTO : Data
    {
        [JsonProperty]
        public bool IsAccessGranted;

        [JsonProperty]
        public ValidationResultDTO ValidationResult;

        [JsonProperty]
        public UserDataDTO UserData;

        public LogInAnswerDTO(bool isAccessGranted, ValidationResultDTO validationResult = null, UserDataDTO userData = null)
        {
            IsAccessGranted = isAccessGranted;
            ValidationResult = validationResult;
            UserData = userData;
        }

        public LogInAnswerDTO()
        {
            DataType = DataType.LogInAnswer;
        }
    }
}
