using UnityEngine;
//using MadPike.Security;
using System.Security.Cryptography;
using System.Text;

public static class SecurePlayerPrefs
{
	public static void SetString(string key, string value)
	{
		string password = "portbliss";
		var desEncryption = new DESEncryption();
		string hashedKey = GenerateMD5(key);
		string encryptedValue = desEncryption.Encrypt(value, password);
		PlayerPrefs.SetString(hashedKey, encryptedValue);

		PlayerPrefs.Save();
	}
	
	public static string GetString(string key)
	{
		string password = "portbliss";
		string hashedKey = GenerateMD5(key);
		if (PlayerPrefs.HasKey(hashedKey))
		{
			var desEncryption = new DESEncryption();
			string encryptedValue = PlayerPrefs.GetString(hashedKey);
			string decryptedValue;
			desEncryption.TryDecrypt(encryptedValue, password, out decryptedValue);
			return decryptedValue;
		}
		else
		{
			return "";
		}
	}
	
	public static string GetString(string key, string defaultValue)
	{
		if (HasKey(key))
		{
			string s = GetString(key);
			return s==""?defaultValue:s;
		}
		else
		{
			return defaultValue;
		}
	}
	
	public static bool HasKey(string key)
	{
		string hashedKey = GenerateMD5(key);
		bool hasKey = PlayerPrefs.HasKey(hashedKey);
		return hasKey;
	}

	public static void DeleteKey(string key)
	{
		string hashedKey = GenerateMD5(key);
		PlayerPrefs.DeleteKey(hashedKey);
	}
	
	/// <summary>
	/// Generates an MD5 hash of the given text.
	/// WARNING. Not safe for storing passwords
	/// </summary>
	/// <returns>MD5 Hashed string</returns>
	/// <param name="text">The text to hash</param>
	static string GenerateMD5(string text)
	{
		var md5 = MD5.Create();
		byte[] inputBytes = Encoding.UTF8.GetBytes(text);
		byte[] hash = md5.ComputeHash(inputBytes);
		
		// step 2, convert byte array to hex string
		var sb = new StringBuilder();
		for (int i = 0; i < hash.Length; i++)
		{
			sb.Append(hash[i].ToString("X2"));
		}
		return sb.ToString();
	}
}
