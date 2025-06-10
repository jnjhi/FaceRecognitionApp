using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace FaceRecognitionClient.MVVMStructures.ViewModels
{
    public class ImageDebugViewModel : BaseViewModel
    {
        private BitmapImage _debugImage;

        /// <summary>
        /// The image to display in the UI, converted to BitmapImage.
        /// </summary>
        public BitmapImage DebugImage
        {
            get => _debugImage;
            private set
            {
                _debugImage = value;
                OnPropertyChanged();
            }
        }

        public ImageDebugViewModel(BitmapImage bitmap)
        {
            DebugImage = bitmap;
        }
    }
}
