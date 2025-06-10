namespace FaceRecognitionClient.InternalDataModels
{
    public class BasicPersonData
    {
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string GovernmentID { get; set; }

        public BasicPersonData(string fullName, string firstName, string lastName, string governmentId)
        {
            FullName = fullName;
            FirstName = firstName;
            LastName = lastName;
            GovernmentID = governmentId;
        }
    }
}
