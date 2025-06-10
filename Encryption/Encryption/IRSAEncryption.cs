namespace Encryption
{
    public interface IRSAEncryption
    {
        void LoadPublicKey(string publicKey);

        string GetPublicKey();

        string Encrypt(string plainText);

        string Decrypt(string encryptedText);

        void Dispose();
    }
}
