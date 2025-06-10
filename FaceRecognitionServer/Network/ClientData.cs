using Encryption;

// Holds encryption-related data for a connected client.
// Each client gets a unique RSA and AES instance for secure communication.
internal class ClientData
{
    // RSA encryption instance used for securely exchanging AES keys
    public IRSAEncryption RSAEncryption;

    // AES encryption instance used for encrypting all ongoing communication
    public IAESEncryption AESEncryption;
}