using FaceRecognitionClient.Commands;
using FaceRecognitionClient.MVVMStructures.ViewModels;
using FaceRecognitionClient.StateMachine;
using FaceRecognitionClient.Utils;
using FaceRecognitionClient.Views;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace FaceRecognitionClient.MVVMStructures.ViewModels.FaceRecognition
{
    // This ViewModel controls the behavior of the camera capture screen.
    // It manages the live camera feed, allows users to take a photo, and accept or reject it.
    // On acceptance, the photo is passed through a shared image store to the recognition screen.
    internal class CameraCaptureViewModel : BaseViewModel, IStateNotifier
    {
        private readonly SharedImageStore _sharedImageStore;

        private VideoCapture m_Camera;
        private CancellationTokenSource m_CancellationTokenSource;
        private WriteableBitmap m_CameraFrame;
        private bool m_isCameraRunning;
        private bool m_IsPhotoTaken;

        public event Action<ApplicationTrigger> OnTriggerOccurred;

        // This is the frame shown in the UI. It's updated every ~30ms from the webcam.
        public WriteableBitmap CameraFrame
        {
            get => m_CameraFrame;
            set { m_CameraFrame = value; OnPropertyChanged(); }
        }

        // Whether a photo was taken and accepted/rejected.
        public bool IsPhotoTaken
        {
            get => m_IsPhotoTaken;
            set { m_IsPhotoTaken = value; OnPropertyChanged(); }
        }

        // Starts the camera feed (disabled when already running).
        public AsyncRelayCommand StartCameraCommand => new AsyncRelayCommand(execute => StartCameraAsync(), (_) => !m_isCameraRunning);

        // Captures a still photo and stops the camera preview.
        public AsyncRelayCommand TakePhotoCommand => new AsyncRelayCommand(execute => TakePhotoAsync(), (_) => m_isCameraRunning);

        // Stops the camera and releases resources.
        public AsyncRelayCommand StopCameraCommand => new AsyncRelayCommand(execute => StopCameraAsync(), (_) => m_isCameraRunning);

        // Accepts the captured photo and sends it to the shared store for recognition.
        public RelayCommand AcceptPhotoCommand => new RelayCommand(execute => AcceptPhoto());

        // Rejects the captured photo and restarts the camera.
        public AsyncRelayCommand RejectPhotoCommand => new AsyncRelayCommand(execute => RejectPhotoAsync());

        // Goes back to the face recognition window (without capturing a photo).
        public RelayCommand GoBackCommand => new RelayCommand(_ => GoBackToFaceRecognitionWindow());

        public CameraCaptureViewModel(SharedImageStore sharedImageStore)
        {
            _sharedImageStore = sharedImageStore;
        }

        // Immediately releases the camera hardware (called on window close or cancel).
        public void StopCamera()
        {
            if (m_Camera != null)
            {
                m_Camera.Release(); // Stop video capture
                m_Camera.Dispose(); // Free unmanaged resources
                m_Camera = null;
            }
        }

        // Starts the live preview from the default camera.
        // A background task reads camera frames and converts them to WriteableBitmap for UI binding.
        private async Task StartCameraAsync()
        {
            m_Camera = new VideoCapture(0, VideoCaptureAPIs.DSHOW); // DSHOW is the Windows driver interface
            if (!m_Camera.IsOpened()) return;

            m_isCameraRunning = true;
            m_CancellationTokenSource = new CancellationTokenSource();

            await Task.Run(() =>
            {
                var mat = new Mat(); // Matrix to hold raw camera frame
                while (!m_CancellationTokenSource.Token.IsCancellationRequested)
                {
                    m_Camera.Read(mat); // Read next frame from camera

                    if (!mat.Empty()) // Frame is valid
                    {
                        var image = mat.ToWriteableBitmap(); // Convert to WPF-compatible image
                        image.Freeze(); // Allow cross-thread usage
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            CameraFrame = image; // Update bound property on UI thread
                        });
                    }
                }
                mat.Dispose(); // Cleanup the matrix once cancelled
            });
        }

        // Captures a photo and stops the live feed.
        private async Task TakePhotoAsync()
        {
            await StopCameraAsync();  // Stop preview
            IsPhotoTaken = true;      // Mark that a photo has been taken
        }

        // Transitions back to face recognition screen (without photo).
        private void GoBackToFaceRecognitionWindow()
        {
            OnTriggerOccurred?.Invoke(ApplicationTrigger.FaceRecognitionRequested);
        }

        // Accepts the currently shown frame as a photo.
        // Converts it into a BitmapImage and stores it in the shared store for recognition use.
        private void AcceptPhoto()
        {
            _sharedImageStore.CapturedImage = ImageProcessingUtils.ConvertWriteableBitmapToBitmapImage(CameraFrame);
            OnTriggerOccurred?.Invoke(ApplicationTrigger.PhotoAccepted);
        }

        // If the user rejects the photo, we resume the live preview to let them retake.
        private async Task RejectPhotoAsync()
        {
            IsPhotoTaken = false;
            await StartCameraAsync();
        }

        // Stops the camera safely and cleans up resources.
        private async Task StopCameraAsync()
        {
            m_CancellationTokenSource?.Cancel(); // Ask the capture loop to stop
            await Task.Delay(100);               // Give it time to terminate
            m_Camera?.Release();                 // Release camera hardware
            m_isCameraRunning = false;
        }
    }
}
