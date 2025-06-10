using DataProtocols;
using DataProtocols.Authentication.ErrorMessages;
using DataProtocols.Authentication.LogInMessages;
using DataProtocols.Authentication.Models;
using FaceRecognitionServer.Services;
using FaceRecognitionServer.Services.DataBases.ConnectionToTables;

// Handles login requests from the client.
// Implements ITypedMessageHandler<LogInDataDTO> to process user login attempts.
public class LogInHandler : ITypedMessageHandler<LogInDataDTO>
{
    // Network manager for sending the response back to the client
    private readonly ISecureNetworkManager _networkManager;

    // Interface to the user table in the database
    private readonly IConnectionToUserDataBase _db;

    public LogInHandler(ISecureNetworkManager networkManager, IConnectionToUserDataBase userStorageSystem)
    {
        _networkManager = networkManager;
        _db = userStorageSystem;
    }

    // Handles the login attempt and responds with success/failure and user data if successful
    public async Task HandleMessageAsync(LogInDataDTO message, string ip)
    {
        await Task.Run(() =>
        {
            bool exists = _db.IsExists(message); // Check if username-password combo is valid

            var answer = new LogInAnswerDTO
            {
                ValidationResult = new ValidationResultDTO(),
                UserData = new UserDataDTO()
            };

            if (exists)
            {
                // Case 1: User exists and password is correct
                answer.IsAccessGranted = true;

                var userRecord = _db.GetUserByUserName(message.UserName);
                answer.UserData.UserId = userRecord.UserId;
                answer.UserData.UserName = userRecord.UserName;
                answer.UserData.Email = userRecord.Email;
            }
            else if (_db.IsUserNameExists(message.UserName))
            {
                // Case 2: Username exists but password is wrong
                answer.IsAccessGranted = false;
                answer.ValidationResult.PasswordError = "The password is wrong";
            }
            else
            {
                // Case 3: Username does not exist
                answer.IsAccessGranted = false;
                answer.ValidationResult.UserNameError = "There is no user with this user name";
            }

            // Send login result back to the client
            string response = ConvertUtils.Serialize(answer);
            _networkManager.SendMessage(response, ip);
        });
    }
}
