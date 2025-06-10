using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;
using System.Linq;

// Basic TCP server that listens for client connections and routes incoming messages.
// Does not handle encryption — this is the raw unencrypted communication layer.
internal class NetworkManager : INetworkManager
{
    private const int k_PortNo = 5000;
    private const string k_IpAddress = "127.0.0.1";
    private const int k_MaxConnectionsPer10Sec = 10;

    public event Action<string> OnClientRemove;
    public event Action<string> OnClientAdd;
    public event Action<string, string> OnMessageReceive;

    private List<IChatClient> AllClients = new List<IChatClient>();
    private TcpListener m_Listener;

    // Keeps a log of connection timestamps per IP for basic DoS protection
    private readonly ConcurrentDictionary<IPAddress, List<DateTime>> _connectionLog = new();

    public NetworkManager() { }

    // Starts the TCP listener and launches the listening thread
    public void Connect()
    {
        IPAddress localAdd = IPAddress.Parse(k_IpAddress);
        m_Listener = new TcpListener(localAdd, k_PortNo);

        Console.WriteLine("Simple TCP Server");
        Console.WriteLine("Listening to ip {0} port: {1}", k_IpAddress, k_PortNo);
        Console.WriteLine("Server is ready.");

        m_Listener.Start();

        Thread thread = new Thread(Listen);
        thread.Start();

        Console.WriteLine("Network manager has been instantiated");
    }

    // Accepts incoming client connections in a loop
    private void Listen()
    {
        while (true)
        {
            AllClients.Add(AddNewClient(m_Listener));
        }
    }

    // Accepts and initializes a new client connection
    private IChatClient AddNewClient(TcpListener listener)
    {
        TcpClient tcpClient = listener.AcceptTcpClient();
        var remoteEndPoint = tcpClient.Client.RemoteEndPoint as IPEndPoint;
        if (remoteEndPoint == null)
        {
            tcpClient.Close();
            throw new Exception("Invalid client endpoint.");
        }

        IPAddress clientIP = remoteEndPoint.Address;

        // Basic DoS protection — reject too many rapid connections from one IP
        if (!CheckDosProtection(clientIP))
        {
            Console.WriteLine($"Blocked IP {clientIP} - suspected DoS attempt.");
            tcpClient.Close();
            return null;
        }

        Console.WriteLine("New socket: " + tcpClient.Client.RemoteEndPoint.ToString());

        IChatClient user = new ChatClient(tcpClient);
        user.OnReceiveDataFromTheClient += OnMessageReceiveFromClient;
        user.OnRemove += OnRemove;

        OnClientAdd?.Invoke(user.GetClientIP);

        return user;
    }

    // Tracks connection timestamps to rate-limit per IP
    private bool CheckDosProtection(IPAddress ip)
    {
        var now = DateTime.Now;
        var connectionList = _connectionLog.GetOrAdd(ip, _ => new List<DateTime>());

        lock (connectionList)
        {
            connectionList.RemoveAll(t => (now - t).TotalSeconds > 10);
            connectionList.Add(now);
            return connectionList.Count < k_MaxConnectionsPer10Sec;
        }
    }

    // Sends a message to all connected clients
    public void Broadcast(string str)
    {
        foreach (var item in AllClients)
        {
            item.SendMessage(str);
        }
    }

    // Sends a message to a specific client by matching their IP
    public void SendMessage(string message, string clientIp)
    {
        foreach (var client in AllClients)
        {
            if (client.GetClientIP == clientIp)
            {
                client.SendMessage(message);
            }
        }
    }

    public void DisconnectClient(string clientIp)
    {
        var client = AllClients.FirstOrDefault(c => c.GetClientIP == clientIp);
        if (client != null)
        {
            client.Disconnect();
        }
    }

    // Clears the current client list (does not disconnect TCP streams)
    public void RemoveAllTheClients() => AllClients.Clear();

    // Stops the TCP listener, terminating the server
    public void Disconnect() => m_Listener?.Stop();

    // Handles client disconnection cleanup
    private void OnRemove(IChatClient chatClient, string id)
    {
        AllClients.Remove(chatClient);
        OnClientRemove?.Invoke(id);
    }

    // Invoked when a client sends a message
    private void OnMessageReceiveFromClient(string message, string ip)
    {
        OnMessageReceive?.Invoke(message, ip);

        if (OnMessageReceive == null)
        {
            Console.WriteLine("no one has yet subscribed to this event");
        }
    }
}
