using Newtonsoft.Json;

namespace DataProtocols.AttendanceMessages
{
    [JsonObject]
    public class PersonMinimalDTO
    {
        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        public string GovernmentID { get; set; }

        [JsonProperty]
        public string FirstName { get; set; }

        [JsonProperty]
        public string LastName { get; set; }

        public PersonMinimalDTO() { }

        public PersonMinimalDTO(int id, string governmentID, string firstName, string lastName)
        {
            Id = id;
            GovernmentID = governmentID;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
