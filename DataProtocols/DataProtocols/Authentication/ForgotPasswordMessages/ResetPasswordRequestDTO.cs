using Newtonsoft.Json;

namespace DataProtocols.Authentication.ForgotPasswordMessages
{
    [Serializable]
    public class ResetPasswordRequestDTO : Data
    {
        [JsonProperty]
        public string Email { get; set; }

        [JsonProperty]
        public string Code { get; set; }

        [JsonProperty]
        public string NewPassword { get; set; }

        public ResetPasswordRequestDTO()
        {
            DataType = DataType.ResetPasswordRequest;
        }

        public ResetPasswordRequestDTO(string email, string code, string newPassword) : this()
        {
            Email = email;
            Code = code;
            NewPassword = newPassword;
        }
    }
}
