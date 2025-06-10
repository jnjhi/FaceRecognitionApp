namespace FaceRecognitionClient.StateMachine
{
    /// <summary>
    /// Implemented by ViewModels that can trigger navigation events.
    /// Allows the view model to push triggers to the state machine.
    /// </summary>
    public interface IStateNotifier
    {
        event Action<ApplicationTrigger> OnTriggerOccurred;
    }
}
