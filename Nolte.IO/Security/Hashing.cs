using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nolte.Security
{
    class Hashing
    {
        /// <summary>
        /// Returns the MD5 hash of the given string
        /// </summary>
        /// <param name="textToHash">The string which should be hashed</param>
        /// <returns>The MD5 sum</returns>
        public static string GetMd5(string textToHash)
        {
            if (string.IsNullOrEmpty(textToHash))
            {
                return string.Empty;
            }

            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var tempTextToHash = Encoding.Default.GetBytes(textToHash);
            var result = md5.ComputeHash(tempTextToHash);
            return BitConverter.ToString(result);
        }


        /// <summary>
        /// Generate a SHA1 hash
        /// </summary>
        /// <param name="originalString">The original string</param>
        /// <returns>SHA1 hash</returns>
        /// <exception cref="ArgumentNullException">This exception will be thrown 
        /// when the crypted string is null or empty.</exception>
        public static string SHA1(string originalString)
        {
            if (System.String.IsNullOrEmpty(originalString))
            {
                throw new ArgumentNullException("The string which needs to be encrypted can not be null.");
            }
            System.Security.Cryptography.SHA1 sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] textToHash = Encoding.Default.GetBytes(originalString);
            byte[] result = sha1.ComputeHash(textToHash);
            return System.Convert.ToBase64String(result);
        }
        
        /// <summary>
        /// Generate a Leetspeak string
        /// </summary>
        /// <param name="originalString">The original string</param>
        /// <returns>The leet string</returns>
        public static string Leet(string originalString)
        {
            string result = System.String.Empty;
            foreach (char c in originalString.ToLower())
            {
                switch (c)
                {
                    case 'a':
                        result += "4";
                        break;
                    case 'b':
                        result += "|3";
                        break;
                    case 'c':
                        result += "(";
                        break;
                    case 'd':
                        result += "|)";
                        break;
                    case 'e':
                        result += "3";
                        break;
                    case 'f':
                        result += "|\"";
                        break;
                    case 'g':
                        result += "6";
                        break;
                    case 'h':
                        result += "#";
                        break;
                    case 'i':
                        result += "!";
                        break;
                    case 'j':
                        result += "?";
                        break;
                    case 'k':
                        result += "|{";
                        break;
                    case 'l':
                        result += "1";
                        break;
                    case 'm':
                        result += @"|\/|";
                        break;
                    case 'n':
                        result += "/V";
                        break;
                    case 'o':
                        result += "0";
                        break;
                    case 'p':
                        result += "|*";
                        break;
                    case 'q':
                        result += "O_";
                        break;
                    case 'r':
                        result += "R";
                        break;
                    case 's':
                        result += "5";
                        break;
                    case 't':
                        result += "7";
                        break;
                    case 'u':
                        result += "µ";
                        break;
                    case 'v':
                        result += @"\/";
                        break;
                    case 'w':
                        result += @"\A/";
                        break;
                    case 'x':
                        result += "%";
                        break;
                    case 'y':
                        result += "°/";
                        break;
                    case 'z':
                        result += "2";
                        break;
                    case '1':
                        result += "eins";
                        break;
                    case '2':
                        result += "zwei";
                        break;
                    case '3':
                        result += "drei";
                        break;
                    case '4':
                        result += "vier";
                        break;
                    case '5':
                        result += "fünf";
                        break;
                    case '6':
                        result += "sechs";
                        break;
                    case '7':
                        result += "sieben";
                        break;
                    case '8':
                        result += "acht";
                        break;
                    case '9':
                        result += "neun";
                        break;
                    case '0':
                        result += "null";
                        break;
                    default:
                        result += c.ToString();
                        break;
                }
            }
            return result;
        }
    }
}
