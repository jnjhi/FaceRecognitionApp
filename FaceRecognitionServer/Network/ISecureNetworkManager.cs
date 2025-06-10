// Interface for a secure network manager that wraps encryption over the base network communication layer.
// Adds encryption-related behavior to a standard network manager.
public interface ISecureNetworkManager
{
    // Triggered when a decrypted message is received from a client (message, IP)
    event Action<string, string> OnMessageReceive;

    // Triggered when a new client is successfully added (IP)
    event Action<string> OnClientAdd;

    // Initializes the underlying network layer and prepares for encrypted communication
    void Connect();

    // Sends an encrypted message to all connected clients
    void Broadcast(string message);

    // Sends an encrypted message to a specific client
    void SendMessage(string message, string clientIp);

    // Removes all known clients and associated encryption state
    void RemoveAllTheClients();

    // Stops the server and closes all active connections
    void Disconnect();
}
