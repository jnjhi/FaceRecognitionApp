/*
    This is a generic finite state machine engine that drives the navigation logic of the application.
    It manages transitions between views (windows) like login, sign-up, and gallery based on defined triggers.

    For example:
    - From LogInWindow, if LoginSuccessful is triggered, the machine transitions to CaptchaWindow.
    - From GalleryWindow, triggering FaceRecognitionRequested brings the user back to FaceRecognitionWindow.

    The class also supports entry/exit actions (e.g., showing or hiding windows),
    and internal transitions (e.g., opening a person details pop-up without leaving the current window).
*/

using System;
using System.Collections.Generic;

namespace FaceRecognitionClient.StateMachine
{
    public class StateMachine<TState, TTrigger> : IStateMachine<TState, TTrigger>
    {
        public event Action<TState> OnStateChanged;

        // Dictionary mapping current state → trigger → next state
        private readonly Dictionary<TState, Dictionary<TTrigger, TState>> _transitions = new();

        // Optional actions to perform when entering or exiting states
        private readonly Dictionary<TState, Action> _stateEntryActions = new();
        private readonly Dictionary<TState, Action> _stateExitActions = new();

        // Optional transition actions (executed between exit and entry)
        private readonly Dictionary<(TState FromState, TTrigger Trigger), Action> _transitionActions = new();

        // Internal (self-state) actions that don’t change state
        private readonly Dictionary<(TState State, TTrigger Trigger), Action> _internalActions = new();

        private TState _currentState;

        public StateMachine(TState initialState)
        {
            _currentState = initialState;
        }

        public TState CurrentState => _currentState;

        /// <summary>
        /// Adds a state transition from one state to another based on a trigger.
        /// Optionally, a custom action can be executed during the transition.
        /// </summary>
        public void AddTransition(TState fromState, TTrigger trigger, TState toState, Action? transitionAction = null)
        {
            if (!_transitions.ContainsKey(fromState))
            {
                _transitions[fromState] = new Dictionary<TTrigger, TState>();
            }

            _transitions[fromState][trigger] = toState;

            if (transitionAction != null)
            {
                _transitionActions[(fromState, trigger)] = transitionAction;
            }
        }

        /// <summary>
        /// Adds an internal transition that performs an action without changing states.
        /// Useful for in-place UI events like opening a modal window.
        /// </summary>
        public void AddInternalTransition(TState state, TTrigger trigger, Action action = null)
        {
            if (action == null)
                return;

            _internalActions[(state, trigger)] = action;
        }

        public void AddStateEntryAction(TState state, Action action)
        {
            _stateEntryActions[state] = action;
        }

        public void AddStateExitAction(TState state, Action action)
        {
            _stateExitActions[state] = action;
        }

        /// <summary>
        /// Fires a trigger — may cause a state change or internal action to occur.
        /// </summary>
        public void Fire(TTrigger trigger)
        {
            // Check if an internal (non-state-changing) action exists
            if (_internalActions.TryGetValue((_currentState, trigger), out var internalAction))
            {
                internalAction();
                return;
            }

            // Check if there is a valid state transition from the current state using this trigger
            if (!_transitions.TryGetValue(_currentState, out var triggerMap) ||
                !triggerMap.TryGetValue(trigger, out var newState))
            {
                ClientLogger.ClientLogger.LogWarning($"No transition defined for trigger '{trigger}' from state '{_currentState}'.");
                throw new InvalidOperationException($"No transition defined for trigger '{trigger}' from state '{_currentState}'.");
            }

            bool isSelfTransition = EqualityComparer<TState>.Default.Equals(_currentState, newState);

            // Run the transition-specific action if defined
            _transitionActions.TryGetValue((_currentState, trigger), out var transitionAction);

            // Execute exit action if transitioning to a new state
            if (!isSelfTransition && _stateExitActions.TryGetValue(_currentState, out var exitAction))
            {
                exitAction();
            }

            transitionAction?.Invoke();

            // Change current state
            _currentState = newState;
            OnStateChanged?.Invoke(_currentState);

            // Execute entry action if entering a new state
            if (!isSelfTransition && _stateEntryActions.TryGetValue(_currentState, out var entryAction))
            {
                entryAction();
            }
        }
    }
}
