using FaceRecognitionClient.Commands;
using FaceRecognitionClient.MVVMStructures.Models.Authentication;
using FaceRecognitionClient.MVVMStructures.ViewModels;
using FaceRecognitionClient.StateMachine;

namespace FaceRecognitionClient.MVVMStructures.ViewModels.Authentication
{
    // Handles the forgot password flow.
    // Sends a reset code to the email and allows the user to reset the password using that code.
    public class ForgotPasswordViewModel : BaseViewModel, IStateNotifier
    {
        public event Action<ApplicationTrigger> OnTriggerOccurred;

        private readonly ForgotPasswordModel m_Model;

        private string m_Email;
        private string m_Code;
        private string m_NewPassword;
        private string m_ConfirmPassword;
        private string m_Status;

        // User input fields
        public string Email { get => m_Email; set { m_Email = value; OnPropertyChanged(); } }
        public string Code { get => m_Code; set { m_Code = value; OnPropertyChanged(); } }
        public string NewPassword { get => m_NewPassword; set { m_NewPassword = value; OnPropertyChanged(); } }
        public string ConfirmPassword { get => m_ConfirmPassword; set { m_ConfirmPassword = value; OnPropertyChanged(); } }
        public string Status { get => m_Status; set { m_Status = value; OnPropertyChanged(); } }

        // Command to request reset code.
        public RelayCommand SendCodeCommand { get; }

        // Command to actually reset the password.
        public RelayCommand ResetPasswordCommand { get; }

        // Navigates back to login view.
        public RelayCommand BackToLoginCommand { get; }

        public ForgotPasswordViewModel(INetworkFacade networkFacade)
        {
            m_Model = new ForgotPasswordModel(networkFacade);

            SendCodeCommand = new RelayCommand(async _ => await SendCodeAsync());
            ResetPasswordCommand = new RelayCommand(async _ => await ResetPasswordAsync());
            BackToLoginCommand = new RelayCommand(_ => OnTriggerOccurred?.Invoke(ApplicationTrigger.LogInRequested));
        }

        // Sends the reset code via email using the model.
        private async Task SendCodeAsync()
        {
            Status = "Sending verification code...";
            await m_Model.SendResetCodeAsync(Email);
            Status = "Code sent. Please check your email.";
        }

        // Verifies the code and resets the password.
        private async Task ResetPasswordAsync()
        {
            if (NewPassword != ConfirmPassword)
            {
                Status = "Passwords do not match.";
                return;
            }

            bool success = await m_Model.VerifyAndResetPasswordAsync(Email, Code, NewPassword);
            Status = success ? "Password reset successful!" : "Failed to reset password.";

            if (success)
            {
                OnTriggerOccurred?.Invoke(ApplicationTrigger.PasswordResetSuccessful);
            }
        }
    }
}
