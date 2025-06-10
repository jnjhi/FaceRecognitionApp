namespace FaceRecognitionServer.Services
{
    /// <summary>
    ///A generic interface that defines a method for handling a message of type T, where T is a specific deserialized message class.
    ///Each message handler implements this interface multiple times — once for each supported message type it can handle.
    /// </summary>
    public interface ITypedMessageHandler<T> 
    {
        Task HandleMessageAsync(T message, string ip);
    }
}
