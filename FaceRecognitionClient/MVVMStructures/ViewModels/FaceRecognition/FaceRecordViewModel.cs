using FaceRecognitionClient.Commands;
using FaceRecognitionClient.InternalDataModels;

namespace FaceRecognitionClient.MVVMStructures.ViewModels.FaceRecognition
{
    // This ViewModel represents a single recognized person in the face recognition result list.
    // It is used to display a summary card (e.g., name, profile image) and supports opening full details.
    public class FaceRecordViewModel : BaseViewModel
    {
        // The full recognition result for this face, including image, name, ID, etc.
        public AdvancedPersonDataWithImage FaceRecord { get; }

        // A command that will be bound to the UI to allow the user to open the details screen.
        public RelayCommand OpenDetailsCommand { get; }

        // A callback function provided by the parent ViewModel (FaceRecognitionViewModel),
        // which will be called when the user requests to view this person's full profile.
        private readonly Action<AdvancedPersonDataWithImage> m_RequestDetails;

        public FaceRecordViewModel(AdvancedPersonDataWithImage faceRecord, Action<AdvancedPersonDataWithImage> requestDetailsCallback)
        {
            FaceRecord = faceRecord;
            m_RequestDetails = requestDetailsCallback;

            // This command is bound to the UI (e.g., a button or double-click)
            // When triggered, it passes the person’s record to the callback to show details.
            OpenDetailsCommand = new RelayCommand(_ => m_RequestDetails?.Invoke(FaceRecord));
        }
    }
}
