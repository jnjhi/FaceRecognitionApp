namespace FaceRecognitionClient.StateMachine
{
    /// <summary>
    /// Represents the events that can trigger a transition from one screen to another.
    /// These are used by the state machine to decide which state to move to next.
    /// </summary>
    public enum ApplicationTrigger
    {
        SignUpRequested,
        SignUpSuccessful,
        SignUpFailed,
        LogInRequested,
        LoginSuccessful,
        LogInFailed,
        DeveloperEntered,
        CameraCaptureRequested,
        PhotoAccepted,
        ShowPersonDetails,
        CaptchaPassed,
        EmailVerified,
        FaceRecognitionRequested,
        GalleryRequested,
        ForgotPasswordRequested,
        PasswordResetSuccessful,
        AttendanceRequested,
        UserDisconnected,
        /// <summary>
        /// User has requested to log out of the application.
        /// </summary>
        LogOutRequested
    }
}
