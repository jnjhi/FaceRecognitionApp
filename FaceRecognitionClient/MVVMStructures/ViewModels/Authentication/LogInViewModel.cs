using FaceRecognitionClient.Commands;
using FaceRecognitionClient.MVVMStructures.Models.Authentication;
using FaceRecognitionClient.StateMachine;

namespace FaceRecognitionClient.MVVMStructures.ViewModels.Authentication
{
    // ViewModel for login screen.
    // Handles login attempts, error display, and navigation to other screens like sign-up or forgot password.
    public class LogInViewModel : BaseViewModel, IStateNotifier
    {
        public event Action<ApplicationTrigger> OnTriggerOccurred;

        public AsyncRelayCommand LogInCommand => new AsyncRelayCommand(execute => OnLogInButtonClick());
        public RelayCommand SwitchToSignUpWindow => new RelayCommand(execute => OnSwitchToSignUpWindowButtonClick());
        public RelayCommand SkipLogInCommand => new RelayCommand(execute => OnSkipLogInButtonClick());
        public RelayCommand ForgotPasswordCommand => new RelayCommand(_ => OnForgotPasswordRequested());

        private LogInModel m_LogIn;

        private string m_UserName;
        private string m_Password;

        private string m_UserNameError;
        private string m_PasswordError;

        public string UserName
        {
            get => m_UserName;
            set
            {
                m_UserName = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => m_Password;
            set => m_Password = value;
        }

        public string UserNameError
        {
            get => m_UserNameError;
            set
            {
                m_UserNameError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsUserNameErrorVisible));
            }
        }

        public string PasswordError
        {
            get => m_PasswordError;
            set
            {
                m_PasswordError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsPasswordErrorVisible));
            }
        }

        public bool IsUserNameErrorVisible => !string.IsNullOrWhiteSpace(UserNameError);
        public bool IsPasswordErrorVisible => !string.IsNullOrWhiteSpace(PasswordError);

        public LogInViewModel(INetworkFacade networkFacade, UserSession userSession)
        {
            m_LogIn = new LogInModel(networkFacade, userSession);
        }

        // Attempts to log in using the model, handles error and success states.
        private async Task OnLogInButtonClick()
        {
            UserNameError = string.Empty;
            PasswordError = string.Empty;

            var result = await m_LogIn.LogIn(m_UserName, m_Password);

            if (result.ValidationResult is not null)
            {
                UserNameError = result.ValidationResult.UserNameError;
                PasswordError = result.ValidationResult.PasswordError;
            }

            if (result.IsAccessGranted)
            {
                OnSuccessfulLogIn();
            }
            else
            {
                OnFailedLogIn();
            }
        }

        private void OnSwitchToSignUpWindowButtonClick() => OnTriggerOccurred?.Invoke(ApplicationTrigger.SignUpRequested);
        private void OnSuccessfulLogIn() => OnTriggerOccurred?.Invoke(ApplicationTrigger.LoginSuccessful);
        private void OnSkipLogInButtonClick() => OnTriggerOccurred?.Invoke(ApplicationTrigger.DeveloperEntered);
        private void OnForgotPasswordRequested() => OnTriggerOccurred?.Invoke(ApplicationTrigger.ForgotPasswordRequested);

        private void OnFailedLogIn()
        {
            OnTriggerOccurred?.Invoke(ApplicationTrigger.LogInFailed);
            Password = string.Empty;
        }
    }
}
