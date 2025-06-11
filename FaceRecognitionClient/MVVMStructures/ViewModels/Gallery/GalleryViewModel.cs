using FaceRecognitionClient.Commands;
using FaceRecognitionClient.InternalDataModels;
using FaceRecognitionClient.MVVMStructures.Models.Gallery;
using FaceRecognitionClient.Services.GalleryService;
using FaceRecognitionClient.StateMachine;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FaceRecognitionClient.MVVMStructures.ViewModels.Gallery
{
    //TODO : strange architecture with the GalleryImageViewModel. why not move the IDetailNotifier into the GalleryImageViewModel
    public class GalleryViewModel : BaseViewModel, IStateNotifier, IDetailNotifier<AdvancedPersonDataWithImage>
    {
        private GalleryModel m_GalleryModel;

        public ObservableCollection<GalleryImageViewModel> Persons { get; } = new ObservableCollection<GalleryImageViewModel>();
        public ICommand RefreshCommand { get; }
        public ICommand BackCommand { get; }

        // Events to trigger navigation or updates
        public event Action<ApplicationTrigger> OnTriggerOccurred;
        public event Action<bool>? OnImageSelectionChanged;
        public event Action<AdvancedPersonDataWithImage> OnDetailRequested;

        public GalleryViewModel(IGalleryService galleryService, GalleryImage image, UserSession userSession)
        {
            m_GalleryModel = new GalleryModel(galleryService, userSession);

            // Command to reload gallery images
            RefreshCommand = new AsyncRelayCommand(_ => LoadImagesAsync());

            // Navigate back to the main navigation window
            BackCommand = new RelayCommand(_ => OnTriggerOccurred?.Invoke(ApplicationTrigger.NavigationRequested));

        }

        // Loads all saved gallery images and populates the UI list
        public async Task LoadImagesAsync()
        {
            Persons.Clear();
            var allImages = await m_GalleryModel.LoadGalleryAsync();

            foreach (var image in allImages)
            {
                Persons.Add(new GalleryImageViewModel(image.Person, OpenPersonDetails));
            }
        }

        private void OpenPersonDetails(AdvancedPersonDataWithImage person) => OnDetailRequested?.Invoke(person);
    }
}

