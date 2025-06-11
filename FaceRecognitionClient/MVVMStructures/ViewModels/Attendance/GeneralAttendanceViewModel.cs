using DataProtocols.GalleryMessages.Models;
using DataProtocols.RetrievingPersonDataMessages;
using FaceRecognitionClient.Commands;
using FaceRecognitionClient.InternalDataModels;
using FaceRecognitionClient.MVVMStructures.Models.Attendance;
using FaceRecognitionClient.MVVMStructures.ViewModels.PersonProfile;
using FaceRecognitionClient.StateMachine;
using FaceRecognitionClient.Utils;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace FaceRecognitionClient.MVVMStructures.ViewModels.Attendance
{
    public class GeneralAttendanceViewModel : BaseViewModel, IDetailNotifier<AdvancedPersonDataWithImage>, IStateNotifier
    {
        private readonly INetworkFacade m_Network;
        private readonly Mapper m_Mapper;
        private readonly GeneralAttendanceModel m_Model;
        private AttendanceRecord m_SelectedAttendanceRecord;
        private bool m_SortDescending = true;

        public ObservableCollection<AttendanceRecord> AttendanceRecords { get; } = new();
        public ICollectionView AttendanceView { get; }
        public AsyncRelayCommand RefreshCommand { get; }
        public AsyncRelayCommand OpenProfileCommand { get; }

        public bool SortDescending
        {
            get => m_SortDescending;
            set
            {
                m_SortDescending = value;
                OnPropertyChanged();
                ApplySort();
            }
        }

        public event Action<AdvancedPersonDataWithImage> OnDetailRequested;

        public event Action<ApplicationTrigger> OnTriggerOccurred;

        public RelayCommand BackCommand { get; }
        public RelayCommand ExportCommand { get; }

        public AttendanceRecord SelectedAttendanceRecord
        {
            get => m_SelectedAttendanceRecord;
            set
            {
                m_SelectedAttendanceRecord = value;
                OnPropertyChanged();
            }
        }

        public GeneralAttendanceViewModel(INetworkFacade network, Mapper mapper)
        {
            m_Network = network;
            m_Mapper = mapper;
            m_Model = new GeneralAttendanceModel(network, mapper);

            AttendanceView = CollectionViewSource.GetDefaultView(AttendanceRecords);
            AttendanceView.SortDescriptions.Add(new SortDescription(nameof(AttendanceRecord.AttendanceTime), ListSortDirection.Descending));

            RefreshCommand = new AsyncRelayCommand(_ => LoadAsync());
            OpenProfileCommand = new AsyncRelayCommand(_ => OpenProfileAsync());
            BackCommand = new RelayCommand(_ => OnTriggerOccurred?.Invoke(ApplicationTrigger.NavigationRequested));
            ExportCommand = new RelayCommand(_ => ExportRecords());
        }

        public async Task LoadAsync()
        {
            try
            {
                var records = await m_Model.GetAttendanceAsync();

                AttendanceRecords.Clear();
                foreach (var record in records)
                { 
                    AttendanceRecords.Add(record);
                }
                    
                ApplySort();
            }
            catch (Exception ex)
            {
                ClientLogger.ClientLogger.LogException(ex, "Failed to load general attendance.");
            }
        }

        private void ApplySort()
        {
            AttendanceView.SortDescriptions.Clear();

            var sortDirection = SortDescending ? ListSortDirection.Descending : ListSortDirection.Ascending;

            AttendanceView.SortDescriptions.Add(new SortDescription(nameof(AttendanceRecord.AttendanceTime), sortDirection));
        }

        private async Task OpenProfileAsync()
        {
            if (SelectedAttendanceRecord == null)
                return;

            try
            {
                var request = new GetAdvancedPersonDataWithProfilePictureByIdRequestDTO(SelectedAttendanceRecord.Id);
                var response = await m_Network.SendRequestAsync<GetAdvancedPersonDataWithProfilePictureByIdRequestDTO, GetAdvancedPersonDataWithProfilePictureByIdResponseDTO>(request);

                if (!response.Success || response.Person == null)
                {
                    ClientLogger.ClientLogger.LogWarning($"Failed to retrieve profile for person ID {SelectedAttendanceRecord.Id}: {response.ErrorMessage}");
                    return;
                }

                var personData = m_Mapper.Map<FaceRecordWithProfilePictureDTO, AdvancedPersonDataWithImage>(response.Person);
                OnOpenPersonProfile(personData);
            }
            catch (Exception ex)
            {
                ClientLogger.ClientLogger.LogException(ex, $"Exception opening person profile window for ID {SelectedAttendanceRecord?.Id}.");
            }
        }

        private void ExportRecords()
        {
            var directory = AttendanceExportUtils.PromptForDirectory();
            if (!string.IsNullOrWhiteSpace(directory))
            {
                AttendanceExportUtils.Export(AttendanceRecords, directory);
            }
        }

        private void OnOpenPersonProfile(AdvancedPersonDataWithImage advancedPersonData) => OnDetailRequested?.Invoke(advancedPersonData);
    }
}
