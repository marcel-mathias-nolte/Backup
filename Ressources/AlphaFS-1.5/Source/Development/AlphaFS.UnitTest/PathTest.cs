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
using Alphaleonis.Win32.Filesystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using DriveInfo = Alphaleonis.Win32.Filesystem.DriveInfo;
using File = Alphaleonis.Win32.Filesystem.File;
using NativeMethods = Alphaleonis.Win32.Filesystem.NativeMethods;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace AlphaFS.UnitTest
{
   /// <summary>This is a test class for Path and is intended to contain all Path Unit Tests.</summary>
   [TestClass]
   public class PathTest
   {
      #region PathTest Helpers

      private TestContext testContextInstance;

      /// <summary>
      ///Gets or sets the test context which provides
      ///information about and functionality for the current test run.
      ///</summary>
      public TestContext TestContext
      {
         get
         {
            return testContextInstance;
         }
         set
         {
            testContextInstance = value;
         }
      }

      private static readonly string StartupFolder = AppDomain.CurrentDomain.BaseDirectory;
      private static readonly string SysDrive = Environment.GetEnvironmentVariable("SystemDrive");
      private static readonly string SysRoot = Environment.GetEnvironmentVariable("SystemRoot");

      private const string SpecificX3 = "Windows XP and Windows Server 2003 specific.";
      private const string TextTrue = "IsTrue";
      private const string TextFalse = "IsFalse";
      private const string TenNumbers = "0123456789";
      private const string TextNew = "Hëllõ Wørld!";
      private const string TextAppend = "GóödByé Wôrld!";
      private const string TextUtf16 = " - ÛņïÇòdè; ǖŤƑ-16";
      private const string ValidName = "JustAFolder";
      private const string WildcardValidNames = "JustAF*lder";
      private const string InValidName = @"Just</\>F*|der";
      private const string NotExist = "DoesNotNeedExist";
      private readonly string _doesNotNeedExist = Path.Combine(StartupFolder, NotExist);

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
            //var lastErr = new Win32Exception();
            //Console.WriteLine("[{0}] [{1}]\n", lastErr.NativeErrorCode, lastErr.Message);

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

      private static void ShowChars(char[] charArray)
      {
         Console.WriteLine("\tChar\tHex Value");

         // Display each invalid character to the console. 
         foreach (char someChar in charArray)
         {
            if (Char.IsWhiteSpace(someChar))
               Console.WriteLine("\t,\t{0:X4}", (int)someChar);
            else
               Console.WriteLine("\t{0:c},\t{1:X4}", someChar, (int)someChar);
         }
      }

      private static byte[] StringToByteArray(string str, params Encoding[] encoding)
      {
         var encode = encoding != null && encoding.Any() ? encoding[0] : new UTF8Encoding(true, true);
         return encode.GetBytes(str);
      }

      #region InputPaths

      static readonly string[] InputPaths = new[]
         {
            @".",
            @".zip",
            @"C:\Test\File\Foo.txt",
            @"\\Server\Share\File.txt",
            @"\\?\UNC\Server\Share Name\File Name 1.txt",
            @"\\Server\Share",
            @"\\Server\C$",
            @"\\?\C:\Directory\",
            @"\",
            @"\Test\File",
            @"\\?\globalroot\device\harddisk0\partition1\",
            @"\\?\Volume{12345678-aac3-31de-3321-3124565341ed}\Program Files\notepad.exe",
            @"Program Files\Microsoft Office",
            @"C",
            @"C:",
            @"C:\",
            @"C:\a",
            @"C:\a\",
            @"C:\a\b",
            @"C:\a\b\",
            @"C:\a\b\c",
            @"C:\a\b\c\",
            @"C:\a\b\c\f",
            @"C:\a\b\c\f.",
            @"C:\a\b\c\f.t",
            @"C:\a\b\c\f.tx",
            @"C:\a\b\c\f.txt",
            @"\\Server\Share\",
            @"\\Server\Share\d",
            @"\\Server\Share\d1",
            @"\\Server\Share\d1\",
            @"\\Server\Share\d1\d",
            @"\\Server\Share\d1\d2",
            @"\\Server\Share\d1\d2\",
            @"\\Server\Share\d1\d2\f",
            @"\\Server\Share\d1\d2\fi",
            @"\\Server\Share\d1\d2\fil",
            @"\\Server\Share\d1\d2\file",
            @"\\Server\Share\d1\d2\file.",
            @"\\Server\Share\d1\d2\file.e",
            @"\\Server\Share\d1\d2\file.ex",
            @"\\Server\Share\d1\d2\file.ext"
         };

      #endregion // InputPaths
      
      private void DumpGetFinalPathNameByHandle(bool isOne)
      {
         string tempFile = Path.GetTempFileName();

         // Results from this method are always prefixed with a LongPath.
         string longTempStream = Path.LongPathPrefix + tempFile;
         bool gotFileNameNormalized;
         bool gotFileNameOpened;
         bool gotVolumeNameDos;
         bool gotVolumeNameGuid;
         bool gotVolumeNameNt;
         bool gotVolumeNameNone;
         bool gotSomething;

         using (FileStream stream = File.Create(tempFile))
         {
            string note = string.Empty;
            if (isOne)
            {
               stream.WriteByte(1);
               note = "\tNote: file needs to be at least one byte; can't map a zero byte file.";
            }

            StopWatcher(true);
            string fileNameNormalized = (isOne) ? Path.GetFinalPathNameByHandleX3(stream) : Path.GetFinalPathNameByHandle(stream);
            string fileNameOpened = (isOne) ? Path.GetFinalPathNameByHandleX3(stream, FinalPathFormats.FileNameOpened) : Path.GetFinalPathNameByHandle(stream, FinalPathFormats.FileNameOpened);

            string volumeNameDos = (isOne) ? Path.GetFinalPathNameByHandleX3(stream, FinalPathFormats.VolumeNameDos) : Path.GetFinalPathNameByHandle(stream, FinalPathFormats.VolumeNameDos);
            string volumeNameGuid = (isOne) ? Path.GetFinalPathNameByHandleX3(stream, FinalPathFormats.VolumeNameGuid) : Path.GetFinalPathNameByHandle(stream, FinalPathFormats.VolumeNameGuid);
            string volumeNameNt = (isOne) ? Path.GetFinalPathNameByHandleX3(stream, FinalPathFormats.VolumeNameNT) : Path.GetFinalPathNameByHandle(stream, FinalPathFormats.VolumeNameNT);
            string volumeNameNone = (isOne) ? Path.GetFinalPathNameByHandleX3(stream, FinalPathFormats.VolumeNameNone) : Path.GetFinalPathNameByHandle(stream, FinalPathFormats.VolumeNameNone);

            // These three output the same.
            gotFileNameNormalized = !string.IsNullOrEmpty(fileNameNormalized) && longTempStream.Equals(fileNameNormalized);
            gotFileNameOpened = !string.IsNullOrEmpty(fileNameOpened) && longTempStream.Equals(fileNameOpened);
            gotVolumeNameDos = !string.IsNullOrEmpty(volumeNameDos) && longTempStream.Equals(volumeNameDos);

            gotVolumeNameGuid = !string.IsNullOrEmpty(volumeNameGuid) && volumeNameGuid.StartsWith(Path.VolumePrefix) && volumeNameGuid.EndsWith(volumeNameNone);
            gotVolumeNameNt = !string.IsNullOrEmpty(volumeNameNt) && volumeNameNt.StartsWith(Path.DevicePrefix);
            gotVolumeNameNone = !string.IsNullOrEmpty(volumeNameNone) && tempFile.EndsWith(volumeNameNone);

            Console.WriteLine("\n\tFilestream        ==\t[{0}]", tempFile);
            Console.WriteLine("\tFilestream.Name   ==\t[{0}]", stream.Name);
            Console.WriteLine("\tFilestream.Length ==\t[{0}] == [{1}]: {2}{3}\n", NativeMethods.UnitSizeToText(stream.Length), TextTrue, (isOne) ? isOne : isOne == false, note);
            Console.WriteLine("\tFileNameNormalized ==\t[{0}]", fileNameNormalized);
            Console.WriteLine("\tFileNameOpened     ==\t[{0}]", fileNameOpened);
            Console.WriteLine("\tVolumeNameDos      ==\t[{0}]", volumeNameDos);
            Console.WriteLine("\tVolumeNameGuid     ==\t[{0}]", volumeNameGuid);
            Console.WriteLine("\tVolumeNameNt       ==\t[{0}]", volumeNameNt);
            Console.WriteLine("\tVolumeNameNone     ==\t[{0}]", volumeNameNone);

            Console.WriteLine("\n{0}", Reporter());

            gotSomething = true;
         }

         bool fileExists = File.Exists(tempFile);
         bool fileNotExists = File.Delete(tempFile, true);
         Console.WriteLine("\n\t(Deleted tempfile == [{0}]: {1})", TextTrue, fileNotExists);
         Assert.IsTrue(fileNotExists);


         Assert.IsTrue(fileExists);
         Assert.IsTrue((isOne) ? isOne : isOne == false);
         Assert.IsTrue(gotFileNameNormalized);
         Assert.IsTrue(gotFileNameOpened);
         Assert.IsTrue(gotVolumeNameDos);
         Assert.IsTrue(gotVolumeNameGuid);
         Assert.IsTrue(gotVolumeNameNt);
         Assert.IsTrue(gotVolumeNameNone);
         Assert.IsTrue(fileNotExists);
         Assert.IsTrue(gotSomething);
      }

      private void Dump83Path(string fullPath)
      {
         Console.WriteLine("\n\tPath: [{0}]", fullPath);

         if (Directory.Exists(fullPath))
         {
            // GetShort83Path()
            StopWatcher(true);
            string short83Path = Path.GetShort83Path(fullPath);
            string reporter = Reporter();
            bool isShort83Path = !string.IsNullOrEmpty(short83Path) && !short83Path.Equals(fullPath) && Directory.Exists(short83Path);
            bool hasTilde = !string.IsNullOrEmpty(short83Path) && short83Path.IndexOf('~') >= 0;

            Console.WriteLine("\n\t{0, 20} == [{1}]: [{2}]: [{3}]\n{4}", "GetShort83Path()", TextTrue, isShort83Path, short83Path, reporter);
            Assert.IsTrue(isShort83Path);
            Assert.IsTrue(hasTilde); // A bit tricky if fullPath is already a shortPath.

            // GetLongFrom83Path()
            StopWatcher(true);
            string longFrom83Path = Path.GetLongFrom83Path(short83Path);
            reporter = Reporter();
            bool isLongFrom83Path = !string.IsNullOrEmpty(longFrom83Path) && !longFrom83Path.Equals(short83Path) && Directory.Exists(longFrom83Path);
            bool noTilde = !string.IsNullOrEmpty(longFrom83Path) && longFrom83Path.IndexOf('~') == -1;

            Console.WriteLine("\n\t{0, 20} == [{1}]: [{2}]: [{3}]\n{4}\n", "GetLongFrom83Path()", TextTrue, isLongFrom83Path, longFrom83Path, reporter);
            Assert.IsTrue(isLongFrom83Path);
            Assert.IsTrue(noTilde);
         }
         else
         {
            Console.WriteLine("\tShare inaccessible: {0}", fullPath);
         }
      }

      private bool DumpCombine(bool isLocal)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");

         string path1 = @"c:\temp";
         string path2 = @"subdir\file.txt";
         string path3 = @"c:\temp.txt";
         string path4 = @"c:^*&)(_=@#'\^&#2.*(.txt";
         string path5 = @"";
         string path6 = null;

         string p1 = null;
         string p2 = null;

         try
         {
            if (!isLocal)
            {
               path1 = Path.LocalToUnc(path1);
               path2 = Path.LocalToUnc(path2);
               path3 = Path.LocalToUnc(path3);
               path4 = Path.LocalToUnc(path4);
            }

            p1 = path1;
            p2 = path2;
            string combAlphsFS = Path.Combine(p1, p2);
            string combSysIO = System.IO.Path.Combine(p1, p2);
            Console.WriteLine("\t\tCombine('{0}', '{1}'): [{2}]\t\tSystem.IO: [{3}]", p1, p2, combAlphsFS, combSysIO);
            Assert.AreEqual(combSysIO, combAlphsFS);

            p1 = path1;
            p2 = path3;
            combAlphsFS = Path.Combine(p1, p2);
            combSysIO = System.IO.Path.Combine(p1, p2);
            Console.WriteLine("\n\t\tCombine('{0}', '{1}'): [{2}]\t\tSystem.IO: [{3}]", p1, p2, combAlphsFS, combSysIO);
            Assert.AreEqual(combSysIO, combAlphsFS);

            p1 = path3;
            p2 = path2;
            combAlphsFS = Path.Combine(p1, p2);
            combSysIO = System.IO.Path.Combine(p1, p2);
            Console.WriteLine("\n\t\tCombine('{0}', '{1}'): [{2}]\t\tSystem.IO: [{3}]", p1, p2, combAlphsFS, combSysIO);
            Assert.AreEqual(combSysIO, combAlphsFS);

            p1 = path4;
            p2 = path2;
            combAlphsFS = Path.Combine(p1, p2);
            combSysIO = System.IO.Path.Combine(p1, p2);
            Console.WriteLine("\n\t\tCombine('{0}', '{1}'): [{2}]\t\tSystem.IO: [{3}]", p1, p2, combAlphsFS, combSysIO);
            Assert.AreEqual(combSysIO, combAlphsFS);

            p1 = path5;
            p2 = path2;
            combAlphsFS = Path.Combine(p1, p2);
            combSysIO = System.IO.Path.Combine(p1, p2);
            Console.WriteLine("\n\t\tCombine('{0}', '{1}'): [{2}]\t\tSystem.IO: [{3}]", p1, p2, combAlphsFS, combSysIO);
            Assert.AreEqual(combSysIO, combAlphsFS);

            p1 = path6;
            p2 = path1;
            combAlphsFS = Path.Combine(p1, p2);
            combSysIO = System.IO.Path.Combine(p1, p2);
            Console.WriteLine("\n\t\tCombine('{0}', '{1}'): [{2}]\t\tSystem.IO: [{3}]", p1, p2, combAlphsFS, combSysIO);
            Assert.AreEqual(combSysIO, combAlphsFS);

            return true;
         }
         catch (Exception ex)
         {
            if (p1 == null)
               p1 = "null";
            if (p2 == null)
               p2 = "null";
            Console.WriteLine("\n\t\tYou cannot combine: [{0}] and: [{1}] because: [{2}]", p1, p2, ex.Message.Replace(Environment.NewLine, string.Empty));

            return false;
         }
      }

      private static void DumpHasExtension()
      {
         const string template = "{0}({1}) == [{2}]: {3}\n";
         string tempFile = Path.GetTempFileName();
         Console.WriteLine("\ntempFile: [{0}]", tempFile);

         // GetFileName();
         Console.WriteLine("GetFileName(): [{0}]\n", Path.GetFileName(tempFile));

         // True
         string extension = Path.GetExtension(tempFile);
         Console.WriteLine("extension: [{0}]", extension);
         bool hasExtension = Path.HasExtension(tempFile) && !string.IsNullOrWhiteSpace(Path.GetExtension(tempFile));
         Console.WriteLine(string.Format(CultureInfo.CurrentCulture, template, "Extension()", "tempFile", TextTrue, hasExtension));

         // False
         extension = tempFile.Split(Path.ExtensionSeparatorChar)[1];
         Console.WriteLine("extension: [{0}]", extension);
         bool hasNoExtension = !Path.HasExtension(extension) && string.IsNullOrWhiteSpace(Path.GetExtension(extension));
         Console.WriteLine(string.Format(CultureInfo.CurrentCulture, template, "Extension()", "extension", TextFalse, hasNoExtension));

         Assert.IsTrue(hasExtension);
         Assert.IsTrue(hasNoExtension);

         bool fileNotExists = File.Delete(tempFile, true);
         Console.WriteLine("\n\t(Deleted tempfile == [{0}]: {1})", TextTrue, fileNotExists);
         Assert.IsTrue(fileNotExists);
      }

      private static void DumpGetFullPath(int mode)
      {
         string dirAlphaFS = Directory.GetCurrentDirectory();
         string dirSysIO = System.IO.Directory.GetCurrentDirectory();
         Console.WriteLine("\n\n\t{0}: Directory.GetCurrentDirectory() == [{1}]\t\tSystem.IO: [{2}]", mode, dirAlphaFS, dirSysIO);
         Assert.AreEqual(dirSysIO, dirAlphaFS);

         dirAlphaFS = Path.GetFullPath(SysDrive);
         dirSysIO = System.IO.Path.GetFullPath(SysDrive);
         Console.WriteLine("\n\t{0}: Path.GetFullPath('{1}') == [{2}]\t\tSystem.IO: [{3}]", mode, SysDrive, dirAlphaFS, dirSysIO);

         if (!dirSysIO.Equals(SysDrive + @"\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE", StringComparison.OrdinalIgnoreCase))
            Assert.AreEqual(dirSysIO, dirAlphaFS);
      }
      
      private static void DumpGetDirectoryName(bool isLocal, string path)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");

         string filePath = isLocal ? path : Path.LocalToUnc(path);
         int i = 0;

         while (filePath != null)
         {
            string directoryNameAlphaFS = Path.GetDirectoryName(filePath);
            string directoryNameSysIO = System.IO.Path.GetDirectoryName(filePath);

            Console.WriteLine("\t\tGetDirectoryName('{0}') returns: [{1}]\t\tSystem.IO: [{2}]\n", filePath, directoryNameAlphaFS, directoryNameSysIO);

            Assert.AreEqual(directoryNameSysIO, directoryNameAlphaFS);

            // This will preserve the previous path.
            filePath = i == 1 ? directoryNameAlphaFS + @"\" : directoryNameAlphaFS;

            i++;
         }
      }

      private static void DumpGetDirectoryNameWithoutRoot(bool isLocal)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");

         string fullPath = Environment.SystemDirectory;
         if (!isLocal)
            fullPath = Path.LocalToUnc(fullPath);

         string directoryNameWithoutRoot = Path.GetDirectoryNameWithoutRoot(fullPath);

         bool hasDirectory = !string.IsNullOrEmpty(directoryNameWithoutRoot) &&
                                !fullPath.StartsWith(directoryNameWithoutRoot) &&
                                fullPath.Contains(directoryNameWithoutRoot);

         Console.WriteLine("\t\tInput Path: [{0}]\t\tGetDirectoryNameWithoutRoot(): [{1}] == [{2}]\n", fullPath, hasDirectory, directoryNameWithoutRoot);

         Assert.IsTrue(hasDirectory);


         fullPath = SysRoot;
         if (!isLocal)
            fullPath = Path.LocalToUnc(fullPath);

         directoryNameWithoutRoot = Path.GetDirectoryNameWithoutRoot(fullPath);

         hasDirectory = !string.IsNullOrEmpty(directoryNameWithoutRoot) &&
                        !fullPath.StartsWith(directoryNameWithoutRoot) &&
                        fullPath.Contains(directoryNameWithoutRoot);

         Console.WriteLine("\t\tInput Path: [{0}]\t\tGetDirectoryNameWithoutRoot(): [{1}] == [{2}]\n", fullPath, hasDirectory, directoryNameWithoutRoot);

         Assert.IsFalse(hasDirectory);

      }

      private static void DumpGetExtension(bool isLocal)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");

         string fileName = SysDrive + @"\mydir.old\myfile.ext";
         string path = SysDrive + @"\mydir.old\";

         if (!isLocal)
         {
            fileName = Path.LocalToUnc(fileName);
            path = Path.LocalToUnc(path);
         }

         string extensionAlphsFS = Path.GetExtension(fileName);
         string extensionSysIO = System.IO.Path.GetExtension(fileName);
         Console.WriteLine("\t\tGetExtension('{0}') returns: [{1}]", fileName, extensionAlphsFS);
         Assert.AreEqual(extensionSysIO, extensionAlphsFS);

         extensionAlphsFS = Path.GetExtension(path);
         extensionSysIO = System.IO.Path.GetExtension(path);
         Console.WriteLine("\n\t\tGetExtension('{0}') returns: [{1}]", path, extensionAlphsFS);
         Assert.AreEqual(extensionSysIO, extensionAlphsFS);
      }

      private static void DumpGetFileName(bool isLocal)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");

         string fileName = SysDrive + @"\mydir\myfile.ext";
         string path = SysDrive + @"\mydir\";

         if (!isLocal)
         {
            fileName = Path.LocalToUnc(fileName);
            path = Path.LocalToUnc(path);
         }

         string resultAlphaFS = Path.GetFileName(fileName);
         string resultSysIO = System.IO.Path.GetFileName(fileName);
         Console.WriteLine("\t\tInput Path: [{0}]\t\tGetFileName(): [{1}]\t\tSystem.IO: [{2}]", fileName, resultAlphaFS, resultSysIO);
         Assert.AreEqual(resultSysIO, resultAlphaFS);

         resultAlphaFS = Path.GetFileName(path);
         resultSysIO = System.IO.Path.GetFileName(path);
         Console.WriteLine("\n\t\tInput Path: [{0}]\t\tGetFileName(): [{1}]\t\tSystem.IO: [{2}]", path, resultAlphaFS, resultSysIO);
         Assert.AreEqual(resultSysIO, resultAlphaFS);
      }

      private static void DumpGetFileNameWithoutExtension(bool isLocal)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");

         string fileName = SysDrive + @"\mydir\myfile.ext";
         string path = SysDrive + @"\mydir\";

         if (!isLocal)
         {
            fileName = Path.LocalToUnc(fileName);
            path = Path.LocalToUnc(path);
         }

         string resultAlphaFS = Path.GetFileNameWithoutExtension(fileName);
         string resultSysIO = System.IO.Path.GetFileNameWithoutExtension(fileName);
         Console.WriteLine("\t\tInput Path: [{0}]\t\tGetFileNameWithoutExtension(): [{1}]\t\tSystem.IO: [{2}]", fileName, resultAlphaFS, resultSysIO);
         Assert.AreEqual(resultSysIO, resultAlphaFS);

         resultAlphaFS = Path.GetFileNameWithoutExtension(path);
         resultSysIO = System.IO.Path.GetFileNameWithoutExtension(path);
         Console.WriteLine("\n\t\tInput Path: [{0}]\t\tGetFileNameWithoutExtension(): [{1}]\t\tSystem.IO: [{2}]", path, resultAlphaFS, resultSysIO);
         Assert.AreEqual(resultSysIO, resultAlphaFS);
      }

      private static void DumpGetFullPath(bool isLocal)
      {
         int pathCnt = 0;
         bool allOk = true;
         int errorCnt = 0;

         foreach (string input in InputPaths)
         {
            string path = input;

            try
            {
               string actual = Path.GetFullPath(path);
               string expected = System.IO.Path.GetFullPath(path);
               Console.WriteLine("\n\t#{0:000}\tInput Path: [{1}]\t\tGetFullPath(): [{2}]\t\tSystem.IO: [{3}]", ++pathCnt, path, actual, expected);
               Assert.AreEqual(expected, actual);
            }
            catch (ArgumentException){}
            catch (Exception ex)
            {
               Console.WriteLine("\n\t\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));
               allOk = false;
               errorCnt++;
            }
         }

         Assert.AreEqual(true, allOk, "Encountered: [{0}] paths where AlphaFS != System.IO", errorCnt);
      }

      private static void DumpGetSuffixedDirectoryName(bool isLocal)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");

         string path = Environment.SystemDirectory;
         string suffixedDirectoryName = Path.GetSuffixedDirectoryName(path);

         if (!isLocal)
         {
            path = Path.LocalToUnc(path);
            suffixedDirectoryName = Path.LocalToUnc(suffixedDirectoryName);
         }

         bool isSuffixed = !string.IsNullOrEmpty(suffixedDirectoryName) &&
                           path.StartsWith(suffixedDirectoryName) &&
                           suffixedDirectoryName.EndsWith(Path.DirectorySeparatorChar.ToString());

         Console.WriteLine("\t\tInput Path: [{0}]\t\tGetSuffixedDirectoryName(): [{1}]", path, suffixedDirectoryName);
         Assert.IsTrue(isSuffixed);
      }

      private static void DumpGetSuffixedDirectoryNameWithoutRoot(bool isLocal)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");

         string path = Environment.SystemDirectory;
         string suffixedDirectoryNameWithoutRoot = Path.GetSuffixedDirectoryNameWithoutRoot(path);

         if (!isLocal)
         {
            path = Path.LocalToUnc(path);
            suffixedDirectoryNameWithoutRoot = Path.LocalToUnc(suffixedDirectoryNameWithoutRoot);
         }

         bool isSuffixedWithoutRoot = !string.IsNullOrEmpty(suffixedDirectoryNameWithoutRoot) &&
                                      !path.StartsWith(suffixedDirectoryNameWithoutRoot) &&
                                      path.Contains(suffixedDirectoryNameWithoutRoot) &&
                                      suffixedDirectoryNameWithoutRoot.EndsWith(Path.DirectorySeparatorChar.ToString());

         Console.WriteLine("\t\tInput Path: [{0}]\t\tGetSuffixedDirectoryNameWithoutRoot(): [{1}]", path, suffixedDirectoryNameWithoutRoot);

         Assert.IsTrue(isSuffixedWithoutRoot);
      }

      private static void GetRemoteNameInfo()
      {
         Console.WriteLine("\nIf your mapped network drives don't show up, take a look at this: {0}", "http://alphafs.codeplex.com/discussions/397693");

         int cnt = 0;
         foreach (string drive in Directory.GetLogicalDrives().Where(drive => new DriveInfo(drive).IsUnc))
         {
            ++cnt;

            StopWatcher(true);
            string gmCn = Path.GetMappedConnectionName(drive);
            string gmUn = Path.GetMappedUncName(drive);
            Console.WriteLine("\n\tPath: [{0}]\tGetMappedConnectionName(): [{1}]", drive, gmCn);
            Console.WriteLine("\tPath: [{0}]\tGetMappedUncName()       : [{1}]\n{2}", drive, gmUn, Reporter());

            Assert.IsTrue(!string.IsNullOrEmpty(gmCn));
            Assert.IsTrue(!string.IsNullOrEmpty(gmUn));
         }

         if (cnt == 0)
            Assert.Fail("No mapped network drives found.");
      }

      #endregion // PathTest Helpers

      #region .NET

      #region ChangeExtension

      [TestMethod]
      public void ChangeExtension()
      {
         Console.WriteLine("Path.ChangeExtension()");
         Console.WriteLine("\n\tThe .NET method is used.");
      }

      #endregion // ChangeExtension

      #region Combine

      [TestMethod]
      public void Combine()
      {
         Console.WriteLine("Path.Combine()");

         // From Microsoft site.
         // This code produces output similar to the following: 
         // 
         // When you combine 'c:\temp' and 'subdir\file.txt', the result is:  
         // 'c:\temp\subdir\file.txt' 
         //  
         // When you combine 'c:\temp' and 'c:\temp.txt', the result is:  
         // 'c:\temp.txt' 
         //  
         // When you combine 'c:\temp.txt' and 'subdir\file.txt', the result is:  
         // 'c:\temp.txt\subdir\file.txt' 
         //  
         // When you combine 'c:^*&)(_=@#'\^&#2.*(.txt' and 'subdir\file.txt', the result is:  
         // 'c:^*&)(_=@#'\^&#2.*(.txt\subdir\file.txt' 
         //  
         // When you combine '' and 'subdir\file.txt', the result is:  
         // 'subdir\file.txt' 
         //  
         // You cannot combine '' and 'subdir\file.txt' because:  
         // Value cannot be null. 
         // Parameter name: path1

         Assert.IsFalse(DumpCombine(true));
         Assert.IsFalse(DumpCombine(false));
      }

      #endregion // Combine

      #region GetDirectoryName

      [TestMethod]
      public void GetDirectoryName()
      {
         Console.WriteLine("Path.GetDirectoryName()");

         // From Microsoft site.
         // This code produces the following output:

         // GetDirectoryName('C:\MyDir\MySubDir\myfile.ext') returns 'C:\MyDir\MySubDir'
         // GetDirectoryName('C:\MyDir\MySubDir') returns 'C:\MyDir'
         // GetDirectoryName('C:\MyDir\') returns 'C:\MyDir'
         // GetDirectoryName('C:\MyDir') returns 'C:\'
         // GetDirectoryName('C:\') returns ''
         
         string path = SysDrive + @"\MyDir\MySubDir\myfile.ext";

         DumpGetDirectoryName(true, path);
         DumpGetDirectoryName(false, path);
      }

      #endregion // GetDirectoryName

      #region GetExtension

      [TestMethod]
      public void GetExtension()
      {
         Console.WriteLine("Path.GetExtension()");

         // From Microsoft site.
         // This code produces output similar to the following: 
         // 
         // GetExtension('C:\mydir.old\myfile.ext') returns '.ext' 
         // GetExtension('C:\mydir.old\') returns ''

         DumpGetExtension(true);
         DumpGetExtension(false);
      }

      #endregion // GetExtension

      #region GetFileName

      [TestMethod]
      public void GetFileName()
      {
         Console.WriteLine("Path.GetFileName()");

         // From Microsoft site.
         // This code produces output similar to the following: 
         // 
         // GetFileName('C:\mydir\myfile.ext') returns 'myfile.ext' 
         // GetFileName('C:\mydir\') returns ''

         DumpGetFileName(true);
         DumpGetFileName(false);
      }

      #endregion // GetFileName

      #region GetFileNameWithoutExtension

      [TestMethod]
      public void GetFileNameWithoutExtension()
      {
         Console.WriteLine("Path.GetFileNameWithoutExtension()");

         // From Microsoft site.
         // This code produces output similar to the following: 
         // 
         // GetFileNameWithoutExtension('C:\mydir\myfile.ext') returns 'myfile' 
         // GetFileName('C:\mydir\') returns ''

         DumpGetFileNameWithoutExtension(true);
         DumpGetFileNameWithoutExtension(false);
      }

      #endregion // GetFileNameWithoutExtension

      #region GetFullPath

      [TestMethod]
      public void GetFullPath()
      {
         Console.WriteLine("Path.GetFullPath()");

         DumpGetFullPath(true);
         DumpGetFullPath(false);
      }

      #endregion // GetFullPath

      #region GetInvalidFileNameChars

      [TestMethod]
      public void GetInvalidFileNameChars()
      {
         Console.WriteLine("Path.GetInvalidFileNameChars()");
         Console.WriteLine("\n\tThe .NET method is used.");
      }

      #endregion // GetInvalidFileNameChars

      #region GetInvalidPathChars

      [TestMethod]
      public void GetInvalidPathChars()
      {
         Console.WriteLine("Path.GetInvalidPathChars()");
         Console.WriteLine("\n\tThe .NET method is used.");
      }

      #endregion // GetInvalidPathChars

      #region GetPathRoot

      [TestMethod]
      public void GetPathRoot()
      {
         Console.WriteLine("Path.GetPathRoot()");

         // From Microsoft site.
         // This code produces output similar to the following:

         // GetPathRoot('\mydir\') returns '\'
         // GetPathRoot('myfile.ext') returns '' 
         // GetPathRoot('C:\mydir\myfile.ext') returns 'C:\'


         string path = @"\mydir\";
         string fileName = "myfile.ext";
         string fullPath = SysDrive + @"\mydir\myfile.ext";
         string pathRootAlphaFS;
         string pathRootSysIO;

         pathRootAlphaFS = Path.GetPathRoot(path);
         pathRootSysIO = System.IO.Path.GetPathRoot(path);
         Console.WriteLine("\n\tGetPathRoot('{0}') returns: [{1}]\t\tSystem.IO: [{2}]", path, pathRootAlphaFS, pathRootSysIO);
         Assert.AreEqual(pathRootSysIO, pathRootAlphaFS);

         pathRootAlphaFS = Path.GetPathRoot(fileName);
         pathRootSysIO = System.IO.Path.GetPathRoot(fileName);
         Console.WriteLine("\n\tGetPathRoot('{0}') returns: [{1}]\t\tSystem.IO: [{2}]", fileName, pathRootAlphaFS, pathRootSysIO);
         Assert.AreEqual(pathRootSysIO, pathRootAlphaFS);

         // Local.
         Console.WriteLine("\n\n\tLocal");

         pathRootAlphaFS = Path.GetPathRoot(fullPath);
         pathRootSysIO = System.IO.Path.GetPathRoot(fullPath);
         Console.WriteLine("\t\tGetPathRoot('{0}') returns: [{1}]\t\tSystem.IO: [{2}]", fullPath, pathRootAlphaFS, pathRootSysIO);
         Assert.AreEqual(pathRootSysIO, pathRootAlphaFS);

         // Network.
         Console.WriteLine("\n\tNetwork");

         fullPath = Path.LocalToUnc(fullPath);
         pathRootAlphaFS = Path.GetPathRoot(fullPath);
         pathRootSysIO = System.IO.Path.GetPathRoot(fullPath);
         Console.WriteLine("\t\tGetPathRoot('{0}') returns: [{1}]\t\tSystem.IO: [{2}]", fullPath, pathRootAlphaFS, pathRootSysIO);
         Assert.AreEqual(pathRootSysIO, pathRootAlphaFS);
      }

      #endregion // GetPathRoot

      #region GetRandomFileName

      [TestMethod]
      public void GetRandomFileName()
      {
         Console.WriteLine("Path.GetRandomFileName()");
         Console.WriteLine("\n\tThe .NET method is used.");
      }

      #endregion //GetRandomFileName

      #region GetTempFileName

      [TestMethod]
      public void GetTempFileName()
      {
         Console.WriteLine("Path.GetTempFileName()");
         Console.WriteLine("\n\tThe .NET method is used.");
      }

      #endregion // GetTempFileName

      #region GetTempPath

      [TestMethod]
      public void GetTempPath()
      {
         Console.WriteLine("Path.GetTempPath()");
         Console.WriteLine("\n\tThe .NET method is used.");
      }

      #endregion // GetTempPath

      #region HasExtension

      [TestMethod]
      public void HasExtension()
      {
         Console.WriteLine("Path.HasExtension()");

         DumpHasExtension();
      }

      #endregion // HasExtension

      #region IsPathRooted

      [TestMethod]
      public void IsPathRooted()
      {
         Console.WriteLine("Path.IsPathRooted()");

         // From Microsoft site.
         // This code produces output similar to the following: 
         // 
         // IsPathRooted('C:\mydir\myfile.ext') returns True 
         // IsPathRooted('\\myPc\mydir\myfile') returns True 
         // IsPathRooted('mydir\sudir\') returns False


         string fileName = SysDrive + @"\mydir\myfile.ext";
         string UncPath = @"\\myPc\mydir\myfile";
         string relativePath = @"mydir\sudir\";

         bool resultAlphaFS = Path.IsPathRooted(fileName);
         bool resultSysIO = System.IO.Path.IsPathRooted(fileName);
         Console.WriteLine("\n\tIsPathRooted('{0}') returns: [{1}]", fileName, resultAlphaFS);
         Assert.IsTrue(resultAlphaFS);
         Assert.IsTrue(resultAlphaFS.Equals(resultSysIO));

         resultAlphaFS = Path.IsPathRooted(UncPath);
         resultSysIO = System.IO.Path.IsPathRooted(UncPath);
         Console.WriteLine("\n\tIsPathRooted('{0}') returns: [{1}]", UncPath, resultAlphaFS);
         Assert.IsTrue(resultAlphaFS);
         Assert.IsTrue(resultAlphaFS.Equals(resultSysIO));

         resultAlphaFS = Path.IsPathRooted(relativePath);
         resultSysIO = System.IO.Path.IsPathRooted(relativePath);
         Console.WriteLine("\n\tIsPathRooted('{0}') returns: [{1}]", relativePath, resultAlphaFS);
         Assert.IsFalse(resultAlphaFS);
         Assert.IsTrue(resultAlphaFS.Equals(resultSysIO));
      }

      #endregion // IsPathRooted

      #endregion // .NET

      #region AlphaFS

      #region GetLongPath

      [TestMethod]
      public void GetLongPath()
      {
         Console.WriteLine("Path.GetLongPath()");

         Directory.SetCurrentDirectory(SysRoot);
         Console.WriteLine("\n\tFolder: [{0}]", SysRoot);

         string fullPath = Path.GetFullPath(Directory.GetCurrentDirectory());
         string longPath = Path.GetLongPath(fullPath);
         bool isLongPath = !string.IsNullOrEmpty(longPath) && longPath.StartsWith(Path.LongPathPrefix);
         bool endsWithCurrentFolder = longPath != null && longPath.EndsWith(fullPath);
         Console.WriteLine("\tPath.GetLongPath() == [{0}] == [{1}]: {2}", longPath, TextTrue, isLongPath);

         Assert.IsTrue(isLongPath);
         Assert.IsTrue(endsWithCurrentFolder);


         string uncPath = Path.LocalToUnc(SysRoot);
         Directory.SetCurrentDirectory(uncPath);
         Console.WriteLine("\n\tFolder: [{0}]", uncPath);

         fullPath = Path.GetFullPath(uncPath);
         longPath = Path.GetLongPath(fullPath);
         isLongPath = !string.IsNullOrEmpty(longPath) && longPath.StartsWith(Path.LongPathUncPrefix);

         // Remove \\ from \\server...
         fullPath = fullPath.TrimStart(Path.DirectorySeparatorChar);

         endsWithCurrentFolder = longPath != null && longPath.EndsWith(fullPath);
         Console.WriteLine("\tPath.GetLongPath() == [{0}] == [{1}]: {2}", longPath, TextTrue, isLongPath);

         Assert.IsTrue(isLongPath);
         Assert.IsTrue(endsWithCurrentFolder);
      }

      #endregion // GetLongPath

      #region GetLongFrom83Path

      [TestMethod]
      public void GetLongFrom83Path()
      {
         Console.WriteLine("Path.GetLongFrom83Path()");

         Dump83Path(StartupFolder);
         Dump83Path(Path.LocalToUnc(StartupFolder));
      }

      #endregion // GetLongFrom83Path

      #region GetMappedConnectionName

      [TestMethod]
      public void GetMappedConnectionName()
      {
         Console.WriteLine("Path.GetMappedConnectionName()");
         
         GetRemoteNameInfo();
      }

      #endregion // GetMappedConnectionName

      #region GetMappedUncName

      [TestMethod]
      public void GetMappedUncName()
      {
         Console.WriteLine("Path.GetMappedUncName()");
         
         GetRemoteNameInfo();
      }

      #endregion // GetMappedUncName

      #region GetRegularPath

      [TestMethod]
      public void GetRegularPath()
      {
         Console.WriteLine("Path.GetRegularPath()");

         Directory.SetCurrentDirectory(StartupFolder);
         string longPath = Path.PrefixLongPath(Path.GetFullPath(Directory.GetCurrentDirectory()));
         Console.WriteLine("\n\tLong Path: [{0}]", longPath);

         string regularPath = Path.GetRegularPath(longPath);
         bool isRegularPath = !string.IsNullOrEmpty(regularPath) && !regularPath.StartsWith(Path.LongPathPrefix);
         bool endsWithCurrentFolder = regularPath.EndsWith(StartupFolder);
         Console.WriteLine("\n\tGetRegularPath() == [{0}]", regularPath);
         Console.WriteLine("\n\tGetRegularPath() == [{0}]: {1}", TextTrue, isRegularPath);

         Assert.IsTrue(isRegularPath);
         Assert.IsTrue(endsWithCurrentFolder);
      }

      #endregion // GetRegularPath

      #region GetShort83Path

      [TestMethod]
      public void GetShort83Path()
      {
         Console.WriteLine("Path.GetShort83Path()");

         GetLongFrom83Path();
      }

      #endregion // GetShort83Path
      
      #region IsLogicalDrive

      [TestMethod]
      public void IsLogicalDrive()
      {
         Console.WriteLine("Path.IsLogicalDrive()\n");

         // True
         string letterString = SysDrive[0].ToString();
         bool isLogicalDrive = Path.IsLogicalDrive(letterString);
         Console.WriteLine("\tIsLogicalDrive(\"{0}\") == [{1}]: {2}\n", letterString, TextTrue, isLogicalDrive);

         // False
         letterString = @"\\" + letterString;
         bool isNotLogicalDrive = !Path.IsLogicalDrive(letterString);
         Console.WriteLine("\tIsLogicalDrive(\"{0}\") == [{1}]: {2}\n\n", letterString, TextFalse, isNotLogicalDrive);

         Assert.IsTrue(isLogicalDrive);
         Assert.IsTrue(isNotLogicalDrive);
      }

      #endregion // IsLogicalDrive

      #region IsLongPath

      [TestMethod]
      public void IsLongPath()
      {
         Console.WriteLine("Path.IsLongPath()\n");

         string fullPath = Path.GetFullPath(Directory.GetCurrentDirectory());
         string longPath = Path.PrefixLongPath(fullPath);

         // True
         bool isLongPath = Path.IsLongPath(longPath) && longPath.StartsWith(Path.LongPathPrefix);
         Console.WriteLine("Input Path: [{0}]\n", longPath);
         Console.WriteLine("IsLongPath({0}) == [{1}]: {2}\n\n", "fullPath", TextTrue, isLongPath);

         // False
         fullPath = Path.GetFullPath(Directory.GetCurrentDirectory());
         bool isNotLongPath = !Path.IsLongPath(fullPath) && !fullPath.StartsWith(Path.LongPathPrefix);
         Console.WriteLine("Input Path: [{0}]\n", fullPath);
         Console.WriteLine("IsLongPath({0}) == [{1}]: {2}\n", "fullPath", TextFalse, isNotLongPath);

         Assert.IsTrue(isLongPath);
         Assert.IsTrue(isNotLongPath);
      }

      #endregion // IsLongPath

      #region IsUnc

      [TestMethod]
      public void IsUnc()
      {
         Console.WriteLine("Path.IsUnc()\n");

         string serverShare = "server" + Path.DirectorySeparatorChar + "share";

         // True
         string unc1 = Path.UncPrefix + serverShare;
         string unc2 = Path.LongPathUncPrefix + serverShare;
         string unc3 = Path.DosDeviceUncPrefix + serverShare;
         string unc4 = Path.DosDeviceLanmanPrefix + ";" + serverShare;
         string unc7 = Path.DosDeviceMupPrefix + ";" + serverShare;
         bool isUnc1 = Path.IsUnc(unc1);
         bool isUnc2 = Path.IsUnc(unc2);

         // Only retrieve this information if we're dealing with a real Network share mapping.

         bool isUnc3 = Path.IsUnc(unc3) == false;
         bool isUnc4 = Path.IsUnc(unc4);
         bool isUnc7 = Path.IsUnc(unc7);

         Console.WriteLine("\tIsUnc([{0}]) == [{1}]: {2}\n Prefix: {3}\n", unc1, TextTrue, isUnc1, "Path.UncPrefix");
         Console.WriteLine("\tIsUnc([{0}]) == [{1}]: {2}\n Prefix: {3}\n", unc2, TextTrue, isUnc2, "Path.LongPathUncPrefix");
         Console.WriteLine("\tIsUnc([{0}]) == [{1}]: {2}\n Prefix: {3}\n", unc3, TextFalse, isUnc3, "Path.DosDeviceUncPrefix");
         Console.WriteLine("\tIsUnc([{0}]) == [{1}]: {2}\n Prefix: {3} + \";\"\n", unc4, TextTrue, isUnc4, "Path.DosDeviceLanmanPrefix");
         Console.WriteLine("\tIsUnc([{0}]) == [{1}]: {2}\n Prefix: {3} + \";\"\n", unc7, TextTrue, isUnc7, "Path.DosDeviceMupPrefix");

         // False
         string unc5 = Environment.SystemDirectory;
         string unc6 = Path.LongPathPrefix + unc5;
         bool isUnc5 = !Path.IsUnc(unc5);
         bool isUnc6 = !Path.IsUnc(unc6);

         Console.WriteLine("");
         Console.WriteLine("IsUnc([{0}]) == [{1}]: {2}\n", unc5, TextFalse, isUnc5);
         Console.WriteLine("IsUnc([{0}]) == [{1}]: {2}\n", unc6, TextFalse, isUnc6);

         Assert.IsTrue(isUnc1);
         Assert.IsTrue(isUnc2);
         Assert.IsTrue(isUnc3);
         Assert.IsTrue(isUnc4);
         Assert.IsTrue(isUnc7);
         Assert.IsTrue(isUnc5);
         Assert.IsTrue(isUnc6);
      }

      #endregion // IsUnc

      #region IsValidName

      [TestMethod]
      public void IsValidName()
      {
         Console.WriteLine("Path.IsValidName()\n");

         // True
         bool isValid = Path.IsValidName(ValidName);
         Console.WriteLine("IsValidName([{0}]) == [{1}]: {2}\n", ValidName, TextTrue, isValid);

         // False
         bool isInvalid = !Path.IsValidName(InValidName);
         Console.WriteLine("IsValidName([{0}]) == [{1}]: {2}\n(Illegal characters in name)", InValidName, TextFalse, isInvalid);

         Assert.IsTrue(isValid);
         Assert.IsTrue(isInvalid);
      }

      #endregion // IsValidName

      #region IsValidPath

      [TestMethod]
      public void IsValidPath()
      {
         Console.WriteLine("Path.IsValidPath()\n");

         // True
         bool isValidPath1 = Path.IsValidPath(ValidName);
         bool isValidPath2 = Path.IsValidPath(WildcardValidNames, true);
         Console.WriteLine("IsValidPath([{0}]) == [{1}]: {2}\n", ValidName, TextTrue, isValidPath1);
         Console.WriteLine("IsValidPath([{0}]) == [{1}]: {2}\n(Wildcard enabled)\n", WildcardValidNames, TextTrue, isValidPath2);

         // False
         bool isNotValidPath = !Path.IsValidPath(WildcardValidNames);
         Console.WriteLine("IsValidPath([{0}]) == [{1}]: {2}\n(Wildcard disabled)", WildcardValidNames, TextFalse, isNotValidPath);

         Assert.IsTrue(isValidPath1);
         Assert.IsTrue(isValidPath2);
         Assert.IsTrue(isNotValidPath);
      }

      #endregion // IsValidPath

      #region LocalToUnc

      [TestMethod]
      public void LocalToUnc()
      {
         string localPath = SysRoot + @"\A sub Folder";
         Console.WriteLine("Path.LocalToUnc()");
         Console.WriteLine("\nIf your mapped network drives don't show up, take a look at this: {0}\n", "http://alphafs.codeplex.com/discussions/397693");

         string uncPath = Path.LocalToUnc(localPath);
         bool hasUncPath = !string.IsNullOrEmpty(uncPath) && Path.IsUnc(uncPath);
         Console.WriteLine("\tLocal Path: [{0}]", localPath);
         Console.WriteLine("\tNetwork share Path: [{0}]\n", uncPath);

         // Get Logical Drives from Local Host, .IsReady Drives only, strip trailing backslash.
         foreach (string drive in Directory.GetLogicalDrives(false, true, true))
         {
            StopWatcher(true);

            string pathUnc = Path.LocalToUnc(drive);

            Console.WriteLine("\tLogical Drive: [{0}] ==\t{1, -30}\n{2}\n", drive, pathUnc, Reporter());
         }

         Assert.IsTrue(hasUncPath);
      }

      #endregion // LocalToUnc

      #region PrefixLongPath

      [TestMethod]
      public void PrefixLongPath()
      {
         Console.WriteLine("Path.PrefixLongPath()");
         Console.WriteLine("\n\tDirectory: [{0}]", SysRoot);

         string folderName = Path.GetFileName(SysRoot);
         string longPath = Path.PrefixLongPath(SysRoot);
         Console.WriteLine("\n\tMakeLongPath(): [{0}]", longPath);
         Assert.IsTrue(longPath.StartsWith(Path.LongPathPrefix));
         Assert.IsTrue(longPath.EndsWith(SysRoot));
         Assert.IsTrue(Directory.Exists(longPath));

         string longPathUnc = Path.LocalToUnc(SysRoot);
         longPathUnc = Path.PrefixLongPath(longPathUnc);
         if (Directory.Exists(longPathUnc))
         {
            Console.WriteLine("\n\tMakeLongPath() (UNC): [{0}]", longPathUnc);
            Assert.IsTrue(longPathUnc.StartsWith(Path.LongPathUncPrefix));
            Assert.IsTrue(longPathUnc.EndsWith(folderName));
            Assert.IsTrue(Directory.Exists(longPathUnc));
         }
         else
            Assert.IsTrue(false, "Share inaccessible: {0}", longPathUnc);
      }

      #endregion // PrefixLongPath

      #region Directory

      #region GetDirectoryNameWithoutRoot

      [TestMethod]
      public void GetDirectoryNameWithoutRoot()
      {
         Console.WriteLine("Path.GetDirectoryNameWithoutRoot()");

         DumpGetDirectoryNameWithoutRoot(true);
         DumpGetDirectoryNameWithoutRoot(false);
      }

      #endregion // GetDirectoryNameWithoutRoot

      #region GetSuffixedDirectoryName

      [TestMethod]
      public void GetSuffixedDirectoryName()
      {
         Console.WriteLine("Path.GetSuffixedDirectoryName()");

         DumpGetSuffixedDirectoryName(true);
         DumpGetSuffixedDirectoryName(false);
      }

      #endregion // GetSuffixedDirectoryName

      #region GetSuffixedDirectoryNameWithoutRoot

      [TestMethod]
      public void GetSuffixedDirectoryNameWithoutRoot()
      {
         Console.WriteLine("Path.GetSuffixedDirectoryNameWithoutRoot()");

         DumpGetSuffixedDirectoryNameWithoutRoot(true);
         DumpGetSuffixedDirectoryNameWithoutRoot(false);
      }

      #endregion // GetSuffixedDirectoryNameWithoutRoot

      #endregion // Directory

      #region File

      [TestMethod]
      public void GetFinalPathNameByHandle()
      {
         Console.WriteLine("Path.GetFinalPathNameByHandle()");

         DumpGetFinalPathNameByHandle(false);
      }

      [TestMethod]
      public void GetFinalPathNameByHandleX3__internal()
      {
         Console.WriteLine("Path.GetFinalPathNameByHandleX3() - {0}", SpecificX3);

         DumpGetFinalPathNameByHandle(true);
      }

      

      #endregion // File
      

      #region Utility

      [TestMethod]
      public void DirectorySeparatorAdd()
      {
         Console.WriteLine("Path.DirectorySeparatorAdd()\n");

         const string nonSlashedString = "SlashMe";
         Console.WriteLine("\tstring: [{0}];\n", nonSlashedString);

         // True, add DirectorySeparatorChar.
         string hasBackslash = Path.DirectorySeparatorAdd(nonSlashedString, false);
         bool addedBackslash = hasBackslash.EndsWith(Path.DirectorySeparatorChar.ToString()) && (nonSlashedString + Path.DirectorySeparatorChar).Equals(hasBackslash);
         Console.WriteLine("\tDirectorySeparatorAdd(string);\n\tAdded == [{0}]: {1}\n\tResult: [{2}]\n", TextTrue, addedBackslash, hasBackslash);

         // True, add AltDirectorySeparatorChar.
         string hasSlash = Path.DirectorySeparatorAdd(nonSlashedString, true);
         bool addedSlash = hasSlash.EndsWith(Path.AltDirectorySeparatorChar.ToString(CultureInfo.CurrentCulture)) && (nonSlashedString + Path.AltDirectorySeparatorChar).Equals(hasSlash);
         Console.WriteLine("\tDirectorySeparatorAdd(string, true);\n\tAdded == [{0}]: {1}\n\tResult: [{2}]\n", TextTrue, addedSlash, hasSlash);

         Assert.IsTrue(addedBackslash);
         Assert.IsTrue(addedSlash);
      }

      [TestMethod]
      public void DirectorySeparatorRemove()
      {
         Console.WriteLine("Path.DirectorySeparatorRemove()\n");

         const string backslashedString = @"Backslashed\";
         const string slashedString = "Slashed/";
         // True, add DirectorySeparatorChar.
         string hasBackslash = Path.DirectorySeparatorRemove(backslashedString);
         bool removedBackslash = !hasBackslash.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.CurrentCulture)) && !backslashedString.Equals(hasBackslash);
         Console.WriteLine("\tstring = @[{0}];\n", backslashedString);
         Console.WriteLine("\tDirectorySeparatorRemove(string);\n\tRemoved == [{0}]: {1}\n\tResult: [{2}]\n", TextTrue, removedBackslash, hasBackslash);

         // True, add AltDirectorySeparatorChar.
         string hasSlash = Path.DirectorySeparatorRemove(slashedString, true);
         bool removedSlash = !hasSlash.EndsWith(Path.AltDirectorySeparatorChar.ToString(CultureInfo.CurrentCulture)) && !slashedString.Equals(hasSlash);
         Console.WriteLine("\tstring: [{0}];\n", slashedString);
         Console.WriteLine("\tDirectorySeparatorRemove(string, true);\n\tRemoved == [{0}]: {1}\n\tResult: [{2}]\n", TextTrue, removedSlash, hasSlash);

         Assert.IsTrue(removedBackslash);
         Assert.IsTrue(removedSlash);
      }

      #endregion // Utility

      #endregion // AlphaFS
   }
}