namespace DataProtocols.FaceRecognitionMessages.Models
{
    public  class PersonDataValidationResultDTO
    {
        public string FirstNameError { get; set; } = string.Empty;
        public string LastNameError { get; set; } = string.Empty;
        public string GovernmentIDError { get; set; } = string.Empty;
        public string SexError { get; set; } = string.Empty;
        public string HeightError { get; set; } = string.Empty;
    }
}
