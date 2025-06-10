using DataProtocols;
using DataProtocols.AttendanceMessages;
using DataProtocols.Authentication.EmailVereficationMessages;
using DataProtocols.Authentication.ForgotPasswordMessages;
using DataProtocols.Authentication.LogInMessages;
using DataProtocols.Authentication.SignUpMessages;
using DataProtocols.FaceRecognitionMessages;
using DataProtocols.GalleryMessages;
using DataProtocols.RetrievingPersonDataMessages;
using FaceRecognitionServer.Network;
using FaceRecognitionServer.Services.AttendanceService;
using FaceRecognitionServer.Services.DataBases.ConnectionToTables;
using FaceRecognitionServer.Services.FaceRecognitionService;
using FaceRecognitionServer.Services.RetrievingPersonDataService;

namespace FaceRecognitionServer
{
    /// <summary>
    /// Manages communication between client and server:
    ///    Opens listening on the server side — it is the sole listener. It creates all message handlers,
    ///    registers them in the pipeline so each DataType is associated with its own handler,
    ///    and ensures that any received message is routed correctly through the pipeline.
    /// </summary>
    public class CommunicationManager
    {
        // Underlying secure network manager, handles encryption/decryption and socket I/O
        private readonly ISecureNetworkManager _networkManager;

        // Pipeline that dispatches raw messages to the appropriate typed handler
        private readonly MessagePipeline _pipeline;

        /// <summary>
        /// Initializes the network and message handlers, then begins listening for incoming messages.
        /// </summary>
        public CommunicationManager()
        {
            // Create and connect the secure network manager
            _networkManager = new SecureNetworkManager();
            _networkManager.Connect();      // Open socket, perform any key exchange handshakes

            // Instantiate the message pipeline for routing messages by DataType
            _pipeline = new MessagePipeline();

            var galleryStorageSystem = new GalleryStorageSystem();
            var usersStorageSystem = new ConnectionToUserTable();
            var facesStorageSystem = new ConnectionToFaceTable();
            var attendanceStorageSystem = new AttendanceStorageSystem();

            // Create handler instances, passing the network manager so they can send responses
            var logInHandler = new LogInHandler(_networkManager, usersStorageSystem);
            var signUpHandler = new SignUpHandler(_networkManager, usersStorageSystem);
            var faceRecognitionHandler = new FaceRecognitionHandler(_networkManager, galleryStorageSystem, attendanceStorageSystem);
            var faceRecordHandler = new FaceRecordHandler(_networkManager, facesStorageSystem);
            var forgotPasswordHandler = new ForgotPasswordHandler(_networkManager, usersStorageSystem);
            var emailVerificationHandler = new EmailVerificationHandler(_networkManager);
            var galleryHandler = new GalleryMessageHandler(_networkManager, galleryStorageSystem);
            var attendanceHandler = new AttendanceHandler(_networkManager, attendanceStorageSystem, facesStorageSystem);
            var RetrievingPersonDataService = new RetrievingPersonDataHandler(_networkManager, galleryStorageSystem);

            // Register each handler in the pipeline under its corresponding DataType
            _pipeline.RegisterHandler<LogInDataDTO>(DataType.LogInData, logInHandler);
            _pipeline.RegisterHandler<SignUpDataDTO>(DataType.SignUpData, signUpHandler);
            _pipeline.RegisterHandler<PreRecognitionFaceDataDTO>(DataType.RecognizeAndLogFaceRequest, faceRecognitionHandler);
            _pipeline.RegisterHandler<UpdatePersonDataRequestDTO>(DataType.UpdatePersonDataRequest, faceRecordHandler);
            _pipeline.RegisterHandler<ForgotPasswordCodeRequestDTO>(DataType.ForgotPasswordCodeRequest, forgotPasswordHandler);
            _pipeline.RegisterHandler<ResetPasswordRequestDTO>(DataType.ResetPasswordRequest, forgotPasswordHandler);
            _pipeline.RegisterHandler<EmailVerificationCodeRequestDTO>(DataType.EmailVerificationCodeRequest, emailVerificationHandler);
            _pipeline.RegisterHandler<EmailVerificationCodeVerificationRequestDTO>(DataType.EmailVerificationCodeVerificationRequest, emailVerificationHandler);
            _pipeline.RegisterHandler<GetGalleryRequestDTO>(DataType.GetGalleryRequest, galleryHandler);
            _pipeline.RegisterHandler<GetPersonAttendanceRequestDTO>(DataType.GetPersonAttendanceRequest, attendanceHandler);
            _pipeline.RegisterHandler<GetAllAttendanceRequestDTO>(DataType.GetAllAttendanceRequest, attendanceHandler);
            _pipeline.RegisterHandler<GetAdvancedPersonDataWithProfilePictureByIdRequestDTO>(DataType.GetAdvancedPersonDataWithProfilePictureByIdRequest, RetrievingPersonDataService);

            // Subscribe to the event fired when a new message arrives over the network
            _networkManager.OnMessageReceive += async (message, ip) =>
            {
                // Attempt to route the message through our pipeline
                bool processed = await _pipeline.ProcessMessageAsync(message, ip);
                if (!processed)
                {
                    // If no handler was registered for this DataType, log for debugging
                    Console.WriteLine("Unhandled message: " + message);
                }
            };
        }
    }
}
