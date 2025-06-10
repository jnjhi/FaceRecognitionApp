using FaceRecognitionClient.Commands;
using FaceRecognitionClient.StateMachine;

namespace FaceRecognitionClient.MVVMStructures.ViewModels.Disconnected
{
    /// <summary>
    /// View model displayed when the server disconnects the user.
    /// Shows the error message and allows returning to the log in screen.
    /// </summary>
    public class DisconnectedViewModel : BaseViewModel, IStateNotifier
    {
        public event Action<ApplicationTrigger> OnTriggerOccurred;

        private string m_ErrorMessage = string.Empty;

        /// <summary>
        /// Message provided by the server explaining why the user was disconnected.
        /// </summary>
        public string ErrorMessage
        {
            get => m_ErrorMessage;
            set { m_ErrorMessage = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Command that navigates back to the log in window.
        /// </summary>
        public RelayCommand LogInAgainCommand { get; }

        public DisconnectedViewModel()
        {
            LogInAgainCommand = new RelayCommand(_ => OnTriggerOccurred?.Invoke(ApplicationTrigger.LogInRequested));
        }
    }
}
