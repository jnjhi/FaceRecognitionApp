using DataProtocols.NetworkConnection;
using DataProtocols;
using Encryption;
using Newtonsoft.Json;

// Provides encrypted communication over the underlying NetworkManager using RSA and AES.
// Handles secure key exchange, message encryption/decryption, and client session management.
public class SecureNetworkManager : ISecureNetworkManager
{
    // Triggered when a decrypted message is received from a client
    public event Action<string, string> OnMessageReceive;

    // Triggered when a new client has completed secure handshake
    public event Action<string> OnClientAdd;

    // Underlying unencrypted network layer
    private INetworkManager m_NetworkManager;

    // Holds per-client encryption data (RSA + AES)
    private Dictionary<string, ClientData> m_Clients;

    // Initializes the network and subscribes to message receive event
    public void Connect()
    {
        m_NetworkManager = new NetworkManager();
        m_Clients = new Dictionary<string, ClientData>();

        // Intercept and decrypt all messages received from clients
        m_NetworkManager.OnMessageReceive += OnMessageReceiveFromClient;

        m_NetworkManager.Connect();
    }

    // Encrypts and sends a message to all clients using their individual AES keys
    public void Broadcast(string message)
    {
        foreach (var client in m_Clients)
        {
            SendMessage(message, client.Key);
        }
    }

    // Encrypts the message using AES and sends it to the specified client
    public void SendMessage(string message, string clientIp)
    {
        var encryptedMessage = m_Clients[clientIp].AESEncryption.Encrypt(message);
        m_NetworkManager.SendMessage(encryptedMessage, clientIp);
    }

    // Clears all client encryption sessions and removes clients from the base network
    public void RemoveAllTheClients()
    {
        m_Clients.Clear();
        m_NetworkManager.RemoveAllTheClients();
    }

    // Gracefully stops the underlying network server
    public void Disconnect()
    {
        m_NetworkManager.Disconnect();
    }

    // Handles incoming messages — decrypts if client is known, or performs key exchange if not
    private void OnMessageReceiveFromClient(string message, string ip)
    {
        if (m_Clients.ContainsKey(ip))
        {
            // AES-decrypt the message and forward it
            var decryptedMessage = m_Clients[ip].AESEncryption.Decrypt(message);
            OnMessageReceive.Invoke(decryptedMessage, ip);
        }
        else
        {
            // Handle initial handshake using RSA public key from the client
            AddNewClient(message, ip);
        }
    }

    // Initializes encryption keys for a new client and performs AES key exchange using RSA
    private void AddNewClient(string rsaPublicKeyMessage, string ip)
    {
        var clientData = new ClientData
        {
            AESEncryption = new AESEncryption(),
            RSAEncryption = new RSAEncryption()
        };

        m_Clients.Add(ip, clientData);

        // Deserialize client's RSA public key
        var publicKey = ConvertUtils.Deserialize<RSAPublicKeyDTO>(rsaPublicKeyMessage);
        clientData.RSAEncryption.LoadPublicKey(publicKey.PublicKey);

        // Generate new AES session key
        clientData.AESEncryption.GenerateKey();

        // Prepare AES key exchange object
        var aesKey = new AESKeyDTO
        {
            DataType = DataType.AESKey,
            Key = clientData.AESEncryption.GetKey(),
            IV = clientData.AESEncryption.GetIV()
        };

        // Serialize and encrypt AES key exchange payload with the client's RSA key
        var aesPayload = JsonConvert.SerializeObject(aesKey, Formatting.Indented);
        var encryptedPayload = clientData.RSAEncryption.Encrypt(aesPayload);

        // Send encrypted AES key to client — secure handshake complete
        m_NetworkManager.SendMessage(encryptedPayload, ip);

        // Notify system of new secure client
        OnClientAdd?.Invoke(ip);
    }
}
