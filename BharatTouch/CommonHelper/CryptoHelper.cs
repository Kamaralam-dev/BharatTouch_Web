using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Windows.Input;

/*
Specifically, We’re using:

Algorithm: AES(Advanced Encryption Standard)

Key size: 256 - bit(because your key is 32 characters, UTF - 8 encoded)

Mode: CBC(Cipher Block Chaining)

Padding: PKCS7

IV: Fixed, 16-byte string

In accurate way:

"We use AES-256 encryption in CBC mode with PKCS7 padding for data at rest, and TLS 1.2+ for data in transit."
That’s a solid, modern encryption method — though security auditors sometimes prefer a random IV per record to avoid pattern leakage.

 */

namespace BharatTouch.CommonHelper
{
    public static  class CryptoHelper
    {
        private static readonly string Key = "kamaralamsector26chandigarhphase"; // 32 chars for AES-256
        private static readonly string IV = "1234567890123456"; // 16 chars fixed IV

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return null;

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.IV = Encoding.UTF8.GetBytes(IV);
                aes.Mode = CipherMode.CBC; // CBC is fine with fixed IV for deterministic encryption
                aes.Padding = PaddingMode.PKCS7;

                var encryptor = aes.CreateEncryptor();
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                return Convert.ToBase64String(cipherBytes);
            }
        }

        public static string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText)) return null;

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.IV = Encoding.UTF8.GetBytes(IV);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                var decryptor = aes.CreateDecryptor();
                var cipherBytes = Convert.FromBase64String(encryptedText);
                var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

                return Encoding.UTF8.GetString(plainBytes);
            }
        }

        public static bool IsEncrypted(string value)
        {
            try
            {
                //byte[] data = Convert.FromBase64String(value);
                //if (data.Length < (16 + 16)) // salt + IV minimal size
                //    return false;

                // Try decrypting — if it fails, not encrypted
                CryptoHelper.Decrypt(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}