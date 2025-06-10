/// <summary>
/// A generic interface for notifying when the user wants to view the details of a specific item.
/// </summary>
/// <typeparam name="T">The type of the detail object (e.g., a person or a face)</typeparam>
public interface IDetailNotifier<T>
{
    /// <summary>
    /// Raised when the user requests to see more information about a specific item.
    /// </summary>
    event Action<T> OnDetailRequested;
}
