using Newtonsoft.Json;

namespace DataProtocols.NetworkConnection
{
    [Serializable]
    public class RSAPublicKeyDTO : Data
    {
        [JsonProperty]
        public string PublicKey;
    }
}
