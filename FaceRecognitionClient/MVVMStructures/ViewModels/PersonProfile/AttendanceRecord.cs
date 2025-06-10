namespace FaceRecognitionClient.MVVMStructures.ViewModels.PersonProfile
{
    public class AttendanceRecord
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string GovernmentId { get; set; }
        public DateTime AttendanceTime { get; set; }
    }
}