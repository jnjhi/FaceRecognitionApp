using System.Net.Sockets;
using FaceRecognitionServer;

// Represents an individual TCP client connection on the server.
// Implements the IChatClient interface for sending and receiving messages.
internal class ChatClient : IChatClient
{
    private TcpClient _client;
    private string _clientIP;
    private byte[] _data;
    private bool _IsConnected;

    public event Action<string, string> OnReceiveDataFromTheClient;
    public event Action<IChatClient, string> OnRemove;

    public string GetClientIP => _clientIP;

    public ChatClient(TcpClient client)
    {
        _client = client;
        _clientIP = client.Client.RemoteEndPoint.ToString();
        _data = new byte[_client.ReceiveBufferSize];
        _IsConnected = true;

        _client.GetStream().BeginRead(_data, 0, Convert.ToInt32(_client.ReceiveBufferSize), ReceiveMessage, null);
    }

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
            Logger.LogException(ex, "Failed to send message to client");
        }
    }

    private void ReceiveMessage(IAsyncResult ar)
    {
        int bytesRead;

        if (!_IsConnected)
        {
            return;
        }

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

            string messageReceived = System.Text.Encoding.ASCII.GetString(_data, 0, bytesRead);
            OnReceiveDataFromTheClient?.Invoke(messageReceived, _clientIP);

            lock (_client.GetStream())
            {
                _client.GetStream().BeginRead(_data, 0, Convert.ToInt32(_client.ReceiveBufferSize), ReceiveMessage, null);
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "Failed while receiving data");
        }
    }

    public void Disconnect()
    {
        try
        {
            _client.Close();
            _IsConnected = false;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "Error during client disconnect");
        }
        finally
        {
            OnRemove?.Invoke(this, _clientIP);
        }
    }
}
