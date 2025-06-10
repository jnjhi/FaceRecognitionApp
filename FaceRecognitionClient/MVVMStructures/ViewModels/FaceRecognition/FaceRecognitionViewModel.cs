using FaceRecognitionClient.Commands;
using FaceRecognitionClient.InternalDataModels;
using FaceRecognitionClient.MVVMStructures.Models.FaceRecognition;
using FaceRecognitionClient.Services.GalleryService;
using FaceRecognitionClient.StateMachine;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace FaceRecognitionClient.MVVMStructures.ViewModels.FaceRecognition
{
    internal class FaceRecognitionViewModel : BaseViewModel, IStateNotifier, IDetailNotifier<AdvancedPersonDataWithImage>
    {
        private readonly SharedImageStore m_SharedImageStore;
        private FaceRecognitionModel m_FaceRecognitionModel;
        private BitmapImage m_Image;

        public event Action<ApplicationTrigger> OnTriggerOccurred;

        public event Action<AdvancedPersonDataWithImage> OnDetailRequested;

        public AsyncRelayCommand UploadImageCommand { get; }

        public RelayCommand OpenCameraViewCommand { get; }

        public RelayCommand OpenAttendanceCommand { get; }

        public AsyncRelayCommand SendImageCommand { get; }

        public RelayCommand OpenGalleryCommand { get; }

        public ObservableCollection<FaceRecordViewModel> RecognizedPersons { get; } = new();

        // The image currently loaded (from disk or camera)
        public BitmapImage ImageSource
        {
            get => m_Image;
            set
            {
                m_Image = value;
                OnPropertyChanged();
            }
        }

        public FaceRecognitionViewModel(INetworkFacade networkFacade, IGalleryService galleryService, SharedImageStore sharedImageStore, Mapper mapper)
        {
            m_FaceRecognitionModel = new FaceRecognitionModel(networkFacade, galleryService, mapper);
            m_SharedImageStore = sharedImageStore;

            // Bind UI buttons to actions
            UploadImageCommand = new AsyncRelayCommand(_ => OnUploadImage());
            OpenCameraViewCommand = new RelayCommand(_ => ChangeToCameraCaptureView());
            SendImageCommand = new AsyncRelayCommand(_ => OnSendImageForRecognitionAsync(), _ => ImageSource != null);
            OpenGalleryCommand = new RelayCommand(_ => OnTriggerOccurred?.Invoke(ApplicationTrigger.GalleryRequested));
            OpenAttendanceCommand = new RelayCommand(_ => OnTriggerOccurred?.Invoke(ApplicationTrigger.AttendanceRequested));
        }

        // Loads the photo that was taken by the camera into this view
        public void LoadCapturedImage()
        {
            ImageSource = m_SharedImageStore.CapturedImage;
        }

        // Handles image file upload from disk
        // Shows file dialog, loads selected image, and enables the "Send" button
        private async Task OnUploadImage()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg"
            };

            if (dialog.ShowDialog() == true)
            {
                BitmapImage bitmap = await LoadBitmapImageAsync(dialog.FileName);
                ImageSource = bitmap;

                // Enable "Send" command now that an image is loaded
                SendImageCommand.RaiseCanExecuteChanged();
            }
        }

        // Sends the image to the server for face recognition and displays results
        private async Task OnSendImageForRecognitionAsync()
        {
            if (ImageSource == null)
                return;

            // Send image and receive annotated image + recognition results
            var displayData = await m_FaceRecognitionModel.RecognizeAsync(ImageSource);

            // Show the image returned from server (with face boxes drawn)
            ImageSource = displayData.AnnotatedImage;

            // Clear previous results and display new ones
            RecognizedPersons.Clear();

            foreach (var result in displayData.RecognitionData)
            {
                var viewModel = new FaceRecordViewModel(result, OpenPersonsDetails);
                RecognizedPersons.Add(viewModel);
            }
        }

        // Called when a user clicks on a person card to open their detailed info
        private void OpenPersonsDetails(AdvancedPersonDataWithImage faceRecord) => OnDetailRequested?.Invoke(faceRecord);


        // Loads a BitmapImage from file path (used after upload)
        private Task<BitmapImage> LoadBitmapImageAsync(string filePath)
        {
            return Task.Run(() =>
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(filePath);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze(); // Allow cross-thread access
                return image;
            });
        }

        // Triggers transition to the camera capture screen
        private void ChangeToCameraCaptureView()
        {
            OnTriggerOccurred?.Invoke(ApplicationTrigger.CameraCaptureRequested);
        }
    }
}
