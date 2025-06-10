namespace FaceRecognitionServer.Services.DataBases.Models
{
    public class UserRecord
    {
        public int UserId;
        public string UserName;
        public string FirstName;
        public string LastName;
        public string Email;
        public string City;

        public UserRecord(int userId, string userName, string firstName, string lastName, string email, string city)
        {
            UserId = userId;
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            City = city;
        }
    }
}
