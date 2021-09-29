using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace app.domain.Utilities
{
    //public static class Cipher
    //{
    //    public static string EncryptString(string sector, string text)
    //    {
    //        if (string.IsNullOrEmpty(text))
    //            return string.Empty;

    //        string _passPhrase = string.Empty;
    //        if (sector == "register")
    //            _passPhrase = "mVdizkQWgsHq0SB";
    //        else
    //        if (sector == "resetPublic")
    //            _passPhrase = "XFhkQKu759PbeEy";
    //        else
    //         if (sector == "reset")
    //            _passPhrase = "UC4MccaKx0bwW89";
    //        else
    //        if (sector == "role")
    //            _passPhrase = "nGX3ADjItIAAAKf";


    //        int _Keysize = 128;
    //        string salt128 = "tWhmqNDFcEFwTHTl";
    //        int DerivationIterations = 1000;
    //        byte[] saltStringBytes = Encoding.UTF8.GetBytes(salt128);
    //        byte[] ivStringBytes = Encoding.UTF8.GetBytes("RFPsRfeoWVbsgSLp");


    //        var plainTextBytes = Encoding.UTF8.GetBytes(text);

    //        using (var password = new Rfc2898DeriveBytes(_passPhrase, saltStringBytes, DerivationIterations))
    //        {
    //            var keyBytes = password.GetBytes(_Keysize / 8);
    //            using (var symmetricKey = new RijndaelManaged())
    //            {
    //                symmetricKey.BlockSize = _Keysize;
    //                symmetricKey.Mode = CipherMode.CBC;
    //                symmetricKey.Padding = PaddingMode.PKCS7;
    //                using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
    //                {
    //                    using (var memoryStream = new MemoryStream())
    //                    {
    //                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
    //                        {
    //                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
    //                            cryptoStream.FlushFinalBlock();
    //                            // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
    //                            var cipherTextBytes = saltStringBytes;
    //                            cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
    //                            cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
    //                            memoryStream.Close();
    //                            cryptoStream.Close();
    //                            return Convert.ToBase64String(cipherTextBytes);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    public static string DecryptString(string sector, string text)
    //    {
    //        if (string.IsNullOrEmpty(text))
    //            return string.Empty;

    //        byte[] cipherTextBytesWithSaltAndIv = Convert.FromBase64String(text);

    //        string _passPhrase = string.Empty;
    //        if (sector == "register")
    //            _passPhrase = "mVdizkQWgsHq0SB";
    //        else
    //        if (sector == "resetPublic")
    //            _passPhrase = "XFhkQKu759PbeEy";
    //        else
    //         if (sector == "reset")
    //            _passPhrase = "UC4MccaKx0bwW89";
    //        else
    //        if (sector == "role")
    //            _passPhrase = "nGX3ADjItIAAAKf";

    //        int _Keysize = 128;
    //        string salt128 = "tWhmqNDFcEFwTHTl";
    //        int DerivationIterations = 1000;

    //        byte[] saltStringBytes = Encoding.UTF8.GetBytes(salt128);
    //        byte[] ivStringBytes = Encoding.UTF8.GetBytes("RFPsRfeoWVbsgSLp");

    //        var v = Encoding.UTF8.GetString(cipherTextBytesWithSaltAndIv.Take(_Keysize / 8).ToArray());
    //        if (v != salt128)
    //            return string.Empty;

    //        var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((_Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((_Keysize / 8) * 2)).ToArray();

    //        using (var password = new Rfc2898DeriveBytes(_passPhrase, saltStringBytes, DerivationIterations))
    //        {
    //            var keyBytes = password.GetBytes(_Keysize / 8);
    //            using (var symmetricKey = new RijndaelManaged())
    //            {
    //                symmetricKey.Mode = CipherMode.CBC;
    //                symmetricKey.Padding = PaddingMode.PKCS7;
    //                symmetricKey.BlockSize = _Keysize;

    //                using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
    //                {
    //                    using (var memoryStream = new MemoryStream(cipherTextBytes))
    //                    {
    //                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
    //                        {
    //                            var plainTextBytes = new byte[cipherTextBytes.Length];
    //                            var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
    //                            memoryStream.Close();
    //                            cryptoStream.Close();
    //                            return Convert.ToBase64String(plainTextBytes);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
}
