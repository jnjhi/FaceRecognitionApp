namespace FaceRecognitionClient.InternalDataModels
{
    public class PersonDataValidationResult
    {
        public string FirstNameError { get; set; } = string.Empty;
        public string LastNameError { get; set; } = string.Empty;
        public string GovernmentIDError { get; set; } = string.Empty;
        public string SexError { get; set; } = string.Empty;

        public string HeightError { get; set; } = string.Empty;

        /// <summary>
        /// True if all error strings are empty.
        /// </summary>
        public bool IsValid =>
            string.IsNullOrEmpty(FirstNameError) &&
            string.IsNullOrEmpty(LastNameError) &&
            string.IsNullOrEmpty(GovernmentIDError) &&
            string.IsNullOrEmpty(SexError) && 
            string.IsNullOrEmpty(HeightError);
    }
}
