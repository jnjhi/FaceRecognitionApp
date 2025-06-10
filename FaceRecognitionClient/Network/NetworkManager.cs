using System.Net.Sockets;

namespace FaceRecognitionClient.Network
{
    /// <summary>
    /// Handles raw TCP connection to the server (unsecured).
    /// Sends/receives ASCII-encoded strings using .NET's TcpClient.
    /// </summary>
    internal class NetworkManager : INetworkManager
    {
        public event Action<string> OnMessageReceive;

        private int m_PortNo = 5000;
        private string m_IpAddress = "127.0.0.1";
        private TcpClient m_Client;
        private byte[] m_Data;
        private bool m_IsConnected = false;

        public NetworkManager()
        {
            Connect(); // Connect automatically when constructed
        }

        public void Connect()
        {
            if (m_IsConnected)
                return;

            m_Client = new TcpClient();
            m_Client.Connect(m_IpAddress, m_PortNo);

            // Buffer for incoming messages
            m_Data = new byte[m_Client.ReceiveBufferSize];

            // Start reading from the stream asynchronously
            m_Client.GetStream().BeginRead(m_Data, 0, m_Data.Length, ReceiveMessage, null);
            m_IsConnected = true;
        }

        public void SendMessage(string message)
        {
            try
            {
                NetworkStream ns = m_Client.GetStream();
                byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
                ns.Write(data, 0, data.Length);
                ns.Flush();
            }
            catch (Exception ex)
            {
                // Optional: handle transmission error
            }
        }

        public void Disconnect()
        {
            try
            {
                if (!m_IsConnected)
                    return;

                m_Client.GetStream().Close();
                m_Client.Close();
            }
            catch (Exception ex)
            {
                // Optional: handle disconnect error
            }
            finally
            {
                m_IsConnected = false;
            }
        }

        private void ReceiveMessage(IAsyncResult ar)
        {
            try
            {
                int bytesRead = m_Client.GetStream().EndRead(ar);

                if (bytesRead < 1)
                    return;

                // Convert the received bytes back into a string
                string textFromServer = System.Text.Encoding.ASCII.GetString(m_Data, 0, bytesRead);

                // Notify listeners
                OnMessageReceive?.Invoke(textFromServer);

                // Continue reading the stream
                m_Client.GetStream().BeginRead(m_Data, 0, m_Data.Length, ReceiveMessage, null);
            }
            catch (Exception ex)
            {
                // Usually triggered when the server disconnects; safe to ignore
            }
        }
    }
}
