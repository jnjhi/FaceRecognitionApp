using DataProtocols.Authentication.SignUpMessages;

namespace FaceRecognitionClient.MVVMStructures.Models.Authentication
{
    /// <summary>
    /// Handles user registration. Sends sign-up request to server,
    /// and stores returned user info into the session if sign-up is successful.
    /// </summary>
    public class SignUpModel
    {
        private INetworkFacade m_NetworkFacade;
        private UserSession m_UserSession;

        public SignUpModel(INetworkFacade networkFacade, UserSession userSession)
        {
            m_NetworkFacade = networkFacade;
            m_UserSession = userSession;
        }

        /// <summary>
        /// Sends a SignUpDataDTO request to the server.
        /// If successful, stores the returned UserId, UserName, and Email into the session.
        /// </summary>
        public async Task<SignUpAnswerDTO> SignUp(string username, string password, string firstName, string lastName, string email, string city)
        {
            var response = await m_NetworkFacade.SendRequestAsync<SignUpDataDTO, SignUpAnswerDTO>(
                new SignUpDataDTO(username, password, firstName, lastName, email, city));

            if (response.IsSignUpSuccessful)
            {
                m_UserSession.UserId = response.UserData.UserId;
                m_UserSession.UserName = response.UserData.UserName;
                m_UserSession.Email = response.UserData.Email;
            }

            return response;
        }
    }
}
