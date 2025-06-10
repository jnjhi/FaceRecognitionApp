using System.Windows.Input;

namespace FaceRecognitionClient.Commands
{
    // RelayCommand is a reusable command implementation for binding buttons and UI actions to ViewModel methods.
    // It is used when the action is synchronous (i.e., does not involve async/await or background work).
    public class RelayCommand : ICommand
    {
        private Action<object> execute;             // The action to invoke when command is executed
        private Func<object, bool> canExecute;      // Optional: logic to enable/disable the command

        // Standard event required by ICommand interface.
        // Notifies WPF that the result of CanExecute() may have changed.
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        // Constructor takes the action to execute, and optionally a condition (canExecute) that determines if the command is enabled.
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        // Returns whether the command is allowed to execute (e.g., if form fields are valid)
        public bool CanExecute(object? parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        // Executes the command when triggered by the UI
        public void Execute(object? parameter)
        {
            execute(parameter);
        }
    }
}
