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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace AlphaFS.UnitTest
{
   /// <summary>This is a test class for DirectoryInfo and is intended to contain all DirectoryInfo Unit Tests.</summary>
   [TestClass]
   public class DirectoryInfoTest
   {
      #region DirectoryInfoTest Helpers

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

      private static void CompareDirectoryInfos(System.IO.DirectoryInfo expected, DirectoryInfo actual)
      {
         int cnt = -1;
         while (cnt != 13)
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
                  case 3: Assert.AreEqual(expected.Exists, actual.Exists, "Mismatch: [Exists]"); break;
                  case 4: Assert.AreEqual(expected.Extension, actual.Extension, "Mismatch: [Extension]"); break;
                  case 5: Assert.AreEqual(expected.FullName, actual.FullName, "Mismatch: [FullName]"); break;
                  case 6: Assert.AreEqual(expected.LastAccessTime, actual.LastAccessTime, "Mismatch: [LastAccessTime]"); break;
                  case 7: Assert.AreEqual(expected.LastAccessTimeUtc, actual.LastAccessTimeUtc, "Mismatch: [LastAccessTimeUtc]"); break;
                  case 8: Assert.AreEqual(expected.LastWriteTime, actual.LastWriteTime, "Mismatch: [LastWriteTime]"); break;
                  case 9: Assert.AreEqual(expected.LastWriteTimeUtc, actual.LastWriteTimeUtc, "Mismatch: [LastWriteTimeUtc]"); break;
                  case 10: Assert.AreEqual(expected.Name, actual.Name, "Mismatch: [Name]"); break;
                  case 11: Assert.AreEqual(expected.Parent.ToString(), actual.Parent.ToString(), "Mismatch: [Parent.ToString()]"); break;
                  case 12: Assert.AreEqual(expected.Root.ToString(), actual.Root.ToString(), "Mismatch: [Root.ToString()]"); break;
               }
            }
            catch (Exception ex)
            {
               Console.WriteLine("\t\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));
            }
         }
      }

      private static bool DumpDirectoryInfo(string path)
      {
         bool allOk = true;
         int errorCnt = 0;

         try
         {
            StopWatcher(true);
            System.IO.DirectoryInfo expected = new System.IO.DirectoryInfo(path);
            Console.WriteLine(Reporter());

            StopWatcher(true);
            DirectoryInfo actual = new DirectoryInfo(path);
            Console.WriteLine(Reporter());

            Dump(expected, -17);
            Dump(actual, -19);

            // Compare values of both instances.
            CompareDirectoryInfos(expected, actual);

            DirectoryInfo directoryInfo2 = new DirectoryInfo(path);
            DirectoryInfo clone = actual;

            int getHashCode1 = actual.GetHashCode();
            int getHashCode2 = clone.GetHashCode();
            int getHashCode3 = directoryInfo2.GetHashCode();

            // Need to check letterString with VolumeSeparatorChar.
            //bool isString = !String.IsNullOrEmpty(clone.ToString()) && clone.Name.Equals(drive + Path.VolumeSeparatorChar);
            bool isTrue1 = clone.Equals(actual);
            bool isTrue2 = clone == actual;
            bool isTrue3 = !(clone != actual);
            bool isTrue4 = actual == directoryInfo2;
            bool isTrue6 = actual.Equals(directoryInfo2);
            bool isTrue5 = !(actual != directoryInfo2);

            Console.WriteLine("\n\t\t directoryInfo.GetHashCode() = [{0}]", getHashCode1);
            Console.WriteLine("\t\t\t  clone.GetHashCode() = [{0}]", getHashCode2);
            Console.WriteLine("\t\tdirectoryInfo2.GetHashCode() = [{0}]\n", getHashCode3);

            Console.WriteLine("\t\t obj clone.Equals()   == [{0}] : {1}", TextTrue, isTrue1);
            Console.WriteLine("\t\t obj clone ==         == [{0}] : {1}", TextTrue, isTrue2);
            Console.WriteLine("\t\t obj clone !=         == [{0}]: {1}\n", TextFalse, isTrue3);

            Console.WriteLine("\t\tdriveInfo == driveInfo2      == [{0}]: {1}", TextFalse, isTrue4);
            Console.WriteLine("\t\tdriveInfo.Equals(driveInfo2) == [{0}]: {1}", TextFalse, isTrue6);
            Console.WriteLine("\t\tdriveInfo != driveInfo2      == [{0}] : {1}\n", TextTrue, isTrue5);

            Assert.IsTrue(isTrue1);
            Assert.IsTrue(isTrue2);
            Assert.IsTrue(isTrue3);
            Assert.IsTrue(isTrue4);
            Assert.IsTrue(isTrue6);
            Assert.IsTrue(isTrue5);

            return true;
         }
         catch (DirectoryNotFoundException) { }
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
            Console.WriteLine("\n\tEncountered: [{0}] DirectoryInfo Properties where AlphaFS != System.IO", errorCnt);

         return allOk;
      }

      #endregion // directoryInfoTest Helpers

      #region .NET

      #region DirectoryInfo

      [TestMethod]
      public void __Class_DirectoryInfo()
      {
         Console.WriteLine("DirectoryInfo() Class");

         bool allOk = true;

         try
         {
            string inputPath = ".";
            DirectoryInfo actual = new DirectoryInfo(inputPath);
            System.IO.DirectoryInfo expected = new System.IO.DirectoryInfo(inputPath);
            string currDirActual = Directory.GetCurrentDirectory();
            string currDirExpected = System.IO.Directory.GetCurrentDirectory();
            Console.WriteLine("\n\tCurrent directory: [{0}]\n\tInput Path: [{1}]\tName is: [{2}]\t\tSystem.IO: [{3}]", currDirActual, inputPath, actual.Name, expected.Name);
            Assert.AreEqual(currDirActual, currDirExpected);

            // Compare values of both instances.
            CompareDirectoryInfos(expected, actual);
         }
         catch (Exception ex)
         {
            Console.WriteLine("\n\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));
         }

         #region Non existing directory

         string path = @"N:\nonexisting";

         #region Local

         Console.WriteLine("\n\tLocal non existing directory: [{0}]", path);
         if (!DumpDirectoryInfo(path))
            allOk = false;
         
         #endregion // Local

         #region Network

         path = Path.LocalToUnc(path);
         Console.WriteLine("\n\tNetwork non existing directory: [{0}]", path);
         if (!DumpDirectoryInfo(path))
            allOk = false;

         #endregion // Network

         #endregion // Non existing directory

         #region Existing directory

         // Somehow System.IO seems to disagree with SystemRoot32 directory.
         path = SysRoot;

         #region Local

         Console.WriteLine("\n\tLocal existing directory: [{0}]", path);
         if (!DumpDirectoryInfo(path))
            allOk = false;

         #endregion //Local

         #region Network

         path = Path.LocalToUnc(path);
         if (Directory.Exists(path))
         {
            Console.WriteLine("\n\tNetwork existing directory: [{0}]", path);
            if (!DumpDirectoryInfo(path))
               allOk = false;
         }
         else
            Assert.IsTrue(false, "Share inaccessible: {0}", path);
         
         #endregion // Network

         #endregion // Existing directory

         Assert.AreEqual(true, allOk, "Encountered DirectoryInfo Properties where AlphaFS != System.IO");
      }

      #endregion // DirectoryInfo

      #endregion // AlphaFS
   }
}