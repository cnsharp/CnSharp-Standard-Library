namespace CnSharp.Security
{
    /// <summary>
    /// RSA encryption algorithm key used in the process.
    /// </summary>
    public class RsaKey
    {
        /// <summary>
        /// Gets the private key used in the encryption process.
        /// </summary>
        public string PrivateKey { get; private set; }

        /// <summary>
        /// Gets the public key used in the decryption process.
        /// </summary>
        public string PublicKey { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RsaKey"/> class.
        /// </summary>
        /// <param name="privateKey">The private key used in the encryption process.</param>
        /// <param name="publicKey">The public key used in the decryption process.</param>
        public RsaKey(string privateKey, string publicKey)
        {
            PrivateKey = privateKey;
            PublicKey = publicKey;
        }
    }
}