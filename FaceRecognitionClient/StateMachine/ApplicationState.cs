namespace FaceRecognitionClient.StateMachine
{
    /// <summary>
    /// Represents all the possible "screens" or "windows" in the application.
    /// This enum is used as the state in the finite state machine.
    /// </summary>
    public enum ApplicationState
    {
        LogInWindow,
        SignUpWindow,
        CaptchaWindow,
        FaceRecognitionWindow,
        CameraCaptureWindow,
        EmailVerificationWindow,
        GalleryWindow,
        ForgotPasswordWindow,
        AttendanceWindow
    }
}
