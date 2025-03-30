using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CnSharp.Security
{
    /// <summary>
    /// Helper class for MD5 encryption algorithm
    /// </summary>
    public static class MD5Helper
    {
        #region Private Fields

        private static MD5CryptoServiceProvider _provider = new MD5CryptoServiceProvider();

        #endregion Private Fields

        #region Entrance

        /// <summary>
        /// Encrypts the input data using the MD5 algorithm.
        /// </summary>
        /// <param name="input">The data to be encrypted.</param>
        /// <returns>The encrypted data using MD5.</returns>
        public static string MD5(this string input)
        {
            var data = Encoding.Default.GetBytes(input);

            return _provider.ComputeHash(data)
                            .Aggregate(new StringBuilder(), (x, y) => x.Append(y.ToString("x2")))
                            .ToString();
        }

        #endregion Entrance
    }
}