using FaceRecognitionClient.MVVMStructures.ViewModels.Gallery;
using System.Windows;
using System.Windows.Media.Animation;

namespace FaceRecognitionClient.Views
{
    /// <summary>
    /// Interaction logic for GalleryWindow.xaml
    /// </summary>
    public partial class GalleryWindow : Window
    {
        public GalleryWindow()
        {
            InitializeComponent();

            // Safe cast and subscription
            if (DataContext is GalleryViewModel viewModel)
            {
                viewModel.OnImageSelectionChanged += HandleImageSelectionChanged;
            }
            else
            {
                // Delay binding until DataContext is fully ready (e.g., if set externally)
                DataContextChanged += OnDataContextChanged;
            }
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is GalleryViewModel viewModel)
            {
                viewModel.OnImageSelectionChanged += HandleImageSelectionChanged;
            }
        }

        private void HandleImageSelectionChanged(bool isImageSelected)
        {
            string storyboardKey = isImageSelected ? "ShrinkGalleryStoryboard" : "ExpandGalleryStoryboard";

            if (Resources[storyboardKey] is Storyboard storyboard)
            {
                Dispatcher.Invoke(() => storyboard.Begin());
            }
        }
    }

}
