using DataProtocols.Authentication.LogInMessages;

namespace FaceRecognitionClient.MVVMStructures.Models.Authentication
{
    /// <summary>
    /// This model handles the login logic. It validates inputs locally,
    /// sends the login request to the server, and updates the current session if login succeeds.
    /// </summary>
    public class LogInModel
    {
        private INetworkFacade m_NetworkFacade;
        private UserSession m_UserSession;

        public LogInModel(INetworkFacade networkFacade, UserSession userSession)
        {
            m_NetworkFacade = networkFacade;
            m_UserSession = userSession;
        }

        /// <summary>
        /// Validates inputs, sends the login request, and updates the session on success.
        /// Returns the login response (whether valid or failed).
        /// </summary>
        public async Task<LogInAnswerDTO> LogIn(string userName, string password)
        {
            if (!LogInLegitimacyCheckingUtils.IsUserNameAndPasswordValid(userName, password, out var validation))
            {
                return new LogInAnswerDTO(false, validation);
            }

            var response = await m_NetworkFacade.SendRequestAsync<LogInDataDTO, LogInAnswerDTO>(
                new LogInDataDTO(userName, password));

            if (!response.IsAccessGranted)
            {
                return response;
            }

            m_UserSession.UserId = response.UserData.UserId;
            m_UserSession.UserName = response.UserData.UserName;
            m_UserSession.Email = response.UserData.Email;

            return response;
        }
    }
}
