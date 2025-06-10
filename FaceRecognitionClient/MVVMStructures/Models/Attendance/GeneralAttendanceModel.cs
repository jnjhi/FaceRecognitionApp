using DataProtocols.AttendanceMessages;
using DataProtocols.GalleryMessages.Models;
using DataProtocols.RetrievingPersonDataMessages;
using FaceRecognitionClient.InternalDataModels;
using FaceRecognitionClient.MVVMStructures.ViewModels.PersonProfile;

namespace FaceRecognitionClient.MVVMStructures.Models.Attendance
{
    public class GeneralAttendanceModel
    {
        private readonly INetworkFacade m_Network;
        private readonly Mapper m_Mapper;

        public GeneralAttendanceModel(INetworkFacade network, Mapper mapper)
        {
            m_Network = network;
            m_Mapper = mapper;
        }

        public async Task<List<AttendanceRecord>> GetAttendanceAsync()
        {
            try
            {
                var request = new GetAllAttendanceRequestDTO();
                var response = await m_Network.SendRequestAsync<GetAllAttendanceRequestDTO, GetAllAttendanceResponseDTO>(request);

                if (!response.Success)
                {
                    ClientLogger.ClientLogger.LogWarning($"Server failed to handle attendance : {response.ErrorMessage}");
                    return new List<AttendanceRecord>();
                }

                if (response.AttendanceEntries == null || response.Attendees == null)
                {
                    ClientLogger.ClientLogger.LogWarning($"Incomplete attendance response.");
                    return new List<AttendanceRecord>();
                }

                return CombineEntriesWithAttendees(response.AttendanceEntries, response.Attendees);
            }
            catch (Exception ex)
            {
                ClientLogger.ClientLogger.LogException(ex, $"Failed to load attendance records.");
                return new List<AttendanceRecord>();
            }
        }

        public async Task<AdvancedPersonDataWithImage> FetchFullPersonDataByIdAsync(int personId)
        {
            try
            {
                return await SendAndMapPersonRequest(personId);
            }
            catch (Exception ex)
            {
                ClientLogger.ClientLogger.LogException(ex, $"Exception while retrieving person data for ID {personId}.");
                return null;
            }
        }

        private async Task<AdvancedPersonDataWithImage> SendAndMapPersonRequest(int personId)
        {
            var request = new GetAdvancedPersonDataWithProfilePictureByIdRequestDTO(personId);
            var response = await m_Network.SendRequestAsync<GetAdvancedPersonDataWithProfilePictureByIdRequestDTO, GetAdvancedPersonDataWithProfilePictureByIdResponseDTO>(request);

            if (!response.Success)
            {
                LogPersonFailure(response.ErrorMessage, personId);
                return null;
            }

            if (response.Person == null)
            {
                LogPersonNull(personId);
                return null;
            }

            return m_Mapper.Map<FaceRecordWithProfilePictureDTO, AdvancedPersonDataWithImage>(response.Person);
        }

        private void LogPersonFailure(string error, int personId)
        {
            ClientLogger.ClientLogger.LogWarning($"Server rejected person data request for ID {personId}: {error}");
        }

        private void LogPersonNull(int personId)
        {
            ClientLogger.ClientLogger.LogWarning($"Received null person data for ID {personId}.");
        }
        private List<AttendanceRecord> CombineEntriesWithAttendees(List<AttendanceEntryDTO> entries, List<PersonMinimalDTO> people)
        {
            var attendeesMap = people.ToDictionary(p => p.Id);
            var output = new List<AttendanceRecord>();

            foreach (var entry in entries)
            {
                if (!attendeesMap.TryGetValue(entry.PersonId, out var person))
                {
                    ClientLogger.ClientLogger.LogWarning($"Missing person data for ID {entry.PersonId}.");
                    continue;
                }

                output.Add(new AttendanceRecord
                {
                    Id = person.Id,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    GovernmentId = person.GovernmentID,
                    AttendanceTime = entry.TimeStamp
                });
            }

            return output;
        }
    }
}
