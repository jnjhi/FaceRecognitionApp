using DataProtocols.AttendanceMessages;
using DataProtocols.Authentication.EmailVereficationMessages;
using DataProtocols.Authentication.ForgotPasswordMessages;
using DataProtocols.Authentication.LogInMessages;
using DataProtocols.Authentication.SignUpMessages;
using DataProtocols.FaceRecognitionMessages;
using DataProtocols.GalleryMessages;
using DataProtocols.DisconnectMessages;
using DataProtocols.NetworkConnection;
using DataProtocols.RetrievingPersonDataMessages;
using Newtonsoft.Json;

namespace DataProtocols
{
    public static class ConvertUtils
    {
        private static readonly Dictionary<DataType, Func<string, object>> m_Deserializers = new()
        {
            { DataType.AESKey, input => JsonConvert.DeserializeObject<AESKeyDTO>(input) },
            { DataType.RSAKey, input => JsonConvert.DeserializeObject<RSAPublicKeyDTO>(input) },
            { DataType.LogInData, input => JsonConvert.DeserializeObject<LogInDataDTO>(input) },
            { DataType.SignUpData, input => JsonConvert.DeserializeObject<SignUpDataDTO>(input) },
            { DataType.LogInAnswer, input => JsonConvert.DeserializeObject<LogInAnswerDTO>(input) },
            { DataType.SignUpAnswer, input => JsonConvert.DeserializeObject<SignUpAnswerDTO>(input) },
            { DataType.RecognizeAndLogFaceRequest, input => JsonConvert.DeserializeObject<PreRecognitionFaceDataDTO>(input) },
            { DataType.FaceRecognitionResult, input => JsonConvert.DeserializeObject<FaceRecognitionResultDTO>(input) },
            { DataType.UpdatePersonDataRequest, input => JsonConvert.DeserializeObject<UpdatePersonDataRequestDTO>(input) },
            { DataType.UpdatePersonDataResponse, input => JsonConvert.DeserializeObject<UpdatePersonDataResponseDTO>(input) },
            { DataType.ForgotPasswordCodeRequest, input => JsonConvert.DeserializeObject<ForgotPasswordCodeRequestDTO>(input) },
            { DataType.ResetPasswordRequest, input => JsonConvert.DeserializeObject<ResetPasswordRequestDTO>(input) },
            { DataType.ResetPasswordResponse, input => JsonConvert.DeserializeObject<ResetPasswordResponseDTO>(input) },
            { DataType.EmailVerificationCodeRequest, input => JsonConvert.DeserializeObject<EmailVerificationCodeRequestDTO>(input) },
            { DataType.EmailVerificationCodeVerificationRequest, input => JsonConvert.DeserializeObject<EmailVerificationCodeVerificationRequestDTO>(input) },
            { DataType.EmailVerificationCodeVerificationResponse, input => JsonConvert.DeserializeObject<EmailVerificationCodeVerificationResponseDTO>(input) },
            { DataType.GetGalleryRequest, input => JsonConvert.DeserializeObject<GetGalleryRequestDTO>(input) },
            { DataType.GetGalleryResponse, input => JsonConvert.DeserializeObject<GetGalleryResponseDTO>(input) },
            { DataType.GetPersonAttendanceRequest, input => JsonConvert.DeserializeObject<GetPersonAttendanceRequestDTO>(input) },
            { DataType.GetPersonAttendanceResponse, input => JsonConvert.DeserializeObject<GetPersonAttendanceResponseDTO>(input) },
            { DataType.GetAllAttendanceRequest, input => JsonConvert.DeserializeObject<GetAllAttendanceRequestDTO>(input) },
            { DataType.GetAllAttendanceResponse, input => JsonConvert.DeserializeObject<GetAllAttendanceResponseDTO>(input) },
            { DataType.GetAdvancedPersonDataWithProfilePictureByIdRequest, input => JsonConvert.DeserializeObject<GetAdvancedPersonDataWithProfilePictureByIdRequestDTO>(input) },
            { DataType.GetAdvancedPersonDataWithProfilePictureByIdResponse, input => JsonConvert.DeserializeObject<GetAdvancedPersonDataWithProfilePictureByIdResponseDTO>(input) },
            { DataType.DisconnectMessage, input => JsonConvert.DeserializeObject<DisconnectMessageDTO>(input) }
        };

        public static string Serialize(Data data) => JsonConvert.SerializeObject(data, Formatting.Indented);

        public static DataType GetDataType(string input) => JsonConvert.DeserializeObject<Data>(input).DataType;

        public static T Deserialize<T>(string input)
        {
            var dataType = GetDataType(input);

            if (!m_Deserializers.TryGetValue(dataType, out var deserializer))
            {
                throw new Exception($"Unknown or unregistered data type: {dataType}");
            }

            var result = deserializer(input);

            if (result is T typedResult)
            {
                return typedResult;
            }

            throw new InvalidCastException($"Data type mismatch. Expected: {typeof(T)}, Actual: {result.GetType()}");
        }
    }
}
