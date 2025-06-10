using Newtonsoft.Json;

namespace DataProtocols.AttendanceMessages
{
    [JsonObject]
    public class GetAllAttendanceRequestDTO : Data
    {
        public GetAllAttendanceRequestDTO()
        {
            DataType = DataType.GetAllAttendanceRequest;
        }
    }
}
