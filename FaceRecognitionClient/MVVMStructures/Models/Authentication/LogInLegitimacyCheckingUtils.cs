using DataProtocols;
using DataProtocols.Authentication.ErrorMessages;

namespace FaceRecognitionClient.MVVMStructures.Models.Authentication
{
    public static class LogInLegitimacyCheckingUtils
    {
        private const int k_MinimalUsernameLength = 6;
        private const int k_MinimalDomainPartLength = 2;
        private const char AT_SYMBOL = '@';
        private const char DOT_SYMBOL = '.';
        private const int k_MinimalNameLength = 2;
        private const int MIN_PASSWORD_LENGTH = 8;

        public static ValidationResultDTO ValidateSignUp(string username, string password, string firstName, string lastName, string email)
        {
            var result = new ValidationResultDTO();

            if (string.IsNullOrWhiteSpace(username))
                result.UserNameError = "Username is required.";
            else if (username.Length < k_MinimalUsernameLength)
                result.UserNameError = $"Username must be at least {k_MinimalUsernameLength} characters.";

            if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < k_MinimalNameLength)
                result.FirstNameError = $"First name must be at least {k_MinimalNameLength} characters.";

            if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < k_MinimalNameLength)
                result.LastNameError = $"Last name must be at least {k_MinimalNameLength} characters.";

            if (string.IsNullOrWhiteSpace(email))
                result.EmailError = "Email is required.";
            else
            {
                string emailError = IsEmailOk(email);
                if (!string.IsNullOrEmpty(emailError))
                    result.EmailError = emailError;
            }

            if (string.IsNullOrWhiteSpace(password))
                result.PasswordError = "Password is required.";
            else
            {
                string passwordError = IsPasswordStrongEnough(password);
                if (!string.IsNullOrEmpty(passwordError))
                    result.PasswordError = passwordError;
            }

            return result;
        }

        public static ValidationResultDTO ValidateLogIn(string username, string password)
        {
            var result = new ValidationResultDTO();

            if (string.IsNullOrWhiteSpace(username))
                result.UserNameError = "Username cannot be empty.";
            else if (username.Length < k_MinimalUsernameLength)
                result.UserNameError = $"Username must be at least {k_MinimalUsernameLength} characters long.";

            if (string.IsNullOrWhiteSpace(password))
                result.PasswordError = "Password cannot be empty.";

            return result;
        }

        public static bool IsUserNameAndPasswordValid(string username, string password, out ValidationResultDTO validation)
        {
            validation = ValidateLogIn(username, password);
            return validation.IsValid;
        }

        private static string IsPasswordStrongEnough(string password)
        {
            string ErrorMessage = "";

            if (password.Length < MIN_PASSWORD_LENGTH)
                ErrorMessage += $"Password must be at least {MIN_PASSWORD_LENGTH} characters long.\n";

            if (!password.Any(char.IsUpper))
                ErrorMessage += "Password must contain at least one uppercase letter.\n";

            if (!password.Any(char.IsLower))
                ErrorMessage += "Password must contain at least one lowercase letter.\n";

            if (!password.Any(char.IsDigit))
                ErrorMessage += "Password must contain at least one digit.\n";

            return ErrorMessage.Trim();
        }


        private static string IsEmailOk(string email)
        {
            string ErrorMessage = "";

            if (!email.Contains(AT_SYMBOL))
            {
                ErrorMessage += "Email is missing '@' symbol.\n";
            }
            else
            {
                string[] AtSeparation = email.Split(AT_SYMBOL);

                if (AtSeparation[0].Length < k_MinimalUsernameLength)
                {
                    ErrorMessage += "Username portion of email is too short.\n";
                }
                if (!AtSeparation[1].Contains(DOT_SYMBOL))
                {
                    ErrorMessage += "Domain portion of email is missing '.' symbol.\n";
                }
                else
                {
                    string[] DotSeparation = AtSeparation[1].Split(DOT_SYMBOL);
                    if (DotSeparation.Length < 2 || DotSeparation[0].Length < k_MinimalDomainPartLength || DotSeparation[1].Length < k_MinimalDomainPartLength)
                    {
                        ErrorMessage += "Invalid domain format.\n";
                    }
                }
            }

            return ErrorMessage.Trim();
        }
    }
}

