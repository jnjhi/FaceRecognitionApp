using DataProtocols;
using FaceRecognitionServer;
using FaceRecognitionServer.Services;

public class MessagePipeline
{
    private readonly Dictionary<DataType, IMessageHandler> _handlers = new();

    public void RegisterHandler<T>(DataType messageType, ITypedMessageHandler<T> handler)
    {
        _handlers[messageType] = new MessageHandlerWrapper<T>(handler);
    }

    public async Task<bool> ProcessMessageAsync(string message, string ip)
    {
        try
        {
            DataType messageType = ConvertUtils.GetDataType(message);

            if (_handlers.TryGetValue(messageType, out var handler))
            {
                await handler.HandleMessageAsync(message, ip);
                return true;
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
        }

        // No matching handler or something failed
        return false;
    }
}