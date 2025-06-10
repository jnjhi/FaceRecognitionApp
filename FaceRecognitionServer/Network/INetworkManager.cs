// Defines the contract for a basic TCP network manager.
// Manages client connections, message routing, and broadcasts.
internal interface INetworkManager
{
    // Triggered when a new message is received from any client (message, client IP)
    event Action<string, string> OnMessageReceive;

    // Triggered when a client disconnects (client IP)
    event Action<string> OnClientRemove;

    // Triggered when a new client connects (client IP)
    event Action<string> OnClientAdd;

    // Starts the server and begins listening for incoming clients
    void Connect();

    // Sends a message to all connected clients
    void Broadcast(string message);

    // Sends a message to a specific client by IP
    void SendMessage(string message, string clientIp);

    // Disconnects a client with the specified IP
    void DisconnectClient(string clientIp);

    // Clears the current client list (e.g., on shutdown or reset)
    void RemoveAllTheClients();

    // Stops the listener and disconnects all clients
    void Disconnect();
}
