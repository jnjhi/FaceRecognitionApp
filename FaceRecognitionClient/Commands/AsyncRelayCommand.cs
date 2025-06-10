using System.Windows.Input;

namespace FaceRecognitionClient.Commands
{
    // AsyncRelayCommand is a command class used when the action to perform is asynchronous (uses async/await).
    // It ensures UI buttons are properly disabled during execution and prevents double clicks or race conditions.
    public class AsyncRelayCommand : ICommand
    {
        private Func<object, Task> executeAsync;    // The asynchronous method to execute
        private Func<object, bool> canExecute;      // Optional logic to control button enable/disable
        private bool isExecuting;                   // Indicates if the command is currently running

        // WPF uses this to update command availability (e.g., disable while task is running)
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        // Constructor accepts an async method to execute and optionally a CanExecute condition
        public AsyncRelayCommand(Func<object, Task> executeAsync, Func<object, bool> canExecute = null)
        {
            this.executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            this.canExecute = canExecute;
        }

        // Only allow execution if we’re not already running and CanExecute is true (if defined)
        public bool CanExecute(object parameter)
        {
            return !isExecuting && (canExecute == null || canExecute(parameter));
        }

        // Asynchronous execution logic
        public async void Execute(object parameter)
        {
            isExecuting = true;               // Prevent re-entry
            RaiseCanExecuteChanged();         // Notify UI to disable button

            try
            {
                await executeAsync(parameter); // Run the actual task
            }
            finally
            {
                isExecuting = false;          // Mark complete
                RaiseCanExecuteChanged();     // Notify UI to re-enable
            }
        }

        // Forces WPF to re-evaluate CanExecute (used when state changes, e.g., form is valid)
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
