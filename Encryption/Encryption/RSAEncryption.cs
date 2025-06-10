using System.Security.Cryptography;
using System.Text;

namespace Encryption
{
    public class RSAEncryption : IRSAEncryption
    {
        private RSA m_Rsa;

        public RSAEncryption()
        {
            m_Rsa = RSA.Create();
        }

        public string GetPublicKey()
        {
            byte[] publicKeyBytes = m_Rsa.ExportRSAPublicKey();
            return Convert.ToBase64String(publicKeyBytes);
        }

        public void LoadPublicKey(string base64PublicKey)
        {
            byte[] publicKeyBytes = Convert.FromBase64String(base64PublicKey);
            m_Rsa.ImportRSAPublicKey(publicKeyBytes, out _);
        }

        public string Encrypt(string plainText)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = m_Rsa.Encrypt(plainBytes, RSAEncryptionPadding.OaepSHA256);

            return Convert.ToBase64String(encryptedBytes);
        }


        public string Decrypt(string encryptedText)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] decryptedBytes = m_Rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        public void Dispose()
        {
            m_Rsa.Dispose();
        }
    }
}
