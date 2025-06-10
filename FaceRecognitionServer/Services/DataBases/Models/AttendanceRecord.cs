namespace FaceRecognitionServer.Services.DataBases.Models
{
    public class AttendanceRecord
    {
        public int Id { get; set; }
        public int RecognizedPersonId { get; set; }
        public DateTime AttendanceTime { get; set; }
    }
}
