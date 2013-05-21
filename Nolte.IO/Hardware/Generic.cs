using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Nolte
{
    public static partial class Hardware
    {
        public static partial class Helper
        {
            /// <summary>
            /// Generates a md5 hash from an given string
            /// </summary>
            /// <param name="TextToHash">the text to create a hash from</param>
            /// <returns>the generated hash</returns>
            internal static string MD5(string TextToHash)
            {
                if ((TextToHash == null) || (TextToHash.Length == 0))
                {
                    return string.Empty;
                }
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] textToHash = Encoding.Default.GetBytes(TextToHash);
                byte[] result = md5.ComputeHash(textToHash);
                return System.BitConverter.ToString(result).Replace("-", "");
            }
        }
    }
}