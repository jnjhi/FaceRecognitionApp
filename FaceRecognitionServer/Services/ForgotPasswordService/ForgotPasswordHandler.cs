using DataProtocols;
using DataProtocols.Authentication.ForgotPasswordMessages;
using FaceRecognitionServer;
using FaceRecognitionServer.Services;
using FaceRecognitionServer.Services.DataBases.ConnectionToTables;
using System.Collections.Concurrent;

// Handles forgot password functionality, including sending reset codes and processing password resets.
// Implements two message types: ForgotPasswordCodeRequestDTO and ResetPasswordRequestDTO.
public class ForgotPasswordHandler : ITypedMessageHandler<ResetPasswordRequestDTO>, ITypedMessageHandler<ForgotPasswordCodeRequestDTO>
{
    // Interface to user database — used to lookup users and update passwords
    private readonly IConnectionToUserDataBase m_Database;

    // Network manager used to send responses back to the client
    private readonly ISecureNetworkManager m_Network;

    // Thread-safe map storing one-time reset codes per email
    private readonly ConcurrentDictionary<string, string> m_CodeMap = new();

    public ForgotPasswordHandler(ISecureNetworkManager network, IConnectionToUserDataBase userStorageSystem)
    {
        m_Network = network;
        m_Database = userStorageSystem;
    }

    // Handles actual password reset once the client submits the code and new password
    public async Task HandleMessageAsync(ResetPasswordRequestDTO message, string ip)
    {
        var response = new ResetPasswordResponseDTO();

        // Check if the code matches what was previously sent
        if (!m_CodeMap.TryGetValue(message.Email, out var expectedCode) || !string.Equals(message.Code, expectedCode, StringComparison.OrdinalIgnoreCase))
        {
            response.IsSuccessful = false;
            response.Message = "Invalid or expired code.";
        }
        else
        {
            // Attempt to update the password in the DB 
            bool success = m_Database.UpdateUserPassword(message.Email, message.NewPassword);
            response.IsSuccessful = success;
            response.Message = success ? "Password reset successful." : "Failed to update password.";

            // If successful, remove the used code
            if (success)
            {
                m_CodeMap.TryRemove(message.Email, out _);
            }
        }

        // Send result back to the client
        var json = ConvertUtils.Serialize(response);
        m_Network.SendMessage(json, ip);
    }

    // Handles generation and sending of password reset code
    public async Task HandleMessageAsync(ForgotPasswordCodeRequestDTO message, string ip)
    {
        var user = m_Database.GetUserByEmail(message.Email);
        if (user == null) return; // email not registered, do nothing

        var code = GenerateResetCode();
        m_CodeMap[message.Email] = code; //store the code in the dictionary

        // Send the code to the user's email
        await EmailSender.SendEmailAsync(message.Email, "Password Reset Code", $"Your reset code is: {code}");
    }

    // Generates a random 6-character alphanumeric reset code
    private string GenerateResetCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var rand = new Random();
        return new string(Enumerable.Range(0, 6).Select(_ => chars[rand.Next(chars.Length)]).ToArray());
    }
}
