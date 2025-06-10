using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FaceRecognitionClient.MVVMStructures.ViewModels
{
    // This is the base class for all ViewModels in the MVVM architecture.
    // It provides a shared implementation of INotifyPropertyChanged so that
    // any subclass can notify the view (UI) when one of its properties changes.

    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        // This event is triggered whenever a property value changes.
        // It allows data-bound UI elements (like TextBox, Label, ComboBox, etc.)
        // to update automatically when the ViewModel changes.
        public event PropertyChangedEventHandler? PropertyChanged;

        // This method is called by ViewModel properties when their values are updated.
        // It raises the PropertyChanged event to tell the UI to refresh the corresponding binding.
        // The [CallerMemberName] attribute automatically fills in the name of the property that called this method.
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // If there are any listeners (e.g., the WPF binding system), notify them.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

