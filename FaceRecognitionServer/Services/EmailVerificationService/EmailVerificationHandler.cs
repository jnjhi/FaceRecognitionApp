using DataProtocols;
using DataProtocols.Authentication.EmailVereficationMessages;
using FaceRecognitionServer;
using FaceRecognitionServer.Services;
using System.Collections.Concurrent;

// Handles email verification logic. This class supports both sending verification codes
// and verifying user-provided codes. It is registered as a handler for two message types:
// EmailVerificationCodeRequestDTO and EmailVerificationCodeVerificationRequestDTO.
public class EmailVerificationHandler : ITypedMessageHandler<EmailVerificationCodeRequestDTO>, ITypedMessageHandler<EmailVerificationCodeVerificationRequestDTO>
{
    // Used to send network responses
    private readonly ISecureNetworkManager m_Network;

    // A thread-safe dictionary mapping user email to the latest verification code sent
    private readonly ConcurrentDictionary<string, string> m_Codes = new();

    public EmailVerificationHandler(ISecureNetworkManager network)
    {
        m_Network = network;
    }

    // Handles requests to generate and send a new email verification code.
    public async Task HandleMessageAsync(EmailVerificationCodeRequestDTO request, string ip)
    {
        var code = GenerateCode();
        m_Codes[request.Email] = code;

        await EmailSender.SendEmailAsync(
            request.Email,
            "Your Verification Code",
            $"Your verification code is: {code}"
        );
    }

    // Handles requests to verify a submitted code.
    public async Task HandleMessageAsync(EmailVerificationCodeVerificationRequestDTO request, string ip)
    {
        //pulls the correct code from storage and checks if the code that user entered is the correct one.
        bool isVerified = m_Codes.TryGetValue(request.Email, out var correctCode) && string.Equals(request.Code, correctCode, StringComparison.OrdinalIgnoreCase); 

        var response = new EmailVerificationCodeVerificationResponseDTO
        {
            IsVerified = isVerified
        };

        var json = ConvertUtils.Serialize(response);
        m_Network.SendMessage(json, ip);
    }

    // Utility function to generate a random 6-character verification code.
    private string GenerateCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var rand = new Random();
        return new string(Enumerable.Range(0, 6).Select(_ => chars[rand.Next(chars.Length)]).ToArray());
    }
}
