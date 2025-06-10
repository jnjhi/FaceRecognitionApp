using Newtonsoft.Json;

namespace DataProtocols.AttendanceMessages
{
    [JsonObject]
    public class GetPersonAttendanceRequestDTO : Data
    {
        [JsonProperty]
        public int RecognizedPersonId { get; set; }

        public GetPersonAttendanceRequestDTO()
        {
            DataType = DataType.GetPersonAttendanceRequest;
        }

        public GetPersonAttendanceRequestDTO(int recognizedPersonId)
        {
            DataType = DataType.GetPersonAttendanceRequest;
            RecognizedPersonId = recognizedPersonId;
        }
    }
}
