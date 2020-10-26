using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PSA_Arduino_NAC
{
	public class AESCrypt
	{
		private ICryptoTransform encryptor = null;

		private ICryptoTransform decryptor = null;

		private AESCryptOptions Options = null;

		public static string ComputeSha256Hash(string rawData)
		{
			SHA256 sHA = SHA256.Create();
			byte[] array = sHA.ComputeHash(Encoding.UTF8.GetBytes(rawData));
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i = checked(i + 1))
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		public static string ComputeSha1Hash(string rawData)
		{
			SHA1 sHA = SHA1.Create();
			byte[] array = sHA.ComputeHash(Encoding.UTF8.GetBytes(rawData));
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i = checked(i + 1))
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		public AESCrypt(string passPhrase)
			: this(passPhrase, null)
		{
		}

		public AESCrypt(string passPhrase, string initVector)
			: this(passPhrase, initVector, new AESCryptOptions())
		{
		}

		public AESCrypt(string passPhrase, string initVector, AESCryptOptions options)
		{
			Options = options;
			if (Options.FixedKeySize.HasValue && Options.FixedKeySize != 128 && Options.FixedKeySize != 192 && Options.FixedKeySize != 256)
			{
				throw new NotSupportedException("ERROR: options.FixedKeySize must be NULL (for auto-detect) or have a value of 128, 192 or 256");
			}

            byte[] array = initVector != null ? Encoding.UTF8.GetBytes(initVector) : new byte[0];
            int num = (Options.FixedKeySize.HasValue ? Options.FixedKeySize.Value : GetAESKeySize(passPhrase));
			byte[] array3 = null;
			if (Options.PasswordHash == AESPasswordHash.None)
			{
				array3 = Encoding.UTF8.GetBytes(passPhrase);
			}
			else
			{
				PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(passPhrase, (Options.PasswordHashSalt != null) ? Encoding.UTF8.GetBytes(options.PasswordHashSalt) : new byte[0], Options.PasswordHash.ToString().ToUpper().Replace("-", ""), Options.PasswordHashIterations);
				array3 = passwordDeriveBytes.GetBytes(num / 8);
			}
			AesManaged aesManaged = new AesManaged
			{
				Padding = Options.PaddingMode,
				Mode = ((array.Length != 0) ? CipherMode.CBC : CipherMode.ECB)
			};
			encryptor = aesManaged.CreateEncryptor(array3, array);
			decryptor = aesManaged.CreateDecryptor(array3, array);
		}

		public string Encrypt(string plainText)
		{
			return Encrypt(Encoding.UTF8.GetBytes(plainText));
		}

		public string Encrypt(byte[] plainTextBytes)
		{
			return Convert.ToBase64String(EncryptToBytes(plainTextBytes));
		}

		public byte[] EncryptToBytes(string plainText)
		{
			return EncryptToBytes(Encoding.UTF8.GetBytes(plainText));
		}

		public byte[] EncryptToBytes(byte[] plainTextBytes)
		{
			byte[] array = (UseSalt() ? AddSalt(plainTextBytes) : plainTextBytes);
			byte[] result = null;
			lock (this)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
					{
						cryptoStream.Write(array, 0, array.Length);
						cryptoStream.FlushFinalBlock();
						result = memoryStream.ToArray();
						cryptoStream.Close();
					}
					memoryStream.Close();
				}
				return result;
			}
		}

		public string Decrypt(string cipherText)
		{
			return Decrypt(Convert.FromBase64String(cipherText));
		}

		public string Decrypt(byte[] cipherTextBytes)
		{
			return Encoding.UTF8.GetString(DecryptToBytes(cipherTextBytes));
		}

		public byte[] DecryptToBytes(string cipherText)
		{
			return DecryptToBytes(Convert.FromBase64String(cipherText));
		}

		public byte[] DecryptToBytes(byte[] cipherTextBytes)
		{
			byte[] array = null;
			byte[] array2 = null;
			int num = 0;
			int num2 = 0;
			array = new byte[cipherTextBytes.Length];
			lock (this)
			{
				MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
				using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
				{
					num = cryptoStream.Read(array, 0, array.Length);
					cryptoStream.Close();
				}
				memoryStream.Close();
			}
			if (UseSalt())
			{
				num2 = (array[0] & 3) | (array[1] & 0xC) | (array[2] & 0x30) | (array[3] & 0xC0);
			}
			checked
			{
				array2 = new byte[num - num2];
				Array.Copy(array, num2, array2, 0, num - num2);
				return array2;
			}
		}

		public static int GetAESKeySize(string passPhrase)
		{
			switch (passPhrase.Length)
			{
				case 16:
					return 128;
				case 24:
					return 192;
				case 32:
					return 256;
				default:
					throw new NotSupportedException("ERROR: AES Password must be of 16, 24 or 32 bits length!");
			}
		}

		private bool UseSalt()
		{
			return Options.MaxSaltLength > 0 && Options.MaxSaltLength >= Options.MinSaltLength;
		}

		private byte[] AddSalt(byte[] plainTextBytes)
		{
			if (!UseSalt())
			{
				return plainTextBytes;
			}
			byte[] array = GenerateSalt(Options.MinSaltLength, Options.MaxSaltLength);
			byte[] array2 = new byte[checked(plainTextBytes.Length + array.Length)];
			Array.Copy(array, array2, array.Length);
			Array.Copy(plainTextBytes, 0, array2, array.Length, plainTextBytes.Length);
			return array2;
		}

		private byte[] GenerateSalt(int minSaltLen, int maxSaltLen)
		{
			int num = 0;
			num = ((minSaltLen != maxSaltLen) ? GenerateRandomNumber(minSaltLen, maxSaltLen) : minSaltLen);
			byte[] array = new byte[num];
			RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
			rNGCryptoServiceProvider.GetNonZeroBytes(array);
			checked
			{
				array[0] = (byte)((array[0] & 0xFC) | (num & 3));
				array[1] = (byte)((array[1] & 0xF3) | (num & 0xC));
				array[2] = (byte)((array[2] & 0xCF) | (num & 0x30));
				array[3] = (byte)((array[3] & 0x3F) | (num & 0xC0));
				return array;
			}
		}

		private int GenerateRandomNumber(int minValue, int maxValue)
		{
			byte[] array = new byte[4];
			RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
			rNGCryptoServiceProvider.GetBytes(array);
			int seed = ((array[0] & 0x7F) << 24) | (array[1] << 16) | (array[2] << 8) | array[3];
			Random random = new Random(seed);
			return random.Next(minValue, checked(maxValue + 1));
		}
	}
}
