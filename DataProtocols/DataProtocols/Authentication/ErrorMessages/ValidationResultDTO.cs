namespace DataProtocols.Authentication.ErrorMessages
{
    public class ValidationResultDTO
    {
        public string UserNameError { get; set; }
        public string PasswordError { get; set; }
        public string FirstNameError { get; set; }
        public string LastNameError { get; set; }
        public string EmailError { get; set; }

        public bool IsValid =>
            string.IsNullOrWhiteSpace(UserNameError) &&
            string.IsNullOrWhiteSpace(PasswordError) &&
            string.IsNullOrWhiteSpace(FirstNameError) &&
            string.IsNullOrWhiteSpace(LastNameError) &&
            string.IsNullOrWhiteSpace(EmailError);
    }
}
