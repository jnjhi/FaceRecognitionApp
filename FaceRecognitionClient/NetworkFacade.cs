using DataProtocols;
using DataProtocols.DisconnectMessages;
using FaceRecognitionClient.Network;

namespace FaceRecognitionClient
{
    /// <summary>
    /// NetworkFacade provides a clean and high-level way for the client to communicate with the server.
    /// It handles message serialization, sending, and listening for responses using a secure network channel.
    /// </summary>
    public class NetworkFacade : INetworkFacade
    {
        private ISecureNetworkManager m_SecureNetworkManager;

        public event Action<string> OnServerDisconnected;

        /// <summary>
        /// Constructor initializes the secure network connection (based on TCP + encryption).
        /// </summary>
        public NetworkFacade()
        {
            
        }

        public void Connect()
        {
            m_SecureNetworkManager = new SecureNetworkManager();
            m_SecureNetworkManager.Connect(); // Establish encrypted communication with the server
            m_SecureNetworkManager.OnMessageReceive += HandleIncomingMessage;
        }

        private void HandleIncomingMessage(string message)
        {
            try
            {
                if (ConvertUtils.GetDataType(message) == DataType.DisconnectMessage)
                {
                    var dto = ConvertUtils.Deserialize<DisconnectMessageDTO>(message);
                    OnServerDisconnected?.Invoke(dto.Reason);
                }
            }
            catch (Exception ex)
            {
                ClientLogger.ClientLogger.LogException(ex, "Failed processing passive message");
            }
        }

        /// <summary>
        /// Sends a generic request to the server and asynchronously waits for a specific typed response.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request message (must inherit from Data)</typeparam>
        /// <typeparam name="TResponse">The expected response message type</typeparam>
        /// <param name="request">The request object to send</param>
        /// <returns>The deserialized response from the server</returns>
        public async Task<TResponse> SendRequestAsync<TRequest, TResponse>(TRequest request)  where TRequest : Data where TResponse : Data
        {
            // Step 1: Serialize the request into a JSON string
            string serializedRequest = ConvertUtils.Serialize(request);

            // Step 2: Send the message over the secure network
            m_SecureNetworkManager.SendMessage(serializedRequest);

            // Step 3: Prepare to wait for a response asynchronously
            var task = new TaskCompletionSource<TResponse>();

            // Step 4: Define a temporary message handler to catch the server's response
            Action<string> messageReceivedHandler = null;
            messageReceivedHandler = incomingMessage =>
            {
                try
                {
                    // Try to deserialize the incoming message into the expected response type
                    TResponse response = ConvertUtils.Deserialize<TResponse>(incomingMessage);

                    // If successful, complete the async task and unregister the handler
                    task.TrySetResult(response);
                    m_SecureNetworkManager.OnMessageReceive -= messageReceivedHandler;
                }
                catch(Exception exception)
                {
                    ClientLogger.ClientLogger.LogException(exception, "Was unable to deserialize the message");
                }
            };

            // Step 5: Register the handler to listen for server responses
            m_SecureNetworkManager.OnMessageReceive += messageReceivedHandler;

            // Step 6: Wait for the response and return it
            TResponse result = await task.Task;
            return result;
        }



        public void SendRequestFireAndForget<T>(T request) where T : Data => m_SecureNetworkManager.SendMessage(ConvertUtils.Serialize(request));

    }
}
