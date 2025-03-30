using System;
using System.Security.Cryptography;
using System.Text;

namespace CnSharp.Security
{
    /// <summary>
    /// 3DES Encryption algorithm
    /// </summary>
    public static class TripleDES
    {
        #region Public Methods

        /// <summary>
        /// Decrypts a string using 3DES.
        /// </summary>
        /// <param name="encryptedText">The encrypted text (ciphertext).</param>
        /// <param name="key">The secret key.</param>
        /// <param name="encoding">The encoding method. Defaults to UTF8 if not specified.</param>
        /// <returns>The decrypted string.</returns>
        public static string Decrypt(string encryptedText, string key, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            var des = new TripleDESCryptoServiceProvider();
            var md5 = new MD5CryptoServiceProvider();

            des.Key = md5.ComputeHash(encoding.GetBytes(key));
            des.Mode = CipherMode.ECB;

            ICryptoTransform desDecrypt = des.CreateDecryptor();

            string result = "";
            try
            {
                byte[] buffer = Convert.FromBase64String(encryptedText);
                result = encoding.GetString(desDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch (Exception e)
            {
                throw new Exception("Invalid Key or input string is not a valid base64 string", e);
            }

            return result;
        }

        /// <summary>
        /// Encrypts a string using 3DES.
        /// </summary>
        /// <param name="plainText">The plain text to encrypt.</param>
        /// <param name="key">The secret key.</param>
        /// <param name="encoding">The encoding method. Defaults to UTF8 if not specified.</param>
        /// <returns>The encrypted string encoded in Base64.</returns>
        public static string Encrypt(string plainText, string key, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            var des = new TripleDESCryptoServiceProvider();
            var md5 = new MD5CryptoServiceProvider();

            des.Key = md5.ComputeHash(encoding.GetBytes(key));
            des.Mode = CipherMode.ECB;

            var desEncrypt = des.CreateEncryptor();

            var buffer = encoding.GetBytes(plainText);
            return Convert.ToBase64String(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
        }

        #endregion
    }
}
