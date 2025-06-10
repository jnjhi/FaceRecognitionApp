// Represents the contract for a connected client that supports receiving and sending messages over TCP.
// Used by the NetworkManager to manage individual clients.
internal interface IChatClient
{
    // Triggered when a message is received from the client (message, client IP)
    event Action<string, string> OnReceiveDataFromTheClient;

    // Triggered when the client is disconnected or removed (client reference, IP)
    event Action<IChatClient, string> OnRemove;

    // Provides the IP address of the connected client
    string GetClientIP { get; }

    // Sends a message string to the connected client
    void SendMessage(string message);
}
