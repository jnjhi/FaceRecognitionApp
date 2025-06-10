namespace Encryption
{
    public interface IAESEncryption
    {
        string GetKey();
        
        string GetIV();

        void GenerateKey();

        void LoadKey(string key);

        void LoadIV(string iv);

        string Encrypt(string plaintext);

        string Decrypt(string ciphertext);
    }
}
