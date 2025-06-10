using FaceRecognitionClient.Commands;
using FaceRecognitionClient.MVVMStructures.Models.Authentication;
using FaceRecognitionClient.StateMachine;
using System.Collections.ObjectModel;

namespace FaceRecognitionClient.MVVMStructures.ViewModels.Authentication
{
    // ViewModel for handling the user registration form.
    // It validates the inputs, sends the signup request to the server, and triggers the state transitions.
    public class SignUpViewModel : BaseViewModel, IStateNotifier
    {
        public event Action<ApplicationTrigger> OnTriggerOccurred;

        public AsyncRelayCommand OnSignUp => new AsyncRelayCommand(execute => OnSignUpButtonClick());
        public RelayCommand SwitchToSignUpWindow => new RelayCommand(execute => OnSwitchToLogInWindowButtonClick());

        private SignUpModel m_SignUpModel;

        private string m_Username;
        private string m_Password;
        private string m_FirstName;
        private string m_LastName;
        private string m_Email;
        private string m_SelectedCity;

        private string m_UserNameError;
        private string m_FirstNameError;
        private string m_PasswordError;
        private string m_LastNameError;
        private string m_EmailError;

        // Error message for username field
        public string UserNameError
        {
            get => m_UserNameError;
            set { m_UserNameError = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsUserNameErrorVisible)); }
        }

        public bool IsUserNameErrorVisible => !string.IsNullOrWhiteSpace(UserNameError);

        // Error message for password field
        public string PasswordError
        {
            get => m_PasswordError;
            set { m_PasswordError = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsPasswordErrorVisible)); }
        }

        public bool IsPasswordErrorVisible => !string.IsNullOrWhiteSpace(PasswordError);

        // Error message for first name field
        public string FirstNameError
        {
            get => m_FirstNameError;
            set { m_FirstNameError = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsFirstNameErrorVisible)); }
        }

        public bool IsFirstNameErrorVisible => !string.IsNullOrWhiteSpace(FirstNameError);

        // Error message for last name field
        public string LastNameError
        {
            get => m_LastNameError;
            set { m_LastNameError = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsLastNameErrorVisible)); }
        }

        public bool IsLastNameErrorVisible => !string.IsNullOrWhiteSpace(LastNameError);

        // Error message for email field
        public string EmailError
        {
            get => m_EmailError;
            set { m_EmailError = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsEmailErrorVisible)); }
        }

        public bool IsEmailErrorVisible => !string.IsNullOrWhiteSpace(EmailError);

        // Bound properties for input fields
        public string UserName
        {
            get => m_Username;
            set { m_Username = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => m_Password;
            set { m_Password = value; OnPropertyChanged(); }
        }

        public string FirstName
        {
            get => m_FirstName;
            set { m_FirstName = value; OnPropertyChanged(); }
        }

        public string LastName
        {
            get => m_LastName;
            set { m_LastName = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => m_Email;
            set { m_Email = value; OnPropertyChanged(); }
        }

        public string SelectedCity
        {
            get => m_SelectedCity;
            set { m_SelectedCity = value; OnPropertyChanged(); }
        }

        // List of available cities for the combo box
        public ObservableCollection<string> Cities { get; set; }

        public SignUpViewModel(INetworkFacade network, UserSession userSession)
        {
            m_SignUpModel = new SignUpModel(network, userSession);
            InitializeTheCitiesComboBox();
        }

        // Called when user clicks the sign-up button
        private async Task OnSignUpButtonClick()
        {
            // Validate all user input
            var validation = LogInLegitimacyCheckingUtils.ValidateSignUp(UserName, Password, FirstName, LastName, Email);

            UserNameError = validation.UserNameError;
            PasswordError = validation.PasswordError;
            FirstNameError = validation.FirstNameError;
            LastNameError = validation.LastNameError;
            EmailError = validation.EmailError;

            if (!validation.IsValid)
                return;

            // Try signing up via the model
            var answer = await m_SignUpModel.SignUp(UserName, Password, FirstName, LastName, Email, SelectedCity);

            // Show server-side validation errors
            if (answer.ValidationResult is not null)
            {
                UserNameError = answer.ValidationResult.UserNameError;
                PasswordError = answer.ValidationResult.PasswordError;
                FirstNameError = answer.ValidationResult.FirstNameError;
                LastNameError = answer.ValidationResult.LastNameError;
                EmailError = answer.ValidationResult.EmailError;
            }

            // Success/failure triggers
            if (!answer.IsSignUpSuccessful)
            {
                OnSignUpFailed();
                return;
            }

            OnSignUpSuccessful();
        }

        // Switch to login screen
        private void OnSwitchToLogInWindowButtonClick() => OnTriggerOccurred?.Invoke(ApplicationTrigger.LogInRequested);

        private void OnSignUpSuccessful() => OnTriggerOccurred?.Invoke(ApplicationTrigger.SignUpSuccessful);
        private void OnSignUpFailed() => OnTriggerOccurred?.Invoke(ApplicationTrigger.SignUpFailed);

        // Predefined list of cities shown to the user
        private void InitializeTheCitiesComboBox()
        {
            Cities = new ObservableCollection<string>
            {
                "Jerusalem", "Tel Aviv", "Haifa", "Rishon LeZion", "Petah Tikva", "Ashdod", "Netanya", "Be'er Sheva",
                "Bnei Brak", "Holon", "Ramat Gan", "Rehovot", "Ashkelon", "Bat Yam", "Beit Shemesh", "Herzliya", "Kfar Saba",
                "Hadera", "Modi'in-Maccabim-Re'ut", "Nazareth", "Lod", "Raanana", "Rosh HaAyin", "Kiryat Ata", "Eilat",
                "Givatayim", "Nahariya", "Acre (Akko)", "Kiryat Gat", "Kiryat Bialik", "Karmiel", "Kiryat Motzkin", "Tiberias",
                "Umm al-Fahm", "Tira", "Yavne", "Or Yehuda", "Dimona", "Sakhnin", "Ma'ale Adumim", "Tirat Carmel",
                "Tamra", "Migdal HaEmek", "Nesher", "Sderot", "Kiryat Shmona", "Arad", "Ofakim", "Gan Yavne", "Kiryat Malakhi"
            };
        }
    }
}
