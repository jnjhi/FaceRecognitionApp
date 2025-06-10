namespace FaceRecognitionClient
{
    /// <summary>
    /// This interface is implemented by ViewModels that need to perform logic when a view becomes active.
    /// For example, loading data, refreshing UI, or starting animations when the window opens.
    /// </summary>
    public interface IStateActivatable
    {
        /// <summary>
        /// Called by the state machine when this view/window is entered (becomes the active UI).
        /// This allows the ViewModel to perform async initialization (e.g. fetching data from the server).
        /// </summary>
        Task OnActivatedAsync();
    }
}
 