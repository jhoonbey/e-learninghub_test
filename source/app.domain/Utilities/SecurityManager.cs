using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace app.domain.Utilities
{
    public class SecurityManager
    {

        //// This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        //// 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        //private const string initVector = "OMgmTlljvzIvLkNt";
        //// This constant is used to determine the keysize of the encryption algorithm
        //private const int keysize = 256;
        ////Encrypt
        //public static string EncryptString(string sector, string text)
        //{
        //    string encryptionKey = string.Empty;

        //    if (sector == "register")
        //        encryptionKey = "mVdizkQWgsHq0SB";
        //    else
        //    if (sector == "resetPublic")
        //        encryptionKey = "XFhkQKu759PbeEy";
        //    else
        //     if (sector == "reset")
        //        encryptionKey = "UC4MccaKx0bwW89";
        //    else
        //    if (sector == "role")
        //        encryptionKey = "nGX3ADjItIAAAKf";

        //    byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
        //    byte[] plainTextBytes = Encoding.UTF8.GetBytes(text);
        //    PasswordDeriveBytes password = new PasswordDeriveBytes(encryptionKey, null);
        //    byte[] keyBytes = password.GetBytes(keysize / 8);
        //    RijndaelManaged symmetricKey = new RijndaelManaged
        //    {
        //        Mode = CipherMode.CBC
        //    };
        //    ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
        //    MemoryStream memoryStream = new MemoryStream();
        //    CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        //    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
        //    cryptoStream.FlushFinalBlock();
        //    byte[] cipherTextBytes = memoryStream.ToArray();
        //    memoryStream.Close();
        //    cryptoStream.Close();
        //    return Convert.ToBase64String(cipherTextBytes);
        //}
        ////Decrypt
        //public static string DecryptString(string sector, string text)
        //{
        //    string encryptionKey = string.Empty;

        //    if (sector == "register")
        //        encryptionKey = "mVdizkQWgsHq0SB";
        //    else
        //    if (sector == "resetPublic")
        //        encryptionKey = "XFhkQKu759PbeEy";
        //    else
        //     if (sector == "reset")
        //        encryptionKey = "UC4MccaKx0bwW89";
        //    else
        //    if (sector == "role")
        //        encryptionKey = "nGX3ADjItIAAAKf";

        //    byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
        //    byte[] cipherTextBytes = Convert.FromBase64String(text);
        //    PasswordDeriveBytes password = new PasswordDeriveBytes(encryptionKey, null);
        //    byte[] keyBytes = password.GetBytes(keysize / 8);
        //    RijndaelManaged symmetricKey = new RijndaelManaged
        //    {
        //        Mode = CipherMode.CBC
        //    };
        //    ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
        //    MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
        //    CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        //    byte[] plainTextBytes = new byte[cipherTextBytes.Length];
        //    int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
        //    memoryStream.Close();
        //    cryptoStream.Close();
        //    return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        //}



        //public static string EncryptString(string sector, string text)
        //{
        //    string encryptionKey = string.Empty;

        //    if (sector == "register")
        //        encryptionKey = "mVdizkQWgsHq0SB";
        //    else if (sector == "resetPublic")
        //        encryptionKey = "XFhkQKu759PbeEy";
        //    else if (sector == "reset")
        //        encryptionKey = "UC4MccaKx0bwW89";
        //    else if (sector == "role")
        //        encryptionKey = "nGX3ADjItIAAAKf";

        //    var _key = Encoding.UTF8.GetBytes(encryptionKey);


        //    using (var aes = Aes.Create())
        //    {
        //        using (var encryptor = aes.CreateEncryptor(_key, aes.IV))
        //        {
        //            using (var ms = new MemoryStream())
        //            {
        //                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        //                {
        //                    using (var sw = new StreamWriter(cs))
        //                    {
        //                        sw.Write(text);
        //                    }
        //                }
        //                var iv = aes.IV;
        //                var encrypted = ms.ToArray();
        //                var result = new byte[iv.Length + encrypted.Length];
        //                Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        //                Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length);

        //                return Convert.ToBase64String(result);
        //            }
        //        }
        //    }
        //}

        //public static string DecryptString(string sector, string encrypted)
        //{
        //    string encryptionKey = string.Empty;

        //    if (sector == "register")
        //        encryptionKey = "mVdizkQWgsHq0SB";
        //    else if (sector == "resetPublic")
        //        encryptionKey = "XFhkQKu759PbeEy";
        //    else if (sector == "reset")
        //        encryptionKey = "UC4MccaKx0bwW89";
        //    else if (sector == "role")
        //        encryptionKey = "nGX3ADjItIAAAKf";


        //    var _key = Encoding.UTF8.GetBytes(encryptionKey);
        //    var b = Convert.FromBase64String(encrypted);
        //    var iv = new byte[16];
        //    var cipher = new byte[16];

        //    Buffer.BlockCopy(b, 0, iv, 0, iv.Length);
        //    Buffer.BlockCopy(b, iv.Length, cipher, 0, iv.Length);

        //    using (var aes = Aes.Create())
        //    {
        //        using (var decryptor = aes.CreateDecryptor(_key, iv))
        //        {
        //            var result = string.Empty;
        //            using (var ms = new MemoryStream(cipher))
        //            {
        //                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
        //                {
        //                    using (var sr = new StreamReader(cs))
        //                    {
        //                        result = sr.ReadToEnd();
        //                    }
        //                }
        //            }
        //            return result;
        //        }
        //    }
        //}

        //public static string EncryptString(string sector, string text)
        //{
        //    string encryptionKey = string.Empty;

        //    if (sector == "register")
        //        encryptionKey = "mVdizkQWgsHq0SB";
        //    else if (sector == "resetPublic")
        //        encryptionKey = "XFhkQKu759PbeEy";
        //    else if (sector == "reset")
        //        encryptionKey = "UC4MccaKx0bwW89";
        //    else if (sector == "role")
        //        encryptionKey = "nGX3ADjItIAAAKf";

        //    var key = Encoding.UTF8.GetBytes(encryptionKey);

        //    using (var aesAlg = Aes.Create())
        //    {
        //        using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
        //        {
        //            using (var msEncrypt = new MemoryStream())
        //            {
        //                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        //                using (var swEncrypt = new StreamWriter(csEncrypt))
        //                {
        //                    swEncrypt.Write(text);
        //                }

        //                var iv = aesAlg.IV;

        //                var decryptedContent = msEncrypt.ToArray();

        //                var result = new byte[iv.Length + decryptedContent.Length];

        //                Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        //                Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

        //                return Convert.ToBase64String(result);
        //            }
        //        }
        //    }
        //}

        //public static string DecryptString(string sector, string text)
        //{
        //    string encryptionKey = string.Empty;

        //    if (sector == "register")
        //        encryptionKey = "mVdizkQWgsHq0SB";
        //    else if (sector == "resetPublic")
        //        encryptionKey = "XFhkQKu759PbeEy";
        //    else if (sector == "reset")
        //        encryptionKey = "UC4MccaKx0bwW89";
        //    else if (sector == "role")
        //        encryptionKey = "nGX3ADjItIAAAKf";

        //    var fullCipher = Convert.FromBase64String(text);

        //    var iv = new byte[16];
        //    var cipher = new byte[16];

        //    Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
        //    Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
        //    var key = Encoding.UTF8.GetBytes(encryptionKey);

        //    using (var aesAlg = Aes.Create())
        //    {
        //        using (var decryptor = aesAlg.CreateDecryptor(key, iv))
        //        {
        //            string result;
        //            using (var msDecrypt = new MemoryStream(cipher))
        //            {
        //                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        //                {
        //                    using (var srDecrypt = new StreamReader(csDecrypt))
        //                    {
        //                        result = srDecrypt.ReadToEnd();
        //                    }
        //                }
        //            }

        //            return result;
        //        }
        //    }
        //}
    }
}
