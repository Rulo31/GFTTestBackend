using System;
using System.Security.Cryptography;
using System.Text;

namespace GFTTestBackend.API.Utils
{
	public class Encryption
	{
		public static string Decrypt(string input, string key)
		{
			byte[] inputArray = Convert.FromBase64String(input);
			using (var tripleDes = new TripleDESCryptoServiceProvider())
			{
				tripleDes.Key = Encoding.UTF8.GetBytes(key); //UTF8Encoding.UTF8.GetBytes();
				tripleDes.Mode = CipherMode.ECB;
				tripleDes.Padding = PaddingMode.PKCS7;
				ICryptoTransform cTransform = tripleDes.CreateDecryptor();
				byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
				tripleDes.Clear();
				return Encoding.UTF8.GetString(resultArray, 0, resultArray.Length);
			}
		}

		public static string Encrypt(string input, string key)
		{
			byte[] inputArray = Encoding.UTF8.GetBytes(input);
			using (var tripleDes = new TripleDESCryptoServiceProvider())
			{
				tripleDes.Key = Encoding.UTF8.GetBytes(key);
				tripleDes.Mode = CipherMode.ECB;
				tripleDes.Padding = PaddingMode.PKCS7;
				ICryptoTransform cTransform = tripleDes.CreateEncryptor();
				byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
				tripleDes.Clear();
				return Convert.ToBase64String(resultArray, 0, resultArray.Length);
			}
		}
	}
}