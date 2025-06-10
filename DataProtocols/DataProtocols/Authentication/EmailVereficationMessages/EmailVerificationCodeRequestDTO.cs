using Newtonsoft.Json;

namespace DataProtocols.Authentication.EmailVereficationMessages
{
    public class EmailVerificationCodeRequestDTO : Data
    {
        [JsonProperty]
        public string Email;

        public EmailVerificationCodeRequestDTO()
        {
            DataType = DataType.EmailVerificationCodeRequest;
        }

        public EmailVerificationCodeRequestDTO(string email)
        {
            DataType = DataType.EmailVerificationCodeRequest;
            Email = email;
        }
    }
}
