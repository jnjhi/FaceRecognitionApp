using FaceRecognitionClient.Commands;
using FaceRecognitionClient.StateMachine;

namespace FaceRecognitionClient.MVVMStructures.ViewModels.NavigationWindow
{
    /// <summary>
    /// View model for the navigation window displayed after authentication.
    /// Provides commands to switch to the major application screens.
    /// </summary>
    public class NavigationWindowViewModel : BaseViewModel, IStateNotifier
    {
        public event Action<ApplicationTrigger> OnTriggerOccurred;

        public RelayCommand OpenFaceRecognitionCommand { get; }
        public RelayCommand OpenGalleryCommand { get; }
        public RelayCommand OpenAttendanceCommand { get; }

        public NavigationWindowViewModel()
        {
            OpenFaceRecognitionCommand = new RelayCommand(_ =>
                OnTriggerOccurred?.Invoke(ApplicationTrigger.FaceRecognitionRequested));
            OpenGalleryCommand = new RelayCommand(_ =>
                OnTriggerOccurred?.Invoke(ApplicationTrigger.GalleryRequested));
            OpenAttendanceCommand = new RelayCommand(_ =>
                OnTriggerOccurred?.Invoke(ApplicationTrigger.AttendanceRequested));
        }
    }
}
