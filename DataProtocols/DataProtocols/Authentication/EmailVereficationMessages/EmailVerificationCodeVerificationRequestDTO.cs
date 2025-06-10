using Newtonsoft.Json;

namespace DataProtocols.Authentication.EmailVereficationMessages
{
    public class EmailVerificationCodeVerificationRequestDTO : Data
    {
        [JsonProperty]
        public string Email;

        [JsonProperty]
        public string Code;

        public EmailVerificationCodeVerificationRequestDTO()
        {
            DataType = DataType.EmailVerificationCodeVerificationRequest;
        }
    }

}
