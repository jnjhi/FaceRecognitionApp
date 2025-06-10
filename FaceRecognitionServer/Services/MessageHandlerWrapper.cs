using DataProtocols;

namespace FaceRecognitionServer.Services
{
    // This class wraps a type-safe handler so it can be treated as a general IMessageHandler.
    public class MessageHandlerWrapper<T> : IMessageHandler
    {
        private readonly ITypedMessageHandler<T> _inner;

        public MessageHandlerWrapper(ITypedMessageHandler<T> inner)
        {
            _inner = inner;
        }

        // Handles the raw message by deserializing it to the expected type T,
        // then forwarding it to the inner typed handler.
        public async Task HandleMessageAsync(string message, string ip)
        {
            var typed = ConvertUtils.Deserialize<T>(message); // Safely convert from string to T
            await _inner.HandleMessageAsync(typed, ip);       // Delegate to the actual handler
        }
    }

}
