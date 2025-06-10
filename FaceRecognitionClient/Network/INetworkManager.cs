namespace FaceRecognitionClient.Network
{
    /// <summary>
    /// Defines the basic contract for unencrypted TCP communication.
    /// Used internally by SecureNetworkManager to send and receive raw messages over sockets.
    /// </summary>
    internal interface INetworkManager
    {
        /// <summary>
        /// Triggered whenever a message is received from the server.
        /// </summary>
        event Action<string> OnMessageReceive;

        /// <summary>
        /// Establishes a TCP connection to the server.
        /// </summary>
        void Connect();

        /// <summary>
        /// Sends a plain (unencrypted) string message to the server.
        /// </summary>
        void SendMessage(string message);

        /// <summary>
        /// Closes the TCP connection to the server.
        /// </summary>
        void Disconnect();
    }
}
