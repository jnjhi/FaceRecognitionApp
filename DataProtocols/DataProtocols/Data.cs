using Newtonsoft.Json;

namespace DataProtocols
{
    [Serializable]
    public class Data
    {
        [JsonProperty]
        public DataType DataType;
    }
}
