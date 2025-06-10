using DataProtocols;
using DataProtocols.NetworkConnection;
using Encryption;
using Newtonsoft.Json;

namespace FaceRecognitionClient.Network
{
    /*
    SecureNetworkManager establishes encrypted communication between the client and server.
    Upon connection, the client sends its RSA public key to the server as the first request.
    The server responds with an AES session key and IV, encrypted with that RSA public key.
    From that point on, all messages are encrypted with AES and decrypted on receipt.
    */

    /// <summary>
    /// Provides secure communication between client and server using RSA for key exchange and AES for encrypted messaging.
    /// Wraps the raw NetworkManager to provide confidentiality over an insecure channel.
    /// </summary>
    internal class SecureNetworkManager : ISecureNetworkManager
    {
        public event Action<string> OnMessageReceive;

        private INetworkManager m_NetworkManager;
        private IAESEncryption m_AESEncryption;
        private IRSAEncryption m_RSAEncryption;

        public SecureNetworkManager()
        {
            m_NetworkManager = new NetworkManager();
            m_AESEncryption = new AESEncryption();
            m_RSAEncryption = new RSAEncryption();
        }

        /// <summary>
        /// Sends RSA public key to server and waits for AES session key exchange.
        /// </summary>
        public void Connect()
        {
            var publicKey = new RSAPublicKeyDTO
            {
                DataType = DataType.RSAKey,
                PublicKey = m_RSAEncryption.GetPublicKey()
            };

            // Step 1: Send our public key to the server
            m_NetworkManager.SendMessage(JsonConvert.SerializeObject(publicKey, Formatting.Indented));

            // Step 2: Wait for AES key from the server
            m_NetworkManager.OnMessageReceive += OnKeyReceive;
        }

        /// <summary>
        /// Encrypts a message using AES and sends it through the raw network layer.
        /// </summary>
        public void SendMessage(string message)
        {
            try
            {
                var encryptedMessage = m_AESEncryption.Encrypt(message);
                m_NetworkManager.SendMessage(encryptedMessage);
            }
            catch (Exception exception)
            {
                ClientLogger.ClientLogger.LogException(exception, "Failed to send the message");
            }
        }

        /// <summary>
        /// Disconnects the network and unregisters message listeners.
        /// </summary>
        public void Disconnect()
        {
            m_NetworkManager.OnMessageReceive -= OnMessageReceiveFromServer;
            m_NetworkManager.Disconnect();
        }

        /// <summary>
        /// Handles a message received after encryption is established.
        /// Decrypts using AES and raises OnMessageReceive.
        /// </summary>
        private void OnMessageReceiveFromServer(string message)
        {
            try
            {
                var decryptedMessage = m_AESEncryption.Decrypt(message);
                OnMessageReceive?.Invoke(decryptedMessage);
            }
            catch (Exception exception)
            {
                ClientLogger.ClientLogger.LogException(exception, "Failed to decrypt the message");
            }
        }

        /// <summary>
        /// Handles the initial AES key exchange sent encrypted with our RSA public key.
        /// </summary>
        private void OnKeyReceive(string message)
        {
            // Step 1: Decrypt the AESKeyDTO from the RSA-encrypted message
            var decryptedMessage = m_RSAEncryption.Decrypt(message);
            var aesKey = ConvertUtils.Deserialize<AESKeyDTO>(decryptedMessage);

            // Step 2: Load AES key and IV for all future communication
            m_AESEncryption.LoadKey(aesKey.Key);
            m_AESEncryption.LoadIV(aesKey.IV);

            // Step 3: Now switch to AES-decrypted message handling
            m_NetworkManager.OnMessageReceive += OnMessageReceiveFromServer;

            // Step 4: Unsubscribe from this one-time RSA handler
            m_NetworkManager.OnMessageReceive -= OnKeyReceive;
        }
    }
}
