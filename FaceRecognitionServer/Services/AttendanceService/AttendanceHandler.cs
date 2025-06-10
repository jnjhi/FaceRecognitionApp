using DataProtocols;
using DataProtocols.AttendanceMessages;
using FaceRecognitionServer.Services.DataBases.ConnectionToTables;

namespace FaceRecognitionServer.Services.AttendanceService
{
    public class AttendanceHandler : ITypedMessageHandler<GetAllAttendanceRequestDTO>, ITypedMessageHandler<GetPersonAttendanceRequestDTO>
    {
        private ISecureNetworkManager _network;
        private AttendanceStorageSystem _attendanceStorageSystem;
        private ConnectionToFaceTable _facesStorageSystem;

        public AttendanceHandler(ISecureNetworkManager secureNetworkManager, AttendanceStorageSystem attendanceStorageSystem, ConnectionToFaceTable facesStorageSystem)
        {
            _network = secureNetworkManager;
            _attendanceStorageSystem = attendanceStorageSystem;
            _facesStorageSystem = facesStorageSystem;
        }

        public Task HandleMessageAsync(GetPersonAttendanceRequestDTO message, string ip)
        {
            GetPersonAttendanceResponseDTO response;

            try
            {
                var attendances = _attendanceStorageSystem.GetAllAttendancesByUserId(message.RecognizedPersonId);
                List<DateTime> attendancesDates = attendances.ConvertAll(record => record.AttendanceTime);
                response = new GetPersonAttendanceResponseDTO(attendancesDates, true, "");
            }
            catch(Exception exception)
            {
                response = new GetPersonAttendanceResponseDTO(null, false, $"Retrieving attendance for user with an id of: {message.RecognizedPersonId} failed");
                Logger.LogException(exception, $"Retrieving attendance for user with an id of: {message.RecognizedPersonId} failed");
            }

            var requestInJason = ConvertUtils.Serialize(response);
            _network.SendMessage(requestInJason, ip);

            return Task.CompletedTask;
        }

        public Task HandleMessageAsync(GetAllAttendanceRequestDTO message, string ip)
        {
            GetAllAttendanceResponseDTO response;

            try
            {
                var attendances = _attendanceStorageSystem.GetAllAttendances();
                List<AttendanceEntryDTO> attendanceEntries = new List<AttendanceEntryDTO>();
                Dictionary<int, PersonMinimalDTO> attendeesDictionary = new Dictionary<int, PersonMinimalDTO>();

                foreach (var attendance in attendances)
                {
                    attendanceEntries.Add(new AttendanceEntryDTO(attendance.RecognizedPersonId, attendance.AttendanceTime));
                    var attendee = _facesStorageSystem.GetPersonInfoById(attendance.RecognizedPersonId);
                    if (!attendeesDictionary.ContainsKey(attendee.Id))
                    {
                        attendeesDictionary.Add(attendance.RecognizedPersonId, new PersonMinimalDTO(attendee.Id, attendee.GovernmentID, attendee.FirstName, attendee.LastName));
                    }
                }
                List<PersonMinimalDTO> attendeesList = attendeesDictionary.Values.ToList();
                response = new GetAllAttendanceResponseDTO(attendeesList, attendanceEntries, true, "");
            }
            catch (Exception exception)
            {
                response = new GetAllAttendanceResponseDTO(null, null, false, $"Failed to get attendance");
                Logger.LogException(exception, $"Failed to get attendance");
            }

            var requestInJason = ConvertUtils.Serialize(response);
            _network.SendMessage(requestInJason, ip);

            return Task.CompletedTask;
        }
    }
}
