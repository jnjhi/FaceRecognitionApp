namespace FaceRecognitionClient
{
    /// <summary>
    /// Stores information about the currently logged-in user.
    /// This object is used throughout the client application to carry user identity and session context.
    /// </summary>
    public class UserSession
    {
        /// <summary>
        /// The unique database ID assigned to the user after logging in or signing up.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The user's email address. Used for display and communication features (e.g., forgot password).
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The username that the user logs in with (not necessarily their email).
        /// </summary>
        public string UserName { get; set; }
    }
}
