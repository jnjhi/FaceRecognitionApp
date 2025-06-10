using DataProtocols.AttendanceMessages;
using FaceRecognitionClient.InternalDataModels;
using FaceRecognitionClient.MVVMStructures.ViewModels.PersonProfile;

namespace FaceRecognitionClient.MVVMStructures.Models.PersonProfile
{
    public class AttendanceModel
    {
        private readonly INetworkFacade m_Network;
        private readonly Mapper m_Mapper;

        public AttendanceModel(INetworkFacade network, Mapper mapper)
        {
            m_Network = network;
            m_Mapper = mapper;
        }

        public async Task<List<AttendanceRecord>> GetAttendanceAsync(AdvancedPersonData person)
        {
            var request = new GetPersonAttendanceRequestDTO(person.Id);

            var response = await m_Network.SendRequestAsync<GetPersonAttendanceRequestDTO, GetPersonAttendanceResponseDTO>(request);

            return ConvertToInternalFormat(response, person);
        }

        private List<AttendanceRecord> ConvertToInternalFormat(GetPersonAttendanceResponseDTO response, AdvancedPersonData person)
        {
            var results = new List<AttendanceRecord>();

            if (!response.Success || response.AttendanceTimes == null)
            {
                return results;
            }

            foreach (var time in response.AttendanceTimes)
            {
                results.Add(new AttendanceRecord
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    GovernmentId = person.GovernmentID,
                    AttendanceTime = time
                });
            }

            return results;
        }
    }
}