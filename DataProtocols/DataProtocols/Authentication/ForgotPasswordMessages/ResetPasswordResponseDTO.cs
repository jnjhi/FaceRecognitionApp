using Newtonsoft.Json;

namespace DataProtocols.Authentication.ForgotPasswordMessages
{
    [Serializable]
    public class ResetPasswordResponseDTO : Data
    {
        [JsonProperty]
        public bool IsSuccessful { get; set; }

        [JsonProperty]
        public string Message { get; set; }

        public ResetPasswordResponseDTO()
        {
            DataType = DataType.ResetPasswordResponse;
        }

        public ResetPasswordResponseDTO(bool isSuccessful, string message) : this()
        {
            IsSuccessful = isSuccessful;
            Message = message;
        }
    }
}