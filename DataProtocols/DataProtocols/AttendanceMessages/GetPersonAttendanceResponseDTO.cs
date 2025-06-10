using Newtonsoft.Json;
using System.Collections.Generic;

namespace DataProtocols.AttendanceMessages
{
    [JsonObject]
    public class GetPersonAttendanceResponseDTO : Data
    {
        // —————— No need to repeat PersonMinimalDTO at all, because the client
        // already knows “which person” was requested. We only send the times.
        [JsonProperty]
        public List<DateTime> AttendanceTimes { get; set; }

        [JsonProperty]
        public bool Success { get; set; }

        [JsonProperty]
        public string ErrorMessage { get; set; }

        public GetPersonAttendanceResponseDTO()
        {
            DataType = DataType.GetPersonAttendanceResponse;
            AttendanceTimes = new List<DateTime>();
            Success = false;
            ErrorMessage = null;
        }

        public GetPersonAttendanceResponseDTO(List<DateTime> times, bool success, string errorMessage = null)
        {
            DataType = DataType.GetPersonAttendanceResponse;
            AttendanceTimes = times ?? new List<DateTime>();
            Success = success;
            ErrorMessage = errorMessage;
        }
    }
}
