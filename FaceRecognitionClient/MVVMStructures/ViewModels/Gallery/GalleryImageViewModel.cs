using FaceRecognitionClient.Commands;
using FaceRecognitionClient.InternalDataModels;
using System.Windows.Input;

namespace FaceRecognitionClient.MVVMStructures.ViewModels.Gallery
{
    public class GalleryImageViewModel : BaseViewModel
    {
        public AdvancedPersonDataWithImage PersonData { get; }
        public ICommand OpenDetailsCommand { get; }

        private readonly Action<AdvancedPersonDataWithImage> m_RequestDetails;

        public GalleryImageViewModel(AdvancedPersonDataWithImage personData, Action<AdvancedPersonDataWithImage> requestDetailsCallback)
        {
            PersonData = personData;
            m_RequestDetails = requestDetailsCallback;

            OpenDetailsCommand = new RelayCommand(execute => m_RequestDetails?.Invoke(PersonData));
        }
    }
}
