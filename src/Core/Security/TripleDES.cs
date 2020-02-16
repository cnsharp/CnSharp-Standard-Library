using System;
using System.Security.Cryptography;
using System.Text;

namespace CnSharp.Security
{
	/// <summary>
	/// 3DES 加密算法
	/// </summary>
	public static class TripleDES
	{

		#region Public Methods

		/// <summary>
		/// 3des解密字符串
		/// </summary>
		/// <param name="entryptText">密文</param>
		/// <param name="encoding">编码方式</param>
		/// <returns>解密后的字符串</returns>
		/// <remarks>静态方法，指定编码方式</remarks>
        public static string Decrypt3DES(string entryptText, string key, Encoding encoding = null)
		{
		    if (encoding == null)
		        encoding = Encoding.UTF8;
			var des = new TripleDESCryptoServiceProvider();
			var hashMD5 = new MD5CryptoServiceProvider();

			des.Key = hashMD5.ComputeHash(encoding.GetBytes(key));
			des.Mode = CipherMode.ECB;

			ICryptoTransform desDecrypt = des.CreateDecryptor();

			string result = "";
			try
			{
				byte[] buffer = Convert.FromBase64String(entryptText);
				result = encoding.GetString(desDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));
			}
			catch (Exception e)
			{
				throw (new Exception("Invalid Key or input string is not a valid base64 string", e));
			}

			return result;
		}

		/// <summary>
		/// 3des加密字符串
		/// </summary>
		/// <param name="plainText">明文</param>
		/// <param name="encoding">编码方式</param>
		/// <returns>加密后并经base63编码的字符串</returns>
		/// <remarks>重载，指定编码方式</remarks>
		public static string Encrypt3DES(string plainText,string key, Encoding encoding = null)
		{
            if (encoding == null)
                encoding = Encoding.UTF8;
			var des = new TripleDESCryptoServiceProvider();
			var hashMD5 = new MD5CryptoServiceProvider();

			des.Key = hashMD5.ComputeHash(encoding.GetBytes(key));
			des.Mode = CipherMode.ECB;

			ICryptoTransform desEncrypt = des.CreateEncryptor();

			byte[] Buffer = encoding.GetBytes(plainText);
			return Convert.ToBase64String(desEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
		}

		#endregion
	}
}