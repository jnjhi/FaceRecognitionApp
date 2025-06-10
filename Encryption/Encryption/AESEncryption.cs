using System.Security.Cryptography;
using System.Text;

namespace Encryption
{
    public class AESEncryption : IAESEncryption
    {
        private Aes m_Aes;

        public AESEncryption()
        {
            m_Aes = Aes.Create();
            m_Aes.Padding = PaddingMode.PKCS7;
        }

        public void GenerateKey()
        {
            m_Aes.GenerateKey();
            m_Aes.GenerateIV();
        }

        public string GetKey()
        {
            return Convert.ToBase64String(m_Aes.Key);
        }

        public string GetIV()
        {
            return Convert.ToBase64String(m_Aes.IV);
        }

        public void LoadKey(string key)
        {
            m_Aes.Key = Convert.FromBase64String(key);
        }

        public void LoadIV(string iv)
        {
            m_Aes.IV = Convert.FromBase64String(iv);
        }

        public string Encrypt(string plaintext)
        {
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, m_Aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(plaintextBytes, 0, plaintextBytes.Length);
                    cs.FlushFinalBlock();
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public string Decrypt(string ciphertext)
        {
            byte[] ciphertextBytes = Convert.FromBase64String(ciphertext);
            using (MemoryStream ms = new MemoryStream(ciphertextBytes))
            {
                using (CryptoStream cs = new CryptoStream(ms, m_Aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(cs))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        public void Dispose()
        {
            m_Aes.Dispose();
        }
    }
}
