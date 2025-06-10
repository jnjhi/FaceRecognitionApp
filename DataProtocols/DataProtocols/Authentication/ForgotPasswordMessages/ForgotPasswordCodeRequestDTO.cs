using Newtonsoft.Json;

namespace DataProtocols.Authentication.ForgotPasswordMessages
{
    [Serializable]
    public class ForgotPasswordCodeRequestDTO : Data
    {
        [JsonProperty]
        public string Email { get; set; }

        public ForgotPasswordCodeRequestDTO()
        {
            DataType = DataType.ForgotPasswordCodeRequest;
        }

        public ForgotPasswordCodeRequestDTO(string email) : this()
        {
            Email = email;
        }
    }
}
