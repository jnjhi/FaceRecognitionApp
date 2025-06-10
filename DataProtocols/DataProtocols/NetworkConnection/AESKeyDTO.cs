using Newtonsoft.Json;

namespace DataProtocols.NetworkConnection
{
    [Serializable]
    public class AESKeyDTO : Data
    {
        [JsonProperty]
        public string IV;

        [JsonProperty]
        public string Key;
    }
}
