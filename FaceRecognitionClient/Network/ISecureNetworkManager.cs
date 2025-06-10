namespace FaceRecognitionClient.Network
{
    /// <summary>
    /// Interface for managing secure communication over the network.
    /// Wraps encryption (RSA, AES) around the raw INetworkManager interface.
    /// </summary>
    internal interface ISecureNetworkManager
    {
        /// <summary>
        /// Triggered when a decrypted message is received from the server.
        /// </summary>
        event Action<string> OnMessageReceive;

        /// <summary>
        /// Connects to the server and initiates secure key exchange.
        /// </summary>
        void Connect();

        /// <summary>
        /// Encrypts and sends a message to the server.
        /// </summary>
        void SendMessage(string message);

        /// <summary>
        /// Disconnects from the server and cleans up encryption handlers.
        /// </summary>
        void Disconnect();
    }
}
