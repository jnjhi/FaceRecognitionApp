using Newtonsoft.Json;

namespace DataProtocols.AttendanceMessages
{
    //TODO: change the names to something more meaningful 
    [JsonObject]
    public class AttendanceEntryDTO
    {
        [JsonProperty]
        public int PersonId { get; set; }

        [JsonProperty]
        public DateTime TimeStamp { get; set; }

        public AttendanceEntryDTO() { }

        public AttendanceEntryDTO(int personId, DateTime timeStamp)
        {
            PersonId = personId;
            TimeStamp = timeStamp;
        }
    }
}
