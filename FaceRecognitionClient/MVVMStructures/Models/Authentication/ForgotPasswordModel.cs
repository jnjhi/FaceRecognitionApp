using DataProtocols.Authentication.EmailVereficationMessages;
using DataProtocols.Authentication.ForgotPasswordMessages;
using FaceRecognitionClient;

namespace FaceRecognitionClient.MVVMStructures.Models.Authentication
{
    /// <summary>
    /// This model handles forgot-password logic, including requesting a reset code and updating the password.
    /// It communicates with the server using the INetworkFacade.
    /// </summary>
    public class ForgotPasswordModel
    {
        private readonly INetworkFacade m_Network;

        public ForgotPasswordModel(INetworkFacade network)
        {
            m_Network = network;
        }

        /// <summary>
        /// Sends a request to the server to send a reset code to the specified email.
        /// </summary>
        public async Task SendResetCodeAsync(string email)
        {
            var request = new ForgotPasswordCodeRequestDTO(email);
            m_Network.SendRequestFireAndForget<ForgotPasswordCodeRequestDTO>(request);
        }

        /// <summary>
        /// Sends the reset code and new password to the server for verification and update.
        /// Returns true if the password was reset successfully.
        /// </summary>
        public async Task<bool> VerifyAndResetPasswordAsync(string email, string code, string newPassword)
        {
            var request = new ResetPasswordRequestDTO
            {
                Email = email,
                Code = code,
                NewPassword = newPassword
            };

            var response = await m_Network.SendRequestAsync<ResetPasswordRequestDTO, ResetPasswordResponseDTO>(request);
            return response.IsSuccessful;
        }
    }
}
