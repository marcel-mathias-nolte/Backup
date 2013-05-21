//-----------------------------------------------------------------------
// <copyright file="Hash.cs" company="RAN COMMUNITY SERVER">
//     (C)2010-2013 Marcel Nolte
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using System.Security.Cryptography;

namespace Nolte.Security
{
    using System;
    using System.Text;

    /// <summary>
    /// Provides hashing functionality
    /// </summary>
    public class Cryptography
    {
        protected byte[] CryptoKey = ASCIIEncoding.ASCII.GetBytes(Environment.MachineName);

        /// <summary>
        /// Set the cryptographic key
        /// </summary>
        /// <param name="Key">The crypto key to use.</param>
        /// <exception cref="ArgumentNullException">This exception will be 
        /// thrown when the key is null or empty.</exception>
        public void SetCryptoKey(string Key)
        {
            if (System.String.IsNullOrEmpty(Key))
            {
                throw new ArgumentNullException("The key can not be null or empty.");
            }
            while (Key.Length < 16)
                Key += Key;
            byte[] t = ASCIIEncoding.ASCII.GetBytes(Key);
            CryptoKey = new byte[16];
            Array.Copy(t, CryptoKey, 16);
        }
        
        public System.String DecryptRJ256(string cypher, string key)
        {
            if (System.String.IsNullOrWhiteSpace(cypher)) return "";
            var iv = "45287112549354892144548565456541";
            var sRet = "";

            var encoding = new UTF8Encoding();
            var Key = new byte[32];
            Array.Copy(encoding.GetBytes(key), Key, 32);
            var IV = encoding.GetBytes(iv);

            using (var rj = new RijndaelManaged())
            {
                try
                {
                    rj.Padding = PaddingMode.Zeros;
                    rj.Mode = CipherMode.CBC;
                    rj.KeySize = 256;
                    rj.BlockSize = 256;
                    rj.Key = Key;
                    rj.IV = IV;
                    var ms = new MemoryStream(Convert.FromBase64String(cypher));

                    using (var cs = new CryptoStream(ms, rj.CreateDecryptor(Key, IV), CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs))
                        {
                            sRet = sr.ReadLine().Trim('\0');
                        }
                    }
                }
                finally
                {
                    rj.Clear();
                }
            }

            return sRet;
        }

        /// <summary>
        /// Encrypt a string using DES.
        /// </summary>
        /// <param name="originalString">The original string.</param>
        /// <returns>The encrypted string.</returns>
        /// <exception cref="ArgumentNullException">This exception will be 
        /// thrown when the original string is null or empty.</exception>
        public string Encrypt(string originalString)
        {
            if (BypassCryptography)
            {
                return originalString;
            }
            if (System.String.IsNullOrEmpty(originalString))
            {
                throw new ArgumentNullException("The string which needs to be encrypted can not be null.");
            }
            AesCryptoServiceProvider cryptoProvider = new AesCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateEncryptor(CryptoKey, CryptoKey), CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter(cryptoStream);
            writer.Write(originalString);
            writer.Flush();
            cryptoStream.FlushFinalBlock();
            writer.Flush();
            return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }

        /// <summary>
        /// Encrypt a string using DES.
        /// </summary>
        /// <param name="originalString">The original string.</param>
        /// <param name="Key">The crypto key.</param>
        /// <returns>The encrypted string.</returns>
        /// <exception cref="ArgumentNullException">This exception will be 
        /// thrown when the original string or the key is null or empty.</exception>
        public string Encrypt(string originalString, string Key)
        {
            if (BypassCryptography)
            {
                return originalString;
            }
            if (System.String.IsNullOrEmpty(originalString) || System.String.IsNullOrEmpty(Key))
            {
                throw new ArgumentNullException("The string which needs to be encrypted and the key can not be null.");
            }
            AesCryptoServiceProvider cryptoProvider = new AesCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateEncryptor(ASCIIEncoding.ASCII.GetBytes(Key), ASCIIEncoding.ASCII.GetBytes(Key)), CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter(cryptoStream);
            writer.Write(originalString);
            writer.Flush();
            cryptoStream.FlushFinalBlock();
            writer.Flush();
            return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }

        /// <summary>
        /// Decrypt a DES crypted string.
        /// </summary>
        /// <param name="cryptedString">The crypted string.</param>
        /// <returns>The decrypted string.</returns>
        /// <exception cref="ArgumentNullException">This exception will be thrown 
        /// when the crypted string is null or empty.</exception>
        public string Decrypt(string cryptedString)
        {
            if (BypassCryptography)
            {
                return cryptedString;
            }
            if (System.String.IsNullOrEmpty(cryptedString))
            {
                throw new ArgumentNullException("The string which needs to be decrypted can not be null.");
            }
            AesCryptoServiceProvider cryptoProvider = new AesCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(cryptedString));
            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(CryptoKey, CryptoKey), CryptoStreamMode.Read);
            StreamReader reader = new StreamReader(cryptoStream);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Decrypt a DES crypted string.
        /// </summary>
        /// <param name="cryptedString">The crypted string.</param>
        /// <param name="Key">The crypto key.</param>
        /// <returns>The decrypted string.</returns>
        /// <exception cref="ArgumentNullException">This exception will be thrown 
        /// when the crypted string or the key is null or empty.</exception>
        public string Decrypt(string cryptedString, string Key)
        {
            if (BypassCryptography)
            {
                return cryptedString;
            }
            if (System.String.IsNullOrEmpty(cryptedString) || System.String.IsNullOrEmpty(Key))
            {
                throw new ArgumentNullException("The string which needs to be decrypted and the key can not be null.");
            }
            AesCryptoServiceProvider cryptoProvider = new AesCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(cryptedString));
            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(ASCIIEncoding.ASCII.GetBytes(Key), ASCIIEncoding.ASCII.GetBytes(Key)), CryptoStreamMode.Read);
            StreamReader reader = new StreamReader(cryptoStream);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Enable encryption/decryption bypass for testing purposes
        /// </summary>
        public static bool BypassCryptography = false;

    }
}
