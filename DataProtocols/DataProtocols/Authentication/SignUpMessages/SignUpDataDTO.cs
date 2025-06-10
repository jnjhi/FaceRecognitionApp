using Newtonsoft.Json;

namespace DataProtocols.Authentication.SignUpMessages
{
    [Serializable]
    public class SignUpDataDTO : Data
    {
        [JsonProperty]
        public string UserName;

        [JsonProperty]
        public string Password;

        [JsonProperty]
        public string FirstName;

        [JsonProperty]
        public string LastName;

        [JsonProperty]
        public string Email;

        [JsonProperty]
        public string City;

        public SignUpDataDTO(string username, string password, string firstName, string lastName, string email, string city)
        {
            DataType = DataType.SignUpData;
            UserName = username;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            City = city;
        }
    }
}
