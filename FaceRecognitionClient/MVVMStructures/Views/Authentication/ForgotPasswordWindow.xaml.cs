using FaceRecognitionClient.MVVMStructures.ViewModels.Authentication;
using System.Windows;
using System.Windows.Input;


namespace FaceRecognitionClient.Views.Authentication
{
    public partial class ForgotPasswordWindow : Window
    {
        public ForgotPasswordWindow()
        {
            InitializeComponent();
        }

        private void OnNewPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ForgotPasswordViewModel vm)
            {
                vm.NewPassword = NewPasswordBox.Password;
            }
        }

        private void OnConfirmPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ForgotPasswordViewModel vm)
            {
                vm.ConfirmPassword = ConfirmPasswordBox.Password;
            }
        }

        private void TogglePasswordVisibility_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VisiblePasswordBox.Text = NewPasswordBox.Password;
            VisiblePasswordBox.Visibility = Visibility.Visible;
            NewPasswordBox.Visibility = Visibility.Collapsed;

            if (DataContext is ForgotPasswordViewModel vm)
            {
                vm.NewPassword = VisiblePasswordBox.Text;
            }
        }

        private void TogglePasswordVisibility_MouseUp(object sender, MouseEventArgs e)
        {
            NewPasswordBox.Password = VisiblePasswordBox.Text;
            VisiblePasswordBox.Visibility = Visibility.Collapsed;
            NewPasswordBox.Visibility = Visibility.Visible;

            if (DataContext is ForgotPasswordViewModel vm)
            {
                vm.NewPassword = NewPasswordBox.Password;
            }
        }

        private void ToggleConfirmVisibility_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VisibleConfirmPasswordBox.Text = ConfirmPasswordBox.Password;
            VisibleConfirmPasswordBox.Visibility = Visibility.Visible;
            ConfirmPasswordBox.Visibility = Visibility.Collapsed;

            if (DataContext is ForgotPasswordViewModel vm)
            {
                vm.ConfirmPassword = VisibleConfirmPasswordBox.Text;
            }
        }

        private void ToggleConfirmVisibility_MouseUp(object sender, MouseEventArgs e)
        {
            ConfirmPasswordBox.Password = VisibleConfirmPasswordBox.Text;
            VisibleConfirmPasswordBox.Visibility = Visibility.Collapsed;
            ConfirmPasswordBox.Visibility = Visibility.Visible;

            if (DataContext is ForgotPasswordViewModel vm)
            {
                vm.ConfirmPassword = ConfirmPasswordBox.Password;
            }
        }
    }
}
