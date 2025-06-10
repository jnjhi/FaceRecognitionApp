using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FaceRecognitionClient.MVVMStructures.ViewModels.Authentication;

namespace FaceRecognitionClient.Views.Authentication
{
    public partial class LogInWindow : Window
    {
        public LogInWindow()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LogInViewModel vm)
            {
                vm.Password = PasswordBox.Password;
            }
        }

        private void TogglePasswordVisibility_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VisiblePasswordBox.Text = PasswordBox.Password;
            VisiblePasswordBox.Visibility = Visibility.Visible;
            PasswordBox.Visibility = Visibility.Collapsed;

            if (DataContext is LogInViewModel vm)
            {
                vm.Password = VisiblePasswordBox.Text;
            }
        }

        private void TogglePasswordVisibility_MouseUp(object sender, MouseEventArgs e)
        {
            PasswordBox.Password = VisiblePasswordBox.Text;
            VisiblePasswordBox.Visibility = Visibility.Collapsed;
            PasswordBox.Visibility = Visibility.Visible;

            if (DataContext is LogInViewModel vm)
            {
                vm.Password = PasswordBox.Password;
            }
        }
    }
}
