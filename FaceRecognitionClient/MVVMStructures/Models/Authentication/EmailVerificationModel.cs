using DataProtocols.Authentication.EmailVereficationMessages;

namespace FaceRecognitionClient.MVVMStructures.Models.Authentication
{
    /// <summary>
    /// This model handles the logic related to verifying a user's email address.
    /// It communicates with the server via INetworkFacade to request and verify codes.
    /// </summary>
    public class EmailVerificationModel
    {
        private readonly INetworkFacade m_Network;
        private readonly UserSession m_Session;

        public EmailVerificationModel(INetworkFacade networkFacade, UserSession session)
        {
            m_Network = networkFacade;
            m_Session = session;
        }

        /// <summary>
        /// Sends a request to the server asking it to send a verification code to the user's email.
        /// </summary>
        public async Task<bool> RequestVerificationCodeAsync()
        {
            var request = new EmailVerificationCodeRequestDTO { Email = m_Session.Email };
            m_Network.SendRequestFireAndForget<EmailVerificationCodeRequestDTO>(request);
            return true; 
        }

        /// <summary>
        /// Submits the code entered by the user to the server for verification.
        /// Returns true if the code is correct.
        /// </summary>
        public async Task<bool> SubmitCodeForVerificationAsync(string code)
        {
            var request = new EmailVerificationCodeVerificationRequestDTO
            {
                Email = m_Session.Email,
                Code = code
            };

            var response = await m_Network.SendRequestAsync<EmailVerificationCodeVerificationRequestDTO, EmailVerificationCodeVerificationResponseDTO>(request);
            return response.IsVerified;
        }
    }
}
