using DataProtocols;
using DataProtocols.Authentication.ErrorMessages;
using DataProtocols.Authentication.Models;
using DataProtocols.Authentication.SignUpMessages;
using FaceRecognitionServer.Services;
using FaceRecognitionServer.Services.DataBases.ConnectionToTables;

// Handles user registration requests from the client.
// Implements ITypedMessageHandler<SignUpDataDTO> to validate and store new users.
public class SignUpHandler : ITypedMessageHandler<SignUpDataDTO>
{
    // Network manager to send a response to the client
    private readonly ISecureNetworkManager _networkManager;

    // Interface to user database operations
    private readonly IConnectionToUserDataBase _db;

    public SignUpHandler(ISecureNetworkManager networkManager, IConnectionToUserDataBase userStorageSystem)
    {
        _networkManager = networkManager;
        _db = userStorageSystem;
    }

    // Processes the signup request: checks for duplicates, inserts the user, and returns result
    public async Task HandleMessageAsync(SignUpDataDTO message, string ip)
    {
        await Task.Run(() =>
        {
            var answer = new SignUpAnswerDTO
            {
                ValidationResult = new ValidationResultDTO(),
                UserData = new UserDataDTO()
            };

            if (_db.IsUserNameExists(message.UserName))
            {
                // Case 1: Username already taken
                answer.IsSignUpSuccessful = false;
                answer.ValidationResult.UserNameError = "username already taken";
            }
            else
            {
                // Case 2: Username is available, insert new user
                _db.InsertNewUser(message);
                answer.IsSignUpSuccessful = true;

                var userRecord = _db.GetUserByUserName(message.UserName);
                answer.UserData.UserId = userRecord.UserId;
                answer.UserData.UserName = userRecord.UserName;
                answer.UserData.Email = userRecord.Email;
            }

            // Send result back to the client
            string response = ConvertUtils.Serialize(answer);
            _networkManager.SendMessage(response, ip);
        });
    }
}
