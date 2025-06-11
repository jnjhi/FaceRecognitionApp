using FaceRecognitionClient.Commands;
using FaceRecognitionClient.InternalDataModels;
using FaceRecognitionClient.MVVMStructures.Models.PersonProfile;
using System.Collections.ObjectModel;

namespace FaceRecognitionClient.MVVMStructures.ViewModels.PersonProfile
{
    public class AttendanceRecordsViewModel : BaseViewModel
    {
        private readonly AttendanceModel m_Model;
        private readonly AdvancedPersonData m_Person;

        public ObservableCollection<AttendanceRecord> AttendanceRecords { get; } = new();

        public AsyncRelayCommand RefreshCommand { get; }

        public AttendanceRecordsViewModel(INetworkFacade network, Mapper mapper, AdvancedPersonData person)
        {
            m_Model = new AttendanceModel(network, mapper);
            m_Person = person;

            RefreshCommand = new AsyncRelayCommand(_ => LoadAsync());
        }

        public async Task LoadAsync()
        {
            try
            {
                var records = await m_Model.GetAttendanceAsync(m_Person);

                AttendanceRecords.Clear();
                foreach (var record in records)
                    AttendanceRecords.Add(record);
            }
            catch (Exception ex)
            {
                ClientLogger.ClientLogger.LogException(ex, "Failed to load attendance in AttendanceRecordsViewModel.");
            }
        }
    }
}
