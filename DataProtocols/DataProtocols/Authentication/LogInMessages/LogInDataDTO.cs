using Newtonsoft.Json;

namespace DataProtocols.Authentication.LogInMessages
{
    [Serializable]
    public class LogInDataDTO : Data
    {
        [JsonProperty]
        public string UserName;

        [JsonProperty]
        public string Password;

        public LogInDataDTO()
        {
            DataType = DataType.LogInData;
        }

        public LogInDataDTO(string userName, string password)
        {
            DataType = DataType.LogInData;
            UserName = userName;
            Password = password;
        }
    }
}
