using DataProtocols;
using DataProtocols.Authentication.LogInMessages;
using DataProtocols.Authentication.SignUpMessages;
using FaceRecognitionServer.Services.DataBases.Models;

namespace FaceRecognitionServer.Services.DataBases.ConnectionToTables
{
    public interface IConnectionToUserDataBase
    {
        void InsertNewUser(SignUpDataDTO userData); // Used at signup from client
        bool IsExists(LogInDataDTO logInData);       // Login verification
        bool IsUserNameExists(string userName);   // For uniqueness check
        string GetUserEmail(string userName);     // returns a user with this email
        UserRecord GetUserByEmail(string email);       // Server-side model, not protocol class
        UserRecord GetUserByUserName(string UserName);
        bool UpdateUserPassword(string email, string newPassword); // Uses hashing + new salt
        bool IsEmailRegistered(string email);          // If you need to validate existence first
    }
}
