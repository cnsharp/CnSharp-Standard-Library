using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CnSharp.Security
{
    /// <summary>
    /// RSA encryption algorithm helper class
    /// </summary>
    public sealed class RsaHelper
    {
        #region Private Fields

        private RSACryptoServiceProvider _provider;
        private int _maxEncryptBlockSize;
        private int _maxDecryptBlockSize;

        #endregion Private Fields

        #region Business Properties

        /// <summary>
        /// Gets the RSA key used in the encryption and decryption process.
        /// </summary>
        public string Key { get; private set; }

        #endregion Business Properties

        #region Entrance

        /// <summary>
        /// Initializes a new instance of the <see cref="RsaHelper"/> class.
        /// </summary>
        /// <param name="key">RSA key used in the encryption and decryption process.</param>
        /// <exception cref="System.ArgumentNullException">key</exception>
        public RsaHelper(string key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            _provider = new RSACryptoServiceProvider();
            _provider.FromXmlString(key);
            _maxEncryptBlockSize = _provider.KeySize / 8 - 11;
            _maxDecryptBlockSize = _provider.KeySize / 8;
        }

        #endregion Entrance

        #region Business Methods

        /// <summary>
        /// Encrypts data using the RSA encryption algorithm.
        /// </summary>
        /// <param name="source">The original data to be encrypted.</param>
        /// <returns>Data encrypted by RSA.</returns>
        public string Encrypt(string source)
        {
            var data = Encoding.Unicode.GetBytes(source);
            if (data.Length <= _maxEncryptBlockSize)
                return Convert.ToBase64String(_provider.Encrypt(data, false));

            using (MemoryStream sourceStream = new MemoryStream(data))
            using (MemoryStream targetStream = new MemoryStream())
            {
                var buffer = new byte[_maxEncryptBlockSize];
                int blockSize = sourceStream.Read(buffer, 0, _maxEncryptBlockSize);

                while (blockSize > 0)
                {
                    var toEncrypt = new byte[blockSize];
                    Array.Copy(buffer, 0, toEncrypt, 0, blockSize);

                    var cryptograph = _provider.Encrypt(toEncrypt, false);
                    targetStream.Write(cryptograph, 0, cryptograph.Length);

                    blockSize = sourceStream.Read(buffer, 0, _maxEncryptBlockSize);
                }

                return Convert.ToBase64String(targetStream.ToArray(), Base64FormattingOptions.None);
            }
        }

        /// <summary>
        /// Decrypts data using the RSA encryption algorithm.
        /// </summary>
        /// <param name="source">Data to be decrypted.</param>
        /// <returns>The original data after decryption.</returns>
        public string Decrypt(string source)
        {
            var data = Convert.FromBase64String(source);
            if (data.Length <= _maxDecryptBlockSize)
                return Encoding.Unicode.GetString(_provider.Decrypt(data, false));

            using (MemoryStream sourceStream = new MemoryStream(data))
            using (MemoryStream targetStream = new MemoryStream())
            {
                var buffer = new Byte[_maxDecryptBlockSize];
                int blockSize = sourceStream.Read(buffer, 0, _maxDecryptBlockSize);

                while (blockSize > 0)
                {
                    var toDecrypt = new byte[blockSize];
                    Array.Copy(buffer, 0, toDecrypt, 0, blockSize);

                    var plaintext = _provider.Decrypt(toDecrypt, false);
                    targetStream.Write(plaintext, 0, plaintext.Length);

                    blockSize = sourceStream.Read(buffer, 0, _maxDecryptBlockSize);
                }

                return Encoding.Unicode.GetString(targetStream.ToArray());
            }
        }

        /// <summary>
        /// Creates a new RSA key used in the encryption and decryption process.
        /// </summary>
        /// <returns></returns>
        public static RsaKey CreateNewKey()
        {
            var provider = new RSACryptoServiceProvider();
            return new RsaKey(provider.ToXmlString(true), provider.ToXmlString(false));
        }

        #endregion Business Methods
    }
}