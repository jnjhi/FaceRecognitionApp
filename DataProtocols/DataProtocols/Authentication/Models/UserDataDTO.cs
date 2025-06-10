using Newtonsoft.Json;

namespace DataProtocols.Authentication.Models
{
    [Serializable]
    public class UserDataDTO
    {
        [JsonProperty]
        public int UserId { get; set; }

        [JsonProperty]
        public string Email { get; set; }

        [JsonProperty]
        public string UserName { get; set; }
    }
}
