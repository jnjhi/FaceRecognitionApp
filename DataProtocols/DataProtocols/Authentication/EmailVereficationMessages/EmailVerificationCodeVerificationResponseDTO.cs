using Newtonsoft.Json;

namespace DataProtocols.Authentication.EmailVereficationMessages
{
    public class EmailVerificationCodeVerificationResponseDTO : Data
    {
        [JsonProperty]
        public bool IsVerified;

        public EmailVerificationCodeVerificationResponseDTO()
        {
            DataType = DataType.EmailVerificationCodeVerificationResponse;
        }
    }

}
