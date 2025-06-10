namespace DataProtocols.DisconnectMessages
{
    public class DisconnectMessageDTO : Data
    {
        public string Reason { get; set; }

        public DisconnectMessageDTO()
        {
            DataType = DataType.DisconnectMessage;
            Reason = "Disconnected due to inactivity.";
        }

        public DisconnectMessageDTO(string reason) : this()
        {
            Reason = reason;
        }
    }
}
