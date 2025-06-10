using Newtonsoft.Json;

namespace DataProtocols.AttendanceMessages
{
    [JsonObject]
    public class GetAllAttendanceResponseDTO : Data
    {
        [JsonProperty]
        public List<PersonMinimalDTO> Attendees { get; set; }

        [JsonProperty]
        public List<AttendanceEntryDTO> AttendanceEntries { get; set; }

        [JsonProperty]
        public bool Success { get; set; }

        [JsonProperty]
        public string ErrorMessage { get; set; }

        public GetAllAttendanceResponseDTO()
        {
            DataType = DataType.GetAllAttendanceResponse;
            Attendees = null;
            AttendanceEntries = null;
            Success = false;
            ErrorMessage = null;
        }

        public GetAllAttendanceResponseDTO(List<PersonMinimalDTO> attendees, List<AttendanceEntryDTO> attendanceEntries, bool success, string errorMessage = null)
        {
            DataType = DataType.GetAllAttendanceResponse;
            Attendees = attendees;
            AttendanceEntries = attendanceEntries;
            Success = success;
            ErrorMessage = errorMessage;
        }
    }
}
