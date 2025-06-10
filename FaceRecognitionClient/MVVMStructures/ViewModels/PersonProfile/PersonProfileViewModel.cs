using FaceRecognitionClient.InternalDataModels;

namespace FaceRecognitionClient.MVVMStructures.ViewModels.PersonProfile
{
    public class PersonProfileViewModel : BaseViewModel
    {
        public PersonalDetailsViewModel PersonalDetailsViewModel { get; }
        public AttendanceRecordsViewModel AttendanceRecordsViewModel { get; }

        private bool m_AttendanceLoaded;

        // Current selected tab as enum
        private PersonProfileTab m_SelectedTab;

        // Bound to TabControl.SelectedIndex in XAML
        public int SelectedTabIndex
        {
            get => (int)m_SelectedTab;
            set
            {
                m_SelectedTab = (PersonProfileTab)value;
                OnPropertyChanged();

                HandleTabChange(m_SelectedTab);
            }
        }

        public PersonProfileViewModel(INetworkFacade network, Mapper mapper, AdvancedPersonDataWithImage person)
        {
            PersonalDetailsViewModel = new PersonalDetailsViewModel(network, person, mapper);
            AttendanceRecordsViewModel = new AttendanceRecordsViewModel(network, mapper, person);
        }

        private void HandleTabChange(PersonProfileTab selectedTab)
        {
            if (ShouldIgnoreTab(selectedTab))
                return;

            LoadAttendanceSafely();
        }

        private bool ShouldIgnoreTab(PersonProfileTab selectedTab)
        {
            return selectedTab != PersonProfileTab.AttendanceRecords || m_AttendanceLoaded;
        }

        private async void LoadAttendanceSafely()
        {
            try
            {
                await AttendanceRecordsViewModel.LoadAsync();
                m_AttendanceLoaded = true;
            }
            catch (Exception ex)
            {
                ClientLogger.ClientLogger.LogException(ex, "Failed to load attendance records in PersonProfileViewModel.");
            }
        }
    }
}
