using FaceRecognitionClient.MVVMStructures.ViewModels;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FaceRecognitionClient.MVVMStructures.Views
{
    /// <summary>
    /// Interaction logic for DebugView.xaml
    /// </summary>
    public partial class DebugView : Window
    {
        public DebugView(BitmapImage bitmap)
        {
            InitializeComponent();
            DataContext = new ImageDebugViewModel(bitmap);
        }
    }
}
