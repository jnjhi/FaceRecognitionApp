namespace FaceRecognitionServer.Services
{
    // This interface defines a general message handler for raw (string-based) messages.
    public interface  IMessageHandler
    {
        //Method that supposed to handle request from user 
        Task HandleMessageAsync(string message, string ip);
    }
}
