using DataProtocols.Authentication.ErrorMessages;
using DataProtocols.Authentication.Models;
using Newtonsoft.Json;

namespace DataProtocols.Authentication.SignUpMessages
{
    [Serializable]
    public class SignUpAnswerDTO : Data
    {
        [JsonProperty]
        public bool IsSignUpSuccessful;

        [JsonProperty]
        public ValidationResultDTO ValidationResult;

        [JsonProperty]
        public UserDataDTO UserData;


        public SignUpAnswerDTO()
        {
            DataType = DataType.SignUpAnswer;
        }

        public SignUpAnswerDTO(bool isSignUpSuccessful, ValidationResultDTO validationResult = null, UserDataDTO userData = null)
        {
            IsSignUpSuccessful = isSignUpSuccessful;
            ValidationResult = validationResult;
            UserData = userData;
        }
    }

}
