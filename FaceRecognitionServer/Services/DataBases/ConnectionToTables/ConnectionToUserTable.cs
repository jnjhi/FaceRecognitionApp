using DataProtocols.Authentication.LogInMessages;
using DataProtocols.Authentication.SignUpMessages;
using FaceRecognitionServer.Services.DataBases.Models;
using System.Data.SqlClient;

namespace FaceRecognitionServer.Services.DataBases.ConnectionToTables
{
    // Provides methods for interacting with the Users table, including user registration, login, and password updates.
    internal class ConnectionToUserTable : IConnectionToUserDataBase
    {
        private const string k_ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\denis\source\repos\FaceRecognition\FaceRecognitionServer\Services\DataBases\FaceRecognitionDB.mdf;Integrated Security=True";

        private SqlConnection m_Connection;
        private SqlCommand m_Command;

        public ConnectionToUserTable()
        {
            m_Connection = new SqlConnection(k_ConnectionString);
            m_Command = new SqlCommand();
        }

        // Registers a new user with hashed password and salt.
        public void InsertNewUser(SignUpDataDTO userData)
        {
            var salt = GenerateSalt();
            var hashedPassword = HashPassword(userData.Password, salt);

            m_Command.CommandText = "INSERT INTO Users (username, password, salt, firstName, lastName, email, city) " +
                                  "VALUES (@username, @password, @salt, @firstName, @lastName, @email, @city)";

            m_Command.Parameters.Clear();
            m_Command.Parameters.AddWithValue("@username", userData.UserName);
            m_Command.Parameters.AddWithValue("@password", hashedPassword);
            m_Command.Parameters.AddWithValue("@salt", salt);
            m_Command.Parameters.AddWithValue("@firstName", userData.FirstName);
            m_Command.Parameters.AddWithValue("@lastName", userData.LastName);
            m_Command.Parameters.AddWithValue("@email", userData.Email);
            m_Command.Parameters.AddWithValue("@city", userData.City);

            m_Connection.Open();
            m_Command.Connection = m_Connection;
            m_Command.ExecuteNonQuery();
            m_Connection.Close();
        }

        // Verifies if a username/password combination is valid.
        public bool IsExists(LogInDataDTO logInData)
        {
            m_Command.CommandText = "SELECT password, salt FROM Users WHERE username = @username";
            m_Command.Parameters.Clear();
            m_Command.Parameters.AddWithValue("@username", logInData.UserName);

            m_Connection.Open();
            m_Command.Connection = m_Connection;

            using (var reader = m_Command.ExecuteReader())
            {
                // SqlDataReader is a forward-only reader that returns one row at a time.
                // reader.Read() advances to the next row, and returns false when done.
                if (reader.Read())
                {
                    // Fetch the stored hashed password and salt
                    string storedHashedPassword = reader["password"].ToString();
                    string storedSalt = reader["salt"].ToString();

                    // Re-hash input password with same salt
                    string hashedInputPassword = HashPassword(logInData.Password, storedSalt);

                    m_Connection.Close();
                    return hashedInputPassword == storedHashedPassword;
                }
            }

            m_Connection.Close();
            return false;
        }

        // Checks if a username already exists in the Users table.
        public bool IsUserNameExists(string userName)
        {
            m_Command.CommandText = "SELECT COUNT(*) FROM Users WHERE username = @username";
            m_Command.Parameters.Clear();
            m_Command.Parameters.AddWithValue("@username", userName);

            m_Connection.Open();
            m_Command.Connection = m_Connection;
            int count = (int)m_Command.ExecuteScalar();
            m_Connection.Close();

            return count > 0;
        }

        // Retrieves email address for a given username.
        public string GetUserEmail(string userName)
        {
            m_Command.CommandText = "SELECT email FROM Users WHERE username = @username";
            m_Command.Parameters.Clear();
            m_Command.Parameters.AddWithValue("@username", userName);

            m_Connection.Open();
            m_Command.Connection = m_Connection;

            string email = null;
            using (var reader = m_Command.ExecuteReader())
            {
                if (reader.Read())
                {
                    // Reads column by name
                    email = reader["email"].ToString();
                }
            }

            m_Connection.Close();
            return email;
        }

        // Returns full user details by username.
        public UserRecord GetUserByUserName(string userName)
        {
            m_Command.CommandText = "SELECT Id, username, firstName, lastName, email, city FROM Users WHERE username = @UserName";
            m_Command.Parameters.Clear();
            m_Command.Parameters.AddWithValue("@UserName", userName);

            m_Connection.Open();
            m_Command.Connection = m_Connection;
            var userRecord = fillUserRecord();
            m_Connection.Close();
            return userRecord;
        }

        // Returns full user details by email.
        public UserRecord GetUserByEmail(string email)
        {
            m_Command.CommandText = "SELECT Id, username, firstName, lastName, email, city FROM Users WHERE email = @Email";
            m_Command.Parameters.Clear();
            m_Command.Parameters.AddWithValue("@Email", email);

            m_Connection.Open();
            m_Command.Connection = m_Connection;
            var userRecord = fillUserRecord();
            m_Connection.Close();
            return userRecord;
        }

        // Updates a user's password.
        public bool UpdateUserPassword(string email, string newPassword)
        {
            var salt = GenerateSalt();
            var hashedPassword = HashPassword(newPassword, salt);

            m_Command.CommandText = "UPDATE Users SET password = @Password, salt = @Salt WHERE email = @Email";
            m_Command.Parameters.Clear();
            m_Command.Parameters.AddWithValue("@Password", hashedPassword);
            m_Command.Parameters.AddWithValue("@Salt", salt);
            m_Command.Parameters.AddWithValue("@Email", email);

            m_Connection.Open();
            m_Command.Connection = m_Connection;
            int rowsAffected = m_Command.ExecuteNonQuery();
            m_Connection.Close();

            return rowsAffected > 0;
        }

        // Checks if the email is already used.
        public bool IsEmailRegistered(string email)
        {
            m_Command.CommandText = "SELECT COUNT(*) FROM Users WHERE email = @Email";
            m_Command.Parameters.Clear();
            m_Command.Parameters.AddWithValue("@Email", email);

            m_Connection.Open();
            m_Command.Connection = m_Connection;
            int count = (int)m_Command.ExecuteScalar();
            m_Connection.Close();

            return count > 0;
        }

        // Securely generates a new 16-byte salt using RNGCryptoServiceProvider
        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        // Hashes password using SHA-256 with the provided salt
        private string HashPassword(string password, string salt)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var saltedPassword = password + salt;
                byte[] hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(hashBytes);
            }
        }

        // Reads a user record from the result set
        private UserRecord fillUserRecord()
        {
            using (var reader = m_Command.ExecuteReader())
            {
                if (reader.Read())
                {
                    // Example of GetString(GetOrdinal(...)) usage:
                    // reader.GetOrdinal("username") → finds the column index of "username"
                    // reader.GetString(index) → gets the string value from that column index
                    // This is safer and faster than using reader["username"].ToString()

                    return new UserRecord(
                        reader.GetInt32(reader.GetOrdinal("Id")),
                        reader.GetString(reader.GetOrdinal("username")),
                        reader.GetString(reader.GetOrdinal("firstName")),
                        reader.GetString(reader.GetOrdinal("lastName")),
                        reader.GetString(reader.GetOrdinal("email")),
                        reader.GetString(reader.GetOrdinal("city"))
                    );
                }
                else
                {
                    throw new Exception("User not found");
                }
            }
        }
    }
}
