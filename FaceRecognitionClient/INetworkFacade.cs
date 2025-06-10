using DataProtocols;

namespace FaceRecognitionClient
{
    /// <summary>
    /// Defines a high-level interface for network communication from the client to the server.
    /// This abstraction hides encryption, serialization, and raw socket management.
    /// </summary>
    public interface INetworkFacade
    {
        /// <summary>
        /// Sends a request to the server and asynchronously waits for a typed response.
        /// </summary>
        /// <typeparam name="TRequest">The request data type (must inherit from Data)</typeparam>
        /// <typeparam name="TResponse">The response data type expected from the server</typeparam>
        /// <param name="request">The actual request object to send</param>
        /// <returns>A task containing the deserialized server response</returns>
        Task<TResponse> SendRequestAsync<TRequest, TResponse>(TRequest request) where TRequest : Data where TResponse : Data;

        void SendRequestFireAndForget<T>(T request) where T : Data;
    }
}
