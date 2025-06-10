namespace FaceRecognitionClient.StateMachine
{
    /// <summary>
    /// Represents a generic state machine that handles transitions between states based on triggers.
    /// </summary>
    public interface IStateMachine<TState, TTrigger>
    {
        event Action<TState> OnStateChanged;

        TState CurrentState { get; }

        void AddTransition(TState fromState, TTrigger trigger, TState toState, Action? transitionAction = null);
        void AddInternalTransition(TState state, TTrigger trigger, Action action);
        void AddStateEntryAction(TState state, Action action);
        void AddStateExitAction(TState state, Action action);

        /// <summary>
        /// Fires a trigger, causing the state machine to evaluate if a transition should occur.
        /// </summary>
        void Fire(TTrigger trigger);
    }
}
