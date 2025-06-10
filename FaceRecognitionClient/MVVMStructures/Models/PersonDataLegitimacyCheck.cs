using FaceRecognitionClient.InternalDataModels;

namespace FaceRecognitionClient.MVVMStructures.Models
{
    /// <summary>
    /// Provides field-level validation for user information like name, government ID, and sex.
    /// Used to prevent invalid data from being submitted to the server.
    /// </summary>
    public static class PersonDataLegitimacyCheck
    {
        public static PersonDataValidationResult Validate(string firstName, string lastName, string governmentId, string sex)
        {
            var result = new PersonDataValidationResult();

            if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2)
                result.FirstNameError = "First name must be at least 2 characters.";

            if (!string.IsNullOrWhiteSpace(firstName) && firstName.Any(c => !char.IsLetter(c)))
                result.FirstNameError = "First name may contain letters only.";

            if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2)
                result.LastNameError = "Last name must be at least 2 characters.";

            if (!string.IsNullOrWhiteSpace(lastName) && lastName.Any(c => !char.IsLetter(c)))
                result.LastNameError = "Last name may contain letters only.";

            if (string.IsNullOrWhiteSpace(governmentId) || governmentId.Length != 9 || governmentId.Any(c => !char.IsDigit(c)))
                result.GovernmentIDError = "Government ID must be exactly 9 digits.";

            if (!string.IsNullOrWhiteSpace(governmentId) && governmentId[0] == '0')
                result.GovernmentIDError = "Government ID must not start with 0.";

            var s = (sex ?? "").Trim();
            if (!(s == "Male" || s == "Female" || s == "Other"))
                result.SexError = "Sex must be 'Male', 'Female' or 'Other'.";

            return result;
        }
    }
}
