using System.Net.Sockets;

// Represents an individual TCP client connection on the server.
// Implements the IChatClient interface for sending and receiving messages.
internal class ChatClient : IChatClient
{
    // Triggered when a message is received from the client
    public event Action<string, string> OnReceiveDataFromTheClient;

    // Triggered when the client should be removed from the system
    public event Action<IChatClient, string> OnRemove;

    private TcpClient _client;
    private string _clientIP;
    private byte[] data;

    // Exposes the client's IP address as a string
    public string GetClientIP => _clientIP;

    // Initializes the client, starts listening for messages
    public ChatClient(TcpClient client)
    {
        _client = client;
        _clientIP = client.Client.RemoteEndPoint.ToString();
        data = new byte[_client.ReceiveBufferSize];

        // Begin async read from the network stream
        _client.GetStream().BeginRead(data, 0, Convert.ToInt32(_client.ReceiveBufferSize), ReceiveMessage, null);
    }

    // Sends a string message to the client
    public void SendMessage(string message)
    {
        try
        {
            NetworkStream ns;
            lock (_client.GetStream())
            {
                ns = _client.GetStream();
            }

            byte[] bytesToSend = System.Text.Encoding.ASCII.GetBytes(message);
            ns.Write(bytesToSend, 0, bytesToSend.Length);
            ns.Flush();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    // Callback for when a message is received from the client
    private void ReceiveMessage(IAsyncResult ar)
    {
        int bytesRead;
        try
        {
            lock (_client.GetStream())
            {
                bytesRead = _client.GetStream().EndRead(ar);
            }

            // If connection is closed, remove the client
            if (bytesRead < 1)
            {
                OnRemove.Invoke(this, _clientIP);
                return;
            }

            // Convert bytes to string and invoke the event
            string messageReceived = System.Text.Encoding.ASCII.GetString(data, 0, bytesRead);
            OnReceiveDataFromTheClient?.Invoke(messageReceived, _clientIP);

            // Continue listening for messages
            lock (_client.GetStream())
            {
                _client.GetStream().BeginRead(data, 0, Convert.ToInt32(_client.ReceiveBufferSize), ReceiveMessage, null);
            }
        }
        catch (Exception ex)
        {
            // Handle socket error silently (e.g., disconnect)
        }
    }
}
