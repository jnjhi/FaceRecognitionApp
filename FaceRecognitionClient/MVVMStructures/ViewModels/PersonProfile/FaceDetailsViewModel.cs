using FaceRecognitionClient.Commands;
using FaceRecognitionClient.InternalDataModels;
using FaceRecognitionClient.MVVMStructures.Models.PersonProfile;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace FaceRecognitionClient.MVVMStructures.ViewModels.PersonProfile
{
    // ViewModel for editing face/person details.
    // Allows user to update fields like name, ID, sex, height, notes, and profile picture.
    // Errors for each field are tracked individually. Changes are submitted via the SaveCommand.
    public class FaceDetailsViewModel : BaseViewModel
    {
        private readonly FaceDetailsModel m_Model;
        private readonly AdvancedPersonData m_Record;

        // —— Image displayed for the selected person ——
        private BitmapImage _profileImage;
        public BitmapImage ProfileImage
        {
            get => _profileImage;
            set
            {
                _profileImage = value;
                OnPropertyChanged();
                ProfileImageError = string.Empty;
            }
        }

        // —— First name input and error tracking ——
        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
                FirstNameError = string.Empty;
            }
        }

        // —— Last name input and error tracking ——
        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
                LastNameError = string.Empty;
            }
        }

        // —— Government ID input and error tracking ——
        private string _governmentID;
        public string GovernmentID
        {
            get => _governmentID;
            set
            {
                _governmentID = value;
                OnPropertyChanged();
                GovernmentIDError = string.Empty;
            }
        }

        // —— Optional height in cm ——
        private int? _heightCm;
        public int? HeightCm
        {
            get => _heightCm;
            set
            {
                _heightCm = value;
                OnPropertyChanged();
            }
        }

        // —— Sex field (bound to ComboBox with predefined values) ——
        private string _sex;
        public string Sex
        {
            get => _sex;
            set
            {
                _sex = value;
                OnPropertyChanged();
            }
        }

        // —— Notes about this person ——
        private string _notes;
        public string Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                OnPropertyChanged();
                NotesError = string.Empty;
            }
        }

        // —— Options used to populate the sex ComboBox ——
        public ObservableCollection<string> SexOptions { get; } = new ObservableCollection<string> { "Male", "Female", "Other" };

        // —— Validation error messages —— 

        private string _profileImageError;
        public string ProfileImageError
        {
            get => _profileImageError;
            set
            {
                _profileImageError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsProfileImageErrorVisible));
            }
        }
        public bool IsProfileImageErrorVisible => !string.IsNullOrWhiteSpace(ProfileImageError);

        private string _firstNameError;
        public string FirstNameError
        {
            get => _firstNameError;
            set
            {
                _firstNameError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsFirstNameErrorVisible));
            }
        }
        public bool IsFirstNameErrorVisible => !string.IsNullOrWhiteSpace(FirstNameError);

        private string _lastNameError;
        public string LastNameError
        {
            get => _lastNameError;
            set
            {
                _lastNameError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLastNameErrorVisible));
            }
        }
        public bool IsLastNameErrorVisible => !string.IsNullOrWhiteSpace(LastNameError);

        private string _governmentIDError;
        public string GovernmentIDError
        {
            get => _governmentIDError;
            set
            {
                _governmentIDError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsGovernmentIDErrorVisible));
            }
        }
        public bool IsGovernmentIDErrorVisible => !string.IsNullOrWhiteSpace(GovernmentIDError);

        private string _notesError;
        public string NotesError
        {
            get => _notesError;
            set
            {
                _notesError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotesErrorVisible));
            }
        }
        public bool IsNotesErrorVisible => !string.IsNullOrWhiteSpace(NotesError);

        // —— Command to trigger saving/updating the record ——
        public AsyncRelayCommand SaveCommand { get; }

        public FaceDetailsViewModel(INetworkFacade networkFacade, AdvancedPersonDataWithImage record, Mapper mapper)
        {
            m_Record = record;
            m_Model = new FaceDetailsModel(networkFacade, mapper);

            // Preload values into the form
            ProfileImage = record.ProfileImage;
            FirstName = record.FirstName;
            LastName = record.LastName;
            GovernmentID = record.GovernmentID;
            HeightCm = record.HeightCm;
            Sex = record.Sex;
            Notes = record.Notes;

            SaveCommand = new AsyncRelayCommand(_ => SaveAsync());
        }

        // Updates the record object and attempts to save via model.
        // Displays field-specific validation errors if save fails.
        private async Task SaveAsync()
        {
            m_Record.FirstName = FirstName;
            m_Record.LastName = LastName;
            m_Record.GovernmentID = GovernmentID;
            m_Record.HeightCm = HeightCm;
            m_Record.Sex = Sex;
            m_Record.Notes = Notes;

            var response = await m_Model.SaveOrUpdateAsync(m_Record);

            if (!response.Success)
            {
                FirstNameError = response.ValidationResult.FirstNameError;
                LastNameError = response.ValidationResult.LastNameError;
                GovernmentIDError = response.ValidationResult.GovernmentIDError;
            }
        }
    }
}
