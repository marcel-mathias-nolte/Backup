/* Copyright (c) 2008-2009 Peter Palotas
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 */
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using File = Alphaleonis.Win32.Filesystem.File;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using Path = Alphaleonis.Win32.Filesystem.Path;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlphaFS.UnitTest
{
   /// <summary>This is a test class for FileInfo and is intended to contain all FileInfo UnitTests.</summary>
   [TestClass]
   public class FileInfoTest
   {
      #region FileInfoTest Helpers

      private static readonly string SysDrive = Environment.GetEnvironmentVariable("SystemDrive");
      private static readonly string SysRoot = Environment.GetEnvironmentVariable("SystemRoot");
      private static readonly string SysRoot32 = Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "System32");
      private const string TextTrue = "IsTrue";
      private const string TextFalse = "IsFalse";
      private const string TextHelloWorld = "Hëllõ Wørld!";
      private static Stopwatch _stopWatcher;

      private static string StopWatcher(bool start = false)
      {
         if (_stopWatcher == null)
            _stopWatcher = new Stopwatch();

         if (start)
         {
            _stopWatcher.Restart();
            return null;
         }

         _stopWatcher.Stop();
         long ms = _stopWatcher.ElapsedMilliseconds;
         TimeSpan elapsed = _stopWatcher.Elapsed;

         return string.Format(CultureInfo.CurrentCulture, "*Duration: {0, 4} ms. ({1})", ms, elapsed);
      }
      private static string Reporter()
      {
         Win32Exception lastError = new Win32Exception();

         StopWatcher();

         return string.Format(CultureInfo.CurrentCulture, "\t\t{0}\t*Win32 Result: {1, 4}\t*Win32 Message: [{2}]", StopWatcher(), lastError.NativeErrorCode, lastError.Message);
      }

      /// <summary>Shows the Object's available Properties and Values.</summary>
      private static bool Dump(object obj, int width = -35)
      {
         int cnt = 0;
         const string nulll = "\t\tnull";
         string template = "\t\t#{0:000}\t{1, " + width + "} ==\t[{2}]";

         if (obj == null)
         {
            Console.WriteLine(nulll);
            return false;
         }

         Console.WriteLine("\n\t\tPointer object to: [{0}]\n", obj.GetType().FullName);

         bool loopOk = false;
         foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj).Sort().Cast<PropertyDescriptor>().Where(descriptor => descriptor != null))
         {
            string propValue = null;
            try
            {
               object value = descriptor.GetValue(obj);
               if (value != null)
                  propValue = value.ToString();

               loopOk = true;
            }
            catch (Exception ex)
            {
               // Please do tell, oneliner preferably.
               propValue = ex.Message.Replace(Environment.NewLine, string.Empty);
            }

            Console.WriteLine(template, ++cnt, descriptor.Name, propValue);
         }

         return loopOk;
      }
      private static byte[] StringToByteArray(string str, params Encoding[] encoding)
      {
         Encoding encode = encoding != null && encoding.Any() ? encoding[0] : new UTF8Encoding(true, true);
         return encode.GetBytes(str);
      }

      private static void CompareFileInfos(System.IO.FileInfo expected, FileInfo actual)
      {
         int cnt = -1;
         while (cnt != 15)
         {
            cnt++;
            try
            {
               // Compare values of both instances.
               switch (cnt)
               {
                  case 0: Assert.AreEqual(expected.Attributes, actual.Attributes, "Mismatch: [Attributes]"); break;
                  case 1: Assert.AreEqual(expected.CreationTime, actual.CreationTime, "Mismatch: [CreationTime]"); break;
                  case 2: Assert.AreEqual(expected.CreationTimeUtc, actual.CreationTimeUtc, "Mismatch: [CreationTimeUtc]"); break;
                  case 3: Assert.AreEqual(expected.Directory.ToString(), actual.Directory.ToString(), "Mismatch: [Directory]"); break;
                  case 4: Assert.AreEqual(expected.DirectoryName, actual.DirectoryName, "Mismatch: [DirectoryName]"); break;
                  case 5: Assert.AreEqual(expected.Exists, actual.Exists, "Mismatch: [Exists]"); break;
                  case 6: Assert.AreEqual(expected.Extension, actual.Extension, "Mismatch: [Extension]"); break;
                  case 7: Assert.AreEqual(expected.FullName, actual.FullName, "Mismatch: [FullName]"); break;
                  case 8: Assert.AreEqual(expected.IsReadOnly, actual.IsReadOnly, "Mismatch: [IsReadOnly]"); break;
                  case 9: Assert.AreEqual(expected.LastAccessTime, actual.LastAccessTime, "Mismatch: [LastAccessTime]"); break;
                  case 10: Assert.AreEqual(expected.LastAccessTimeUtc, actual.LastAccessTimeUtc, "Mismatch: [LastAccessTimeUtc]"); break;
                  case 11: Assert.AreEqual(expected.LastWriteTime, actual.LastWriteTime, "Mismatch: [LastWriteTime]"); break;
                  case 12: Assert.AreEqual(expected.LastWriteTimeUtc, actual.LastWriteTimeUtc, "Mismatch: [LastWriteTimeUtc]"); break;
                  case 13: Assert.AreEqual(expected.Length, actual.Length, "Mismatch: [Length]"); break;
                  case 14: Assert.AreEqual(expected.Name, actual.Name, "Mismatch: [Name]"); break;
               }
            }
            catch (Exception ex)
            {
               Console.WriteLine("\t\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));
            }
         }
      }

      private static bool DumpFileInfo(string path)
      {
         bool allOk = true;
         int errorCnt = 0;

         try
         {
            StopWatcher(true);
            System.IO.FileInfo expected = new System.IO.FileInfo(path);
            Console.WriteLine(Reporter());

            StopWatcher(true);
            FileInfo actual = new FileInfo(path);
            Console.WriteLine(Reporter());

            Dump(expected, -17);
            Dump(actual, -17);

            // Compare values of both instances.
            CompareFileInfos(expected, actual);

            FileInfo fileInfo2 = new FileInfo(path);
            FileInfo clone = actual;

            int getHashCode1 = actual.GetHashCode();
            int getHashCode2 = clone.GetHashCode();
            int getHashCode3 = fileInfo2.GetHashCode();

            bool isTrue1 = clone.Equals(actual);
            bool isTrue2 = clone == actual;
            bool isTrue3 = !(clone != actual);
            bool isTrue4 = actual == fileInfo2;
            bool isTrue6 = actual.Equals(fileInfo2);
            bool isTrue5 = !(actual != fileInfo2);

            Console.WriteLine("\n\t\t fileInfo.GetHashCode() = [{0}]", getHashCode1);
            Console.WriteLine("\t\t    clone.GetHashCode() = [{0}]", getHashCode2);
            Console.WriteLine("\t\tfileInfo2.GetHashCode() = [{0}]\n", getHashCode3);
            Console.WriteLine("\t\t obj clone.Equals()   == [{0}] : {1}", TextTrue, isTrue1);
            Console.WriteLine("\t\t obj clone ==         == [{0}] : {1}", TextTrue, isTrue2);
            Console.WriteLine("\t\t obj clone !=         == [{0}]: {1}\n", TextFalse, isTrue3);
            Console.WriteLine("\t\tfileInfo == fileInfo2      == [{0}]: {1}", TextFalse, isTrue4);
            Console.WriteLine("\t\tfileInfo.Equals(fileInfo2) == [{0}]: {1}", TextFalse, isTrue6);
            Console.WriteLine("\t\tfileInfo != fileInfo2      == [{0}] : {1}\n", TextTrue, isTrue5);

            Assert.IsTrue(isTrue1);
            Assert.IsTrue(isTrue2);
            Assert.IsTrue(isTrue3);
            Assert.IsTrue(isTrue4);
            Assert.IsTrue(isTrue6);
            Assert.IsTrue(isTrue5);

            return true;
         }

         catch (FileNotFoundException) { }
         catch (IOException) { }
         catch (Exception ex)
         {
            Console.WriteLine("\n\t\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));

            // Exception to the Exception; System.IO.GetParent() handles this but throws Exception.
            if (path != null && !path.StartsWith(@"\\?\GlobalRoot", StringComparison.OrdinalIgnoreCase))
            {
               allOk = false;
               errorCnt++;
            }
         }

         if (!allOk)
            Console.WriteLine("\n\tEncountered: [{0}] FileInfo Properties where AlphaFS != System.IO", errorCnt);

         return allOk;
      }

      #endregion // FileInfoTest Helpers

      #region .NET

      #region FileInfo

      [TestMethod]
      public void __Class_FileInfo()
      {
         Console.WriteLine("FileInfo() Class");
         
         bool allOk = true;

         try
         {
            string inputPath = ".";
            FileInfo actual = new FileInfo(inputPath);
            System.IO.FileInfo expected = new System.IO.FileInfo(inputPath);
            string currDirActual = Directory.GetCurrentDirectory();
            string currDirExpected = System.IO.Directory.GetCurrentDirectory();
            Console.WriteLine("\n\tCurrent directory: [{0}]\n\tInput Path: [{1}]\tName is: [{2}]\t\tSystem.IO: [{3}]", currDirActual, inputPath, actual.Name, expected.Name);
            Assert.AreEqual(currDirActual, currDirExpected);

            // Compare values of both instances.
            CompareFileInfos(expected, actual);
         }
         catch (Exception ex)
         {
            Console.WriteLine("\n\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));
         }

         #region Non existing file

         string path = @"N:\nonexisting\ccc.txt";

         #region Local

         Console.WriteLine("\n\tLocal non existing file: [{0}]", path);
         if (!DumpFileInfo(path))
            allOk = false;

         #endregion // Local

         #region Network

         path = Path.LocalToUnc(path);
         Console.WriteLine("\n\tNetwork non existing file: [{0}]", path);
         if (!DumpFileInfo(path))
            allOk = false;

         #endregion // Network

         #endregion // Non existing file

         #region Existing file

         path = Path.Combine(SysRoot32, "notepad.exe");

         #region Local

         Console.WriteLine("\n\tLocal existing file: [{0}]", path);
         if (!DumpFileInfo(path))
            allOk = false;

         #endregion //Local

         #region Network

         path = Path.LocalToUnc(path);
         if (File.Exists(path))
         {
            Console.WriteLine("\n\tNetwork existing file: [{0}]", path);
            if (!DumpFileInfo(path))
               allOk = false;
         }
         else
            Assert.IsTrue(false, "Share inaccessible: {0}", path);

         #endregion // Network

         #endregion // Existing file

         Assert.AreEqual(true, allOk, "Encountered FileInfo Properties where AlphaFS != System.IO");
      }

      #endregion // FileInfo

      #endregion // .NET
   }
}