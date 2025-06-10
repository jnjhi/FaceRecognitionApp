using FaceRecognitionClient;
using FaceRecognitionClient.Commands;
using FaceRecognitionClient.MVVMStructures.Models.Authentication;
using FaceRecognitionClient.MVVMStructures.ViewModels;
using FaceRecognitionClient.StateMachine;

namespace FaceRecognitionClient.MVVMStructures.ViewModels.Authentication
{
    // Handles the logic for verifying a user's email after signup or password reset.
    // Sends a verification code to the user's email and checks the user's input against it.
    // If successful, it notifies the state machine via ApplicationTrigger.EmailVerified.
    public class EmailVerificationViewModel : BaseViewModel, IStateNotifier
    {
        private readonly EmailVerificationModel m_Model;
        private string m_UserInputCode;
        private string m_StatusMessage;
        private bool m_IsSending;

        public event Action<ApplicationTrigger> OnTriggerOccurred;

        // The verification code typed by the user.
        public string UserInputCode
        {
            get => m_UserInputCode;
            set { m_UserInputCode = value; OnPropertyChanged(); }
        }

        // Message shown to the user (e.g., success or error messages).
        public string StatusMessage
        {
            get => m_StatusMessage;
            set { m_StatusMessage = value; OnPropertyChanged(); }
        }

        // Indicates whether the code is currently being sent (used to disable the button).
        public bool IsSending
        {
            get => m_IsSending;
            set { m_IsSending = value; OnPropertyChanged(); }
        }

        // Command to trigger sending the code.
        public RelayCommand SendCodeCommand { get; }

        // Command to trigger verification.
        public AsyncRelayCommand VerifyCodeCommand { get; }

        public EmailVerificationViewModel(INetworkFacade networkFacade, UserSession userSession)
        {
            m_Model = new EmailVerificationModel(networkFacade, userSession);

            SendCodeCommand = new RelayCommand(async _ => await SendCodeAsync(), _ => !IsSending);
            VerifyCodeCommand = new AsyncRelayCommand(async _ => await VerifyCodeAsync());
        }

        // Called when the view is first activated — automatically sends a code.
        public async Task OnActivatedAsync()
        {
            await SendCodeAsync();
        }

        // Sends the verification code via the model.
        private async Task SendCodeAsync()
        {
            IsSending = true;
            StatusMessage = "Sending verification code...";

            try
            {
                await m_Model.RequestVerificationCodeAsync();
                StatusMessage = "Code sent. Please check your email.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to send: {ex.Message}";
            }

            IsSending = false;
        }

        // Verifies the code the user typed.
        private async Task VerifyCodeAsync()
        {
            if (string.IsNullOrWhiteSpace(UserInputCode))
            {
                StatusMessage = "Please enter the verification code.";
                return;
            }

            bool isVerified = await m_Model.SubmitCodeForVerificationAsync(UserInputCode);

            if (isVerified)
            {
                StatusMessage = "✔ Code verified!";
                OnTriggerOccurred?.Invoke(ApplicationTrigger.EmailVerified);
            }
            else
            {
                StatusMessage = "✘ Code incorrect. Try again.";
                UserInputCode = string.Empty;
            }
        }
    }
}
