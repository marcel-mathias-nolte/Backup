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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using Alphaleonis.Win32.Filesystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using File = Alphaleonis.Win32.Filesystem.File;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace AlphaFS.UnitTest
{
   /// <summary>This is a test class for File and is intended to contain all File Unit Tests.</summary>
   [TestClass]
   public class FileTest
   {
      #region FileTest Helpers

      private static readonly string StartupFolder = AppDomain.CurrentDomain.BaseDirectory;
      private static readonly string SysDrive = Environment.GetEnvironmentVariable("SystemDrive");
      private static readonly string SysRoot = Environment.GetEnvironmentVariable("SystemRoot");
      private static readonly string SysRoot32 = Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "System32");

      private const string SpecificX3 = "Windows XP and Windows Server 2003 specific.";
      private const string TextTrue = "IsTrue";
      private const string TextFalse = "IsFalse";
      private const string TenNumbers = "0123456789";
      private static readonly string TextHelloWorld = "Hëllõ Wørld!-" + Path.GetRandomFileName();
      private static readonly string TextGoodByeWorld = "GóödByé Wôrld!-" + Path.GetRandomFileName();
      private const string TextAppend = "GóödByé Wôrld!";
      private const string TextUnicode = " - ÛņïÇòdè; ǖŤƑ";
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

         if (_stopWatcher.IsRunning)
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

      private static bool IsAdmin()
      {
         bool isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

         if (!isAdmin)
            Console.WriteLine("\n\tThis Unit Test must be run as Administrator.");

         return isAdmin;
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
      private static void DumpAccessRules(int cntCheck, FileSecurity dsSystem, FileSecurity dsAlpha)
      {
         Console.WriteLine("\n\tSanity check {0}.", cntCheck);
         StopWatcher(true);
         Console.WriteLine("\t\tFile.GetAccessControl().AreAccessRulesProtected: [{0}]\n\t{1}", dsAlpha.AreAccessRulesProtected, Reporter());
         Assert.AreEqual(dsAlpha.AreAccessRulesProtected, dsSystem.AreAccessRulesProtected);

         StopWatcher(true);
         Console.WriteLine("\t\tFile.GetAccessControl().AreAuditRulesProtected: [{0}]\n\t{1}", dsAlpha.AreAuditRulesProtected, Reporter());
         Assert.AreEqual(dsAlpha.AreAuditRulesProtected, dsSystem.AreAuditRulesProtected);

         StopWatcher(true);
         Console.WriteLine("\t\tFile.GetAccessControl().AreAccessRulesCanonical: [{0}]\n\t{1}", dsAlpha.AreAccessRulesCanonical, Reporter());
         Assert.AreEqual(dsAlpha.AreAccessRulesCanonical, dsSystem.AreAccessRulesCanonical);

         StopWatcher(true);
         Console.WriteLine("\t\tFile.GetAccessControl().AreAuditRulesCanonical: [{0}]\n\t{1}", dsAlpha.AreAuditRulesCanonical, Reporter());
         Assert.AreEqual(dsAlpha.AreAuditRulesCanonical, dsSystem.AreAuditRulesCanonical);
      }
      private static byte[] StringToByteArray(string str, params Encoding[] encoding)
      {
         Encoding encode = encoding != null && encoding.Any() ? encoding[0] : new UTF8Encoding(true, true);
         return encode.GetBytes(str);
      }

      private static void Dump__Class_ByHandleFileInfo(bool isLocal)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");
         string path = isLocal ? Path.GetTempFileName() : Path.LocalToUnc(Path.GetTempFileName());

         FileStream stream = File.Create(path);

         stream.WriteByte(1);

         string streamName = stream.Name;

         StopWatcher(true);
         Console.WriteLine("\n\tFilestream: [{0}]\n{1}", path, Reporter());
         Console.WriteLine("\n\tFilestream.Name == [{0}]", streamName);

         ByHandleFileInfo info = File.GetFileInformationByHandle(stream);
         Assert.IsTrue(Dump(info, -18));

         stream.Dispose();

         Assert.AreEqual(info.CreationTime, System.IO.File.GetCreationTime(path));
         Assert.AreEqual(info.LastAccessTime, System.IO.File.GetLastAccessTime(path));
         Assert.AreEqual(info.LastWriteTime.ToString(), System.IO.File.GetLastWriteTime(path).ToString()); // Curious

         Assert.IsTrue(File.Delete(path));
      }

      private static void DumpGetAttributes(bool isLocal)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");
         string path = isLocal ? SysRoot : Path.LocalToUnc(SysRoot);

         int cnt = 0;
         StopWatcher(true);
         foreach (string file in Directory.EnumerateDirectories(path))
         {
            FileAttributes attrs = File.GetAttributes(file);
            FileAttributes attrsSysIO = System.IO.File.GetAttributes(file);
            Console.WriteLine("\t\t#{0:000}\tFile:\t[{1}]\t\tAttributes: [{2}]\t\tSysIO: [{3}]", ++cnt, file, attrs, attrsSysIO);
            Assert.IsTrue(cnt > 0);
            Assert.AreEqual(attrs, attrsSysIO);
         }
         Console.WriteLine("\n\t{0}\n", Reporter());
      }

      private static void DumpGetCreationTime(bool isLocal)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");
         string path = Path.Combine(SysRoot, "notepad.exe");
         if (!isLocal)
            path = Path.LocalToUnc(path);

         Console.WriteLine("\n\tInput Path: [{0}]\n", path);

         try
         {
            DateTime actual = File.GetCreationTime(path);
            DateTime expected = System.IO.File.GetCreationTime(path);
            Console.WriteLine("\t\tGetCreationTime(): [{0}]\t\tSystem.IO: [{1}]", actual, expected);
            Assert.AreEqual(actual, expected, "File.GetCreationTime()");

            actual = File.GetCreationTimeUtc(path);
            expected = System.IO.File.GetCreationTimeUtc(path);
            Console.WriteLine("\t\tGetCreationTimeUtc(): [{0}]\t\tSystem.IO: [{1}]\n", actual, expected);
            Assert.AreEqual(actual, expected, "File.GetCreationTimeUtc()");

            actual = File.GetLastAccessTime(path);
            expected = System.IO.File.GetLastAccessTime(path);
            Console.WriteLine("\t\tGetLastAccessTime(): [{0}]\t\tSystem.IO: [{1}]", actual, expected);
            Assert.AreEqual(actual, expected, "File.GetLastAccessTime()");

            actual = File.GetLastAccessTimeUtc(path);
            expected = System.IO.File.GetLastAccessTimeUtc(path);
            Console.WriteLine("\t\tGetLastAccessTimeUtc(): [{0}]\t\tSystem.IO: [{1}]\n", actual, expected);
            Assert.AreEqual(actual, expected, "File.GetLastAccessTimeUtc()");

            actual = File.GetLastWriteTime(path);
            expected = System.IO.File.GetLastWriteTime(path);
            Console.WriteLine("\t\tGetLastWriteTime(): [{0}]\t\tSystem.IO: [{1}]", actual, expected);
            Assert.AreEqual(actual, expected, "File.GetLastWriteTime()");

            actual = File.GetLastWriteTimeUtc(path);
            expected = System.IO.File.GetLastWriteTimeUtc(path);
            Console.WriteLine("\t\tGetLastWriteTimeUtc() : [{0}]\t\tSystem.IO: [{1}]", actual, expected);
            Assert.AreEqual(actual, expected, "File.GetLastWriteTimeUtc()");
         }
         catch (Exception ex)
         {
            Console.WriteLine("\n\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));
         }
      }

      private static void DumpEnumerateStreams(bool isLocal)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");
         string path = Path.Combine(Path.GetTempPath(), "File.EnumerateStreams()-" + Path.GetRandomFileName());
         if (!isLocal)
            path = Path.LocalToUnc(path);

         // Create file; unnamed stream 0.
         File.WriteAllText(path, TextHelloWorld);
         string txtFromStream = File.ReadAllText(path);

         Console.WriteLine("\n\t\tStream content: [{0}]", txtFromStream);

         // Create alternate stream.
         string extraStream = ":ThisIsAnExtraStream-" + (isLocal ? "Local" : "Network");
         string txtToStream = TextAppend + " - Extra Stream Information (" + (isLocal ? "Local" : "Network") + ")";
         File.WriteAllText(path + extraStream, txtToStream);

         Console.WriteLine("\n\tAdded stream, name: [{0}]", extraStream);
         txtFromStream = File.ReadAllText(path + extraStream);
         Console.WriteLine("\n\t\tStream content: [{0}]", txtFromStream);
         Assert.AreEqual(txtToStream, txtFromStream);

         StopWatcher(true);
         foreach (BackupStreamInfo fs in File.EnumerateStreams(path))
            Assert.IsTrue(Dump(fs, -10));
         Console.WriteLine("\n\t{0}\n", Reporter());

         Assert.IsTrue(File.Delete(path));
      }

      private static void DumpCopy(bool isLocal)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");
         string path = isLocal ? SysRoot : Path.LocalToUnc(SysRoot);

         string tempPath = Path.GetTempPath("File-Copy-" + Path.GetRandomFileName());
         if (!isLocal)
            tempPath = Path.LocalToUnc(tempPath);

         if (!Directory.Exists(tempPath))
            Directory.CreateDirectory(tempPath);

         #region Copy

         Console.WriteLine("\n\tInput Path: [{0}]\n", path);
         int cnt = 0;
         StopWatcher(true);
         foreach (string file in Directory.EnumerateFiles(path))
         {
            string newFile = Path.Combine(tempPath, Path.GetFileName(file));
            File.Copy(file, newFile);
            Console.WriteLine("\t\t#{0:000}\tCopy: [{1}]", ++cnt, newFile);
            Assert.IsTrue(File.Exists(newFile));
         }
         Console.WriteLine("\n\t\tTotal Size: [{0}]{1}", NativeMethods.UnitSizeToText(Directory.GetProperties(tempPath)["Size"]), Reporter());

         #endregion // Copy

         #region Exception Test

         // Exception: Alphaleonis.Win32.Filesystem.AlreadyExistsException: Path already exists.

         bool exception = false;
         foreach (string file in Directory.EnumerateFiles(path))
         {
            string newFile = Path.Combine(tempPath, Path.GetFileName(file));

            try
            {
               StopWatcher(true);
               File.Copy(file, newFile);
               break;
            }
            catch (AlreadyExistsException ex)
            {
               exception = true;
               Console.WriteLine("\n\t\tException: [{0}]", ex.Message);
               break;
            }
            catch (Exception)
            {
               exception = false;
               break;
            }
         }
         Console.WriteLine("\n\t\tCaught Exception: [{0}]", exception);
         Assert.IsTrue(exception);

         #endregion // Exception Test
         
         Assert.IsTrue(Directory.Delete(tempPath, true, true));
      }

      private static void DumpMove(bool isLocal)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");
         string path = isLocal ? SysRoot : Path.LocalToUnc(SysRoot);

         string tempPath = Path.GetTempPath("File-Move-" + Path.GetRandomFileName());
         if (!isLocal)
            tempPath = Path.LocalToUnc(tempPath);

         if (!Directory.Exists(tempPath))
            Directory.CreateDirectory(tempPath);

         #region Move

         #region Copy (For safety)

         foreach (string file in Directory.EnumerateFiles(path))
         {
            string newFile = Path.Combine(tempPath, Path.GetFileName(file));
            File.Copy(file, newFile);
            Assert.IsTrue(File.Exists(newFile));
         }

         #endregion // Copy (For safety)

         string movePath = Path.GetTempPath("File-Move-2-" + Path.GetRandomFileName());
         if (!Directory.Exists(movePath))
            Directory.CreateDirectory(movePath);

         Console.WriteLine("\n\tInput Path: [{0}]\n", movePath);
         int cnt = 0;
         StopWatcher(true);
         foreach (string file in Directory.EnumerateFiles(tempPath))
         {
            string newFile = Path.Combine(movePath, Path.GetFileName(file));
            File.Move(file, newFile);
            Console.WriteLine("\t\t#{0:000}\tMove: [{1}]", ++cnt, newFile);
            Assert.IsTrue(File.Exists(newFile));
         }
         Console.WriteLine("\n\t\tTotal Size: [{0}]{1}", NativeMethods.UnitSizeToText(Directory.GetProperties(movePath)["Size"]), Reporter());

         #endregion // Move

         #region Exception Test

         #region Copy (For safety)

         foreach (string file in Directory.EnumerateFiles(path))
         {
            string newFile = Path.Combine(tempPath, Path.GetFileName(file));
            File.Copy(file, newFile);
            Assert.IsTrue(File.Exists(newFile));
         }

         #endregion // Copy (For safety)

         // Exception: Alphaleonis.Win32.Filesystem.AlreadyExistsException: Path already exists.

         bool exception = false;
         foreach (string file in Directory.EnumerateFiles(tempPath))
         {
            string newFile = Path.Combine(movePath, Path.GetFileName(file));

            try
            {
               StopWatcher(true);
               File.Move(file, newFile);
               break;
            }
            catch (AlreadyExistsException ex)
            {
               exception = true;
               Console.WriteLine("\n\t\tException: [{0}]", ex.Message);
               break;
            }
            catch (Exception)
            {
               exception = false;
               break;
            }
         }
         Console.WriteLine("\n\t\tCaught Exception: [{0}]", exception);
         Assert.IsTrue(exception);

         #endregion // Exception Test

         Assert.IsTrue(Directory.Delete(movePath, true, true));
         Assert.IsTrue(Directory.Delete(tempPath, true, true));
      }

      #endregion // FileTest Helpers

      #region .NET

      #region AppendAllLines

      [TestMethod]
      public void AppendAllLines()
      {
         Console.WriteLine("File.AppendAllLines()");
         Console.WriteLine("\n\tDefault AlphaFS Encoding: [{0}]", NativeMethods.DefaultFileEncoding.EncodingName);

         // Create file and append text.
         string tempFile = Path.GetTempFileName();

         IEnumerable<string> allLines = new[] { TenNumbers, TextHelloWorld, TextAppend, TextUnicode };

         // Create real UTF-8 file.
         File.AppendAllLines(tempFile, allLines, NativeMethods.DefaultFileEncoding);

         // Read filestream contents.
         using (StreamReader streamRead = File.OpenText(tempFile))
         {
            string line = streamRead.ReadToEnd();

            Console.WriteLine("\n\tCreated: [{0}] filestream: [{1}]\n\n\tAppendAllLines content:\n{2}", streamRead.CurrentEncoding.EncodingName, tempFile, line);

            foreach (string line2 in allLines)
               Assert.IsTrue(line.Contains(line2));
         }

         // Append
         File.AppendAllLines(tempFile, new[] { "Append 1" });
         File.AppendAllLines(tempFile, allLines);
         File.AppendAllLines(tempFile, new[] { "Append 2" });
         File.AppendAllLines(tempFile, allLines);

         // Read filestream contents.
         using (StreamReader streamRead = File.OpenText(tempFile))
         {
            string line = streamRead.ReadToEnd();

            Console.WriteLine("\tAppendAllLines content:\n{0}", line);

            foreach (string line2 in allLines)
               Assert.IsTrue(line.Contains(line2));
         }

         Assert.IsTrue(File.Delete(tempFile, true));
      }

      #endregion // AppendAllLines

      #region AppendAllText

      [TestMethod]
      public void AppendAllText()
      {
         Console.WriteLine("File.AppendAllText()");
         Console.WriteLine("\n\tDefault AlphaFS Encoding: [{0}]", NativeMethods.DefaultFileEncoding.EncodingName);

         // Create file and append text.
         string tempFile = Path.GetTempFileName();

         string allLines = TextHelloWorld;

         // Create real UTF-8 file.
         File.AppendAllText(tempFile, allLines, NativeMethods.DefaultFileEncoding);

         // Read filestream contents.
         using (StreamReader streamRead = File.OpenText(tempFile))
         {
            string line = streamRead.ReadToEnd();

            Console.WriteLine("\n\tCreated: [{0}] filestream: [{1}]\n\n\tAppendAllText content:\n{2}", streamRead.CurrentEncoding.EncodingName, tempFile, line);

            Assert.IsTrue(line.Contains(allLines));
         }

         // Append
         File.AppendAllText(tempFile, "Append 1");
         File.AppendAllText(tempFile, allLines);
         File.AppendAllText(tempFile, "Append 2");
         File.AppendAllText(tempFile, allLines);

         // Read filestream contents.
         using (StreamReader streamRead = File.OpenText(tempFile))
         {
            string line = streamRead.ReadToEnd();

            Console.WriteLine("\tAppendAllText content:\n{0}", line);

            Assert.IsTrue(line.Contains(allLines));
            Assert.IsTrue(line.Contains("Append 1"));
            Assert.IsTrue(line.Contains("Append 2"));
         }

         bool fileNotExists = File.Delete(tempFile, true);
         Console.WriteLine("\n\t(Deleted tempfile == [{0}]: {1})", TextTrue, fileNotExists);
         Assert.IsTrue(fileNotExists);
      }

      #endregion // AppendAllText

      #region AppendText

      [TestMethod]
      public void AppendText()
      {
         Console.WriteLine("File.AppendText()");
         Console.WriteLine("FileInfo.AppendText()");
         Console.WriteLine("\n\tDefault AlphaFS Encoding: [{0}]", NativeMethods.DefaultFileEncoding.EncodingName);

         string utf8 = NativeMethods.DefaultFileEncoding.BodyName.ToUpperInvariant();
         string line;
         string matchLine = string.Empty;
         string tempFile = Path.GetTempFileName();

         StreamReader streamRead;
         StreamWriter streamWrite;

         Console.WriteLine("Default AlphaFS Encoding: {0}", NativeMethods.DefaultFileEncoding.EncodingName);

         #region Create Filestream, CreateText()

         // Create filestream and append text as UTF-8, default.
         using (streamWrite = File.CreateText(tempFile))
         {
            streamWrite.Write(TextHelloWorld);
         }

         // Read filestream contents.
         using (streamRead = File.OpenText(tempFile))
         {
            while ((line = streamRead.ReadLine()) != null)
            {
               Console.WriteLine("\n Created: [{0}] filestream: [{1}]\n  Appended: [{2}]\n  Content : [{3}]", streamRead.CurrentEncoding.EncodingName, tempFile, TextHelloWorld, line);
               matchLine = line; // Catch the last line.
            }
         }
         Assert.IsTrue(matchLine.Equals(TextHelloWorld, StringComparison.OrdinalIgnoreCase));

         #endregion // Create Filestream, CreateText()

         #region AppendText() to Filestream

         // Append text as UTF-8, default.
         using (streamWrite = File.AppendText(tempFile))
         {
            streamWrite.Write(TextAppend);
         }

         // Read filestream contents.
         using (streamRead = File.OpenText(tempFile))
         {
            while ((line = streamRead.ReadLine()) != null)
            {
               Console.WriteLine("\n AppendText() as [{0}]\n  Appended: [{1}]\n  Content : [{2}]", utf8, TextAppend, line);
            }
         }

         // Append text as UTF-8, default.
         using (streamWrite = File.AppendText(tempFile))
         {
            streamWrite.WriteLine(TextUnicode);
         }

         // Read filestream contents.
         matchLine = string.Empty;
         using (streamRead = File.OpenText(tempFile))
         {
            while ((line = streamRead.ReadLine()) != null)
            {
               Console.WriteLine("\n AppendText() as [{0}]\n  Appended: [{1}]\n  Content : [{2}]", utf8, TextAppend, line);
               matchLine = line; // Catch the last line.
            }
         }

         Assert.IsTrue(matchLine.Equals(TextHelloWorld + TextAppend + TextUnicode, StringComparison.OrdinalIgnoreCase));

         bool fileNotExists = File.Delete(tempFile, true);
         Console.WriteLine("\n\t(Deleted tempfile == [{0}]: {1})", TextTrue, fileNotExists);
         Assert.IsTrue(fileNotExists);

         #endregion // AppendText() to Filestream
      }

      #endregion // AppendText

      #region Copy

      [TestMethod]
      public void Copy()
      {
         Console.WriteLine("File.Copy()");

         DumpCopy(true);
         DumpCopy(false);
      }

      #endregion // Copy

      #region Create

      [TestMethod]
      public void Create()
      {
         Console.WriteLine("File.Create()");
         Console.WriteLine("FileInfo.Create()");

         bool fileExists;
         int ten = TenNumbers.Length;
         string path = Path.GetTempPath("AlphaFS Create()");

         File.Delete(path);

         StopWatcher(true);

         using (FileStream stream = File.Create(path))
         {
            Console.WriteLine("\n\tFilestream: [{0}]{1}", path, Reporter());

            // According to NotePad++, creates a file type: "ANSI", which is reported as: "Unicode (UTF-8)".
            stream.Write(StringToByteArray(TenNumbers), 0, ten);

            long fileSize = stream.Length;
            bool isTen = fileSize == ten;

            Console.WriteLine("\n\tFilestream.Name   == [{0}]", stream.Name);
            Console.WriteLine("\n\tFilestream.Length == [{0}] == [{1}]: {2}", NativeMethods.UnitSizeToText(ten), TextTrue, isTen);

            fileExists = File.Exists(path) && stream.Length == ten;

            Assert.IsTrue(Dump(stream, -14));
            Assert.IsTrue(Dump(stream.SafeFileHandle, -9));
         }

         using (StreamReader stream = File.OpenText(path))
         {
            Console.WriteLine("\n\tEncoding: [{0}]", stream.CurrentEncoding.EncodingName);

            string line;
            while (!string.IsNullOrEmpty(line = stream.ReadLine()))
            {
               Console.WriteLine("\n\tContent : [{0}]", line);
            }
         }

         bool fileNotExists = File.Delete(path);
         Console.WriteLine("\n\t(Deleted tempfile == [{0}]: {1})", TextTrue, fileNotExists);

         Assert.IsTrue(fileExists);
         Assert.IsTrue(fileNotExists);
      }

      #endregion // Create

      #region CreateText

      [TestMethod]
      public void CreateText()
      {
         Console.WriteLine("File.CreateText()");
         Console.WriteLine("FileInfo.CreateText()\n");

         AppendText();
      }

      #endregion // CreateText

      #region Decrypt

      [TestMethod]
      public void Decrypt()
      {
         Console.WriteLine("File.Decrypt()");
         Console.WriteLine("FileInfo.Decrypt()");

         Encrypt();
      }

      #endregion // Decrypt

      #region Delete

      [TestMethod]
      public void Delete()
      {
         Console.WriteLine("File.Delete()");
         Console.WriteLine("FileInfo.Delete()");

         string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
         using (FileStream streamCreate = File.Create(path))
         {
            streamCreate.WriteByte(1);
            Console.WriteLine("\n\tInput Path: [{0}]\n\tfilestream.Length = [{1}]", path, NativeMethods.UnitSizeToText(streamCreate.Length));
         }
         Assert.IsTrue(File.Exists(path));

         
         FileStream streamCreate2 = File.Open(path, FileMode.Open);

         // Fail #1; Filestream still open.
         bool deleteOk = false;
         try
         {
            deleteOk = File.Delete(path, true);
         }
         catch (Exception ex)
         {
            Console.WriteLine("\n\tException: [{0}] == {1}", ex.Message, TextTrue);
         }
         Assert.IsFalse(deleteOk);
         Console.WriteLine("\n\tDelete() failed True: [{0}]\n\t(Filestream still open)", deleteOk == false);

         streamCreate2.Close();

         // Fail #2; File ReadOnly attribute is set, forceOverrideReadOnly = false.
         bool isReadOnly = File.SetAttributes(path, FileAttributes.ReadOnly);
         Assert.IsTrue(isReadOnly);

         deleteOk = File.Delete(path);
         Assert.IsFalse(deleteOk);

         bool success = File.Exists(path);
         Assert.IsTrue(success);

         Console.WriteLine("\n\tDelete() failed True: [{0}]\n\t(isReadOnly Attribute set: [{1}])", deleteOk, isReadOnly);

         // Success, forceOverrideReadOnly = true.
         deleteOk = File.Delete(path, true);
         Console.WriteLine("\n\tDelete() successful True: [{0}]", deleteOk);
         Assert.IsTrue(deleteOk);
      }

      #endregion // Delete

      #region Encrypt

      [TestMethod]
      public void Encrypt()
      {
         Console.WriteLine("File.Encrypt()");
         Console.WriteLine("FileInfo.Encrypt()");

         // Create file and append text.
         string tempFile = Path.GetTempFileName();

         // Append text as UTF-8, default.
         File.AppendAllText(tempFile, TextHelloWorld);

         string utf8 = NativeMethods.DefaultFileEncoding.BodyName.ToUpperInvariant();
         string readText8 = File.ReadAllText(tempFile);
         FileAttributes attrs = File.GetAttributes(tempFile);
         FileEncryptionStatus encryptionStatus = File.GetEncryptionStatus(tempFile);
         Console.WriteLine("\n\tCreated {0} file: [{1}]", utf8, tempFile);
         Console.WriteLine("\tContent == [{0}]", readText8);
         Console.WriteLine("\n\tFile.GetAttributes() == [{0}]", attrs);
         Console.WriteLine("\tEncryption status    == [{0}]", encryptionStatus);

         bool encryptOk = File.Encrypt(tempFile);
         attrs = File.GetAttributes(tempFile);
         encryptionStatus = File.GetEncryptionStatus(tempFile);
         Console.WriteLine("\n\tFile.Encrypt() successful == [{0}]: {1}", TextTrue, encryptOk);
         Console.WriteLine("\tFile.GetAttributes()      == [{0}]", attrs);
         Console.WriteLine("\tEncryption status         == [{0}]", encryptionStatus);

         bool decryptOk = File.Decrypt(tempFile);
         attrs = File.GetAttributes(tempFile);
         FileEncryptionStatus decryptionStatus = File.GetEncryptionStatus(tempFile);
         Console.WriteLine("\n\tFile.Decrypt() successful == [{0}]: {1}", TextTrue, decryptOk);
         Console.WriteLine("\tFile.GetAttributes()      == [{0}]", attrs);
         Console.WriteLine("\tDecryption status         == [{0}]", decryptionStatus);

         bool fileNotExists = File.Delete(tempFile, true);
         Console.WriteLine("\n\t(Deleted tempfile == [{0}]: {1})", TextTrue, fileNotExists);
         Assert.IsTrue(fileNotExists);

         Assert.IsTrue(encryptOk);
         Assert.IsTrue(encryptionStatus == FileEncryptionStatus.Encrypted);
         Assert.IsTrue(decryptOk);
         Assert.IsTrue(decryptionStatus == FileEncryptionStatus.Encryptable);
         Assert.IsTrue(fileNotExists);
      }

      #endregion // Encrypt

      #region Exists

      [TestMethod]
      public void Exists()
      {
         Console.WriteLine("File.Exists()");

         string path = Path.GetTempPath("AlphaFS Exists()");

         string streamName;

         using (FileStream stream = File.Create(path))
         {
            stream.WriteByte(1);
            streamName = stream.Name;
         }

         Console.WriteLine("\n\tFilestream: [{0}]", path);
         Console.WriteLine("\n\tFilestream.Name == [{0}]\n", streamName);

         StopWatcher(true);

         bool fileExists = File.Exists(path);

         Console.WriteLine("\n\tExists() == [{0}]: {1}{2}", TextTrue, fileExists, Reporter());

         bool fileNotExists = File.Delete(path);
         Console.WriteLine("\n\t(Deleted tempfile == [{0}]: {1})", TextTrue, fileNotExists);

         Assert.IsTrue(fileExists);
         Assert.IsTrue(fileNotExists);
      }

      #endregion // Exists

      #region GetAccessControl

      [TestMethod]
      public void GetAccessControl()
      {
         Console.WriteLine("File.GetAccessControl()");
         Console.WriteLine("FileInfo.GetAccessControl()");

         FileSecurity gac;
         AuthorizationRuleCollection accessRules;

         string path = Path.Combine(Path.GetTempPath(), "File.GetAccessControl()-" + Path.GetRandomFileName());
         using (File.Create(path)) { }

         #region Local

         bool foundRules = false;
         Console.WriteLine("\n\tLocal: [{0}]", path);

         StopWatcher(true);
         gac = File.GetAccessControl(path);
         StopWatcher();
         accessRules = gac.GetAccessRules(true, true, typeof(NTAccount));

         Console.WriteLine("\n\tFile.GetAccessControl() rules found: [{0}]\"\n{1}", accessRules.Count, Reporter());

         foreach (FileSystemAccessRule far in accessRules)
         {
            Assert.IsTrue(Dump(far, 17));
            foundRules = true;
         }

         Assert.IsTrue(foundRules);

         #endregion // Local

         #region Network

         foundRules = false;
         path = Path.LocalToUnc(path);
         FileInfo fileInfo = new FileInfo(path);

         if (fileInfo.Exists)
         {
            StopWatcher(true);
            gac = fileInfo.GetAccessControl();
            StopWatcher();
            accessRules = gac.GetAccessRules(true, true, typeof(NTAccount));

            Console.WriteLine("\n\n\tNetwork: [{0}]", fileInfo.FullName);
            Console.WriteLine("\n\tfileInfo.GetAccessControl() rules found: [{0}]\n{1}", accessRules.Count, Reporter());

            foreach (FileSystemAccessRule far in accessRules)
            {
               Dump(far, 17);
               foundRules = true;
            }
            Assert.IsTrue(foundRules);
         }
         else
            Console.Write("\n\tShare inaccessible: {0}\n", path);

         #endregion // Network

         bool fileNotExists = fileInfo.Delete();
         Console.WriteLine("\n\t(Deleted tempfile == [{0}]: {1})", TextTrue, fileNotExists);
         Assert.IsTrue(fileNotExists);
      }

      #endregion // GetAccessControl

      #region GetAttributes

      [TestMethod]
      public void GetAttributes()
      {
         Console.WriteLine("File.GetAttributes()");

         DumpGetAttributes(true);
         DumpGetAttributes(false);
      }

      #endregion // GetAttributes

      #region GetCreationTime

      [TestMethod]
      public void GetCreationTime()
      {
         Console.WriteLine("File.GetCreationTime()");

         DumpGetCreationTime(true);
         DumpGetCreationTime(false);
      }

      #endregion // GetCreationTime

      #region GetCreationTimeUtc

      [TestMethod]
      public void GetCreationTimeUtc()
      {
         Console.WriteLine("File.GetCreationTimeUtc()");

         GetCreationTime();
      }

      #endregion // GetCreationTimeUtc

      #region GetLastAccessTime

      [TestMethod]
      public void GetLastAccessTime()
      {
         Console.WriteLine("File.GetLastAccessTime()");

         GetCreationTime();
      }

      #endregion // GetLastAccessTime

      #region GetLastAccessTimeUtc

      [TestMethod]
      public void GetLastAccessTimeUtc()
      {
         Console.WriteLine("File.GetLastAccessTimeUtc()");

         GetCreationTime();
      }

      #endregion // GetLastAccessTimeUtc

      #region GetLastWriteTime

      [TestMethod]
      public void GetLastWriteTime()
      {
         Console.WriteLine("File.GetLastWriteTime()");

         GetCreationTime();
      }

      #endregion // GetLastWriteTime

      #region GetLastWriteTimeUtc

      [TestMethod]
      public void GetLastWriteTimeUtc()
      {
         Console.WriteLine("File.GetLastWriteTimeUtc()");

         GetCreationTime();
      }

      #endregion // GetLastWriteTimeUtc

      #region Move

      [TestMethod]
      public void Move()
      {
         Console.WriteLine("File.Move()");

         DumpMove(true);
         DumpMove(false);
      }

      #endregion // Move

      #region ReadAllLines

      [TestMethod]
      public void ReadAllLines()
      {
         Console.WriteLine("File.ReadAllLines()\n");

         // Create file and append text.
         string tempFile = Path.GetTempFileName();

         string[] createText = { "Hello", "And", "Welcome" };
         File.WriteAllLines(tempFile, createText);

         // Open the file to read from. 
         string[] readText = File.ReadAllLines(tempFile);
         foreach (string s in readText)
         {
            Console.WriteLine(s);
            Assert.IsTrue(createText.Contains(s));
         }

         bool fileNotExists = File.Delete(tempFile, true);
         Console.WriteLine("\n\t(Deleted tempfile == [{0}]: {1})", TextTrue, fileNotExists);
         Assert.IsTrue(fileNotExists);
      }

      #endregion // ReadAllLines

      #region ReadAllText

      [TestMethod]
      public void ReadAllText()
      {
         Console.WriteLine("File.ReadAllText()\n");

         // Create file and append text.
         string tempFile = Path.GetTempFileName();

         string[] createText = { "Hello", "And", "Welcome" };
         File.WriteAllLines(tempFile, createText);

         // Open the file to read from. 
         string textRead = File.ReadAllText(tempFile);
         Console.WriteLine(textRead);

         bool fileNotExists = File.Delete(tempFile, true);
         Console.WriteLine("\n\t(Deleted tempfile == [{0}]: {1})", TextTrue, fileNotExists);
         Assert.IsTrue(fileNotExists);
      }

      #endregion // ReadAllText

      #region SetAccessControl

      [TestMethod]
      public void SetAccessControl()
      {
         Console.WriteLine("File.SetAccessControl()");
         Console.WriteLine("FileInfo.SetAccessControl()");

         if (!IsAdmin())
            Assert.Fail();

         string path = SysDrive + @"\AlphaFile-" + Path.GetRandomFileName();
         string pathAlpha = Path.PrefixLongPath(path);

         Console.WriteLine("\n\tFile: [{0}]", path);

         try
         {
            using (File.Create(pathAlpha))
            {
            }

            // Initial read.
            Console.WriteLine("\n\tInitial read.");
            FileSecurity dsSystem = System.IO.File.GetAccessControl(path, AccessControlSections.Access);
            FileSecurity dsAlpha = File.GetAccessControl(pathAlpha, AccessControlSections.Access);
            AuthorizationRuleCollection accessRulesSystem = dsSystem.GetAccessRules(true, true, typeof (NTAccount));
            AuthorizationRuleCollection accessRulesAlpha = dsAlpha.GetAccessRules(true, true, typeof (NTAccount));
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "\t\tSystem.IO.File.GetAccessControl() rules found: [{0}]", accessRulesSystem.Count));
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "\t\t\t   File.GetAccessControl() rules found: [{0}]", accessRulesAlpha.Count));
            Assert.AreEqual(accessRulesSystem.Count, accessRulesAlpha.Count);

            // Sanity check.
            DumpAccessRules(1, dsSystem, dsAlpha);

            // Remove inherited properties.
            // Passing true for first parameter protects the new permission from inheritance, and second parameter removes the existing inherited permissions 
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture,
                                            "\n\tRemove inherited properties and persist it."));
            dsAlpha.SetAccessRuleProtection(true, false);
            File.SetAccessControl(pathAlpha, dsAlpha, AccessControlSections.Access);

            // Re-read, using instance methods.
            System.IO.FileInfo fiSystem = new System.IO.FileInfo(Path.LocalToUnc(path));
            FileInfo fiAlpha = new FileInfo(Path.PrefixLongPath(Path.LocalToUnc(path)));

            dsSystem = fiSystem.GetAccessControl(AccessControlSections.Access);
            dsAlpha = fiAlpha.GetAccessControl(AccessControlSections.Access);

            // Sanity check.
            DumpAccessRules(2, dsSystem, dsAlpha);

            // Restore inherited properties.
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture,
                                            "\n\tRestore inherited properties and persist it."));
            dsAlpha.SetAccessRuleProtection(false, true);
            File.SetAccessControl(pathAlpha, dsAlpha, AccessControlSections.Access);

            // Re-read.
            dsSystem = System.IO.File.GetAccessControl(path, AccessControlSections.Access);
            dsAlpha = File.GetAccessControl(pathAlpha, AccessControlSections.Access);

            // Sanity check.
            DumpAccessRules(3, dsSystem, dsAlpha);

            fiSystem.Delete();
            fiAlpha.Delete();
            bool fileNotExists = fiAlpha.Exists;
            Console.WriteLine("\n\t(Deleted tempfile == [{0}]: {1})", TextTrue, fileNotExists);
            Assert.IsTrue(fileNotExists);
         }
         catch (Exception ex)
         {
            Console.WriteLine("\nException: [{0}]\n", ex.Message.Replace(Environment.NewLine, string.Empty));
         }
      }

      #endregion // SetAccessControl

      #region WriteAllLines

      [TestMethod]
      public void WriteAllLines()
      {
         Console.WriteLine("File.WriteAllLines()");
         Console.WriteLine("\n Default AlphaFS Encoding: [{0}]", NativeMethods.DefaultFileEncoding.EncodingName);

         // Create file and append text.
         string tempFile = Path.GetTempFileName();

         string[] allLines = new[] { TenNumbers, TextHelloWorld, TextAppend, TextUnicode };

         // Create real UTF-8 file.
         File.WriteAllLines(tempFile, allLines, NativeMethods.DefaultFileEncoding);

         // Read filestream contents.
         using (StreamReader streamRead = File.OpenText(tempFile))
         {
            string line = streamRead.ReadToEnd();

            Console.WriteLine("\n Created: [{0}] filestream: [{1}]\n\n WriteAllLines content:\n{2}", streamRead.CurrentEncoding.EncodingName, tempFile, line);

            foreach (string line2 in allLines)
               Assert.IsTrue(line.Contains(line2));
         }

         bool fileNotExists = File.Delete(tempFile, true);
         Console.WriteLine("\n\t(Deleted tempfile == [{0}]: {1})", TextTrue, fileNotExists);
         Assert.IsTrue(fileNotExists);
      }

      #endregion // WriteAllLines

      #region WriteAllText

      [TestMethod]
      public void WriteAllText()
      {
         Console.WriteLine("File.WriteAllText()");
         Console.WriteLine("\n\tDefault AlphaFS Encoding: [{0}]", NativeMethods.DefaultFileEncoding.EncodingName);

         // Create file and append text.
         string tempFile = Path.GetTempFileName();

         string allLines = TextHelloWorld;

         // Create real UTF-8 file.
         File.WriteAllText(tempFile, allLines, NativeMethods.DefaultFileEncoding);

         // Read filestream contents.
         using (StreamReader streamRead = File.OpenText(tempFile))
         {
            string line = streamRead.ReadToEnd();

            Console.WriteLine("\n\tCreated: [{0}] filestream: [{1}]\n\n\tWriteAllText content:\n{2}", streamRead.CurrentEncoding.EncodingName, tempFile, line);

            Assert.IsTrue(line.Contains(allLines));
         }

         // (over)Write.
         File.WriteAllText(tempFile, "Append 1");
         File.WriteAllText(tempFile, allLines);
         File.WriteAllText(tempFile, "Append 2");
         File.WriteAllText(tempFile, allLines);

         // Read filestream contents.
         using (StreamReader streamRead = File.OpenText(tempFile))
         {
            string line = streamRead.ReadToEnd();

            Console.WriteLine("\tWriteAllText content:\n{0}", line);

            Assert.IsTrue(line.Contains(allLines));
            Assert.IsTrue(!line.Contains("Append 1"));
            Assert.IsTrue(!line.Contains("Append 2"));
         }

         bool fileNotExists = File.Delete(tempFile, true);
         Console.WriteLine("\n\t(Deleted tempfile == [{0}]: {1})", TextTrue, fileNotExists);
         Assert.IsTrue(fileNotExists);
      }

      #endregion // WriteAllText

      #endregion // .NET

      #region AlphaFS

      #region __Class_ByHandleFileInfo

      [TestMethod]
      public void __Class_ByHandleFileInfo()
      {
         Console.WriteLine("ByHandleFileInfo() Class");

         Dump__Class_ByHandleFileInfo(true);
         Dump__Class_ByHandleFileInfo(false);
      }

      #endregion // __Class_ByHandleFileInfo

      #region __Class_FileSystemEntryInfo

      [TestMethod]
      public void __Class_FileSystemEntryInfo()
      {
         Console.WriteLine("FileSystemEntryInfo() Class");

         string path = Path.GetTempPath("AlphaFS FileSystemEntryInfo().xyz");

         using (StreamWriter streamWrite = File.CreateText(path))
         {
            streamWrite.Write(TextHelloWorld);
         }

         Console.WriteLine("\n\tTemp File: [{0}]", path);

         FileSystemEntryInfo fsei = File.GetFileSystemEntryInfo(path);
         bool foundFsei = fsei != null;
         Assert.IsTrue(Dump(fsei, -17));

         Assert.IsTrue(foundFsei);
         Assert.IsTrue(File.Delete(path));
      }

      #endregion // __Class_FileSystemEntryInfo

      #region Compress

      [TestMethod]
      public void Compress()
      {
         Console.WriteLine("File.Compress()");

         // Create file and append text.
         string tempFile8 = Path.GetTempFileName();

         // Check that the drive, on which the tempfile will be created, is NOT NTFS-compressed.
         DirectoryInfo di = new DirectoryInfo(tempFile8);

         if (di.Attributes != (FileAttributes)(-1) && NativeMethods.HasFileAttribute(di.Attributes, FileAttributes.Compressed))
            Assert.IsTrue(false, "Volume is a compressed volume.");

         // Append text as UTF-8, default.
         for (int i = 0; i < 10000; i++)
         {
            File.AppendAllText(tempFile8, TextHelloWorld);
         }

         string utf8 = NativeMethods.DefaultFileEncoding.BodyName.ToUpperInvariant();
         FileAttributes attrs = File.GetAttributes(tempFile8);
         FileInfo file = new FileInfo(tempFile8);
         long sizeUncompressed = file.Length;
         long sizeCompressed = file.LengthCompressed;
         long sizeBytes = File.GetSize(tempFile8);
         Console.WriteLine("\n\tCreated {0} file: [{1}]", utf8, tempFile8);
         Console.WriteLine("\tFile.Length == [{0}]", NativeMethods.UnitSizeToText(sizeUncompressed));
         Console.WriteLine("\tFile.LengthCompressed() == [{0}]", NativeMethods.UnitSizeToText(sizeCompressed));
         Console.WriteLine("\tFile.GetSize() == [{0}]", NativeMethods.UnitSizeToText(sizeBytes));
         Console.WriteLine("\tFile.GetAttributes() == [{0}]", attrs);
         Assert.IsTrue((attrs & FileAttributes.Compressed) != FileAttributes.Compressed);
         Assert.IsTrue(sizeUncompressed == sizeCompressed);
         Assert.IsTrue(sizeBytes == sizeUncompressed);

         file = new FileInfo(tempFile8);
         bool compressOk = file.Compress();
         Assert.IsTrue(compressOk);
         attrs = File.GetAttributes(tempFile8);
         sizeUncompressed = file.Length;
         sizeCompressed = File.GetCompressedSize(tempFile8);
         sizeBytes = File.GetSize(tempFile8);
         Console.WriteLine("\n\tFile.Compress() successful == [{0}]: {1}", TextTrue, compressOk);
         Console.WriteLine("\tFile.Length == [{0}]", NativeMethods.UnitSizeToText(sizeUncompressed));
         Console.WriteLine("\tFile.GetCompressedSize() == [{0}]", NativeMethods.UnitSizeToText(sizeCompressed));
         Console.WriteLine("\tFile.GetSize() == [{0}]", NativeMethods.UnitSizeToText(sizeBytes));
         Console.WriteLine("\tFile.GetAttributes() == [{0}]", attrs);
         Assert.IsTrue(NativeMethods.HasFileAttribute(attrs, FileAttributes.Compressed));
         Assert.IsTrue(sizeUncompressed != sizeCompressed);
         Assert.IsTrue(sizeBytes == sizeUncompressed);

         bool decompressOk = File.Decompress(tempFile8);
         attrs = File.GetAttributes(tempFile8);
         file = new FileInfo(tempFile8);
         sizeUncompressed = file.Length;
         sizeCompressed = file.LengthCompressed;
         sizeBytes = File.GetSize(tempFile8);
         Console.WriteLine("\n\tFile.Decompress() successful == [{0}]: {1}", TextTrue, decompressOk);
         Console.WriteLine("\tFile Size == [{0}]", NativeMethods.UnitSizeToText(sizeUncompressed));
         Console.WriteLine("\tFile.LengthCompressed() == [{0}]", NativeMethods.UnitSizeToText(sizeCompressed));
         Console.WriteLine("\tFile.GetSize() == [{0}]", NativeMethods.UnitSizeToText(sizeBytes));
         Console.WriteLine("\tFile.GetAttributes() == [{0}]", attrs);
         Assert.IsTrue((attrs & FileAttributes.Compressed) != FileAttributes.Compressed);
         Assert.IsTrue(sizeUncompressed == sizeCompressed);
         Assert.IsTrue(sizeBytes == sizeUncompressed);

         bool fileNotExists = file.Delete(true);
         Console.WriteLine("\n\t(Deleted tempfile == [{0}]: {1})", TextTrue, fileNotExists);
         Assert.IsTrue(fileNotExists);
      }

      #endregion // Compress

      #region Decompress

      [TestMethod]
      public void Decompress()
      {
         Console.WriteLine("File.Decompress()");

         Compress();
      }

      #endregion // Compress/Decompress

      #region EnumerateStreams

      [TestMethod]
      public void EnumerateStreams()
      {
         Console.WriteLine("File.EnumerateStreams()");
         Console.WriteLine("FileInfo.EnumerateStreams()");

         DumpEnumerateStreams(true);
         DumpEnumerateStreams(false);
      }

      #endregion // EnumerateStreams
      
      #region GetCompressedSize

      [TestMethod]
      public void GetCompressedSize()
      {
         Console.WriteLine("File.GetCompressedSize()");
         Console.WriteLine("FileInfo.GetCompressedSize()");

         Compress();
      }

      #endregion // GetCompressedSize

      #region GetEncryptionStatus

      [TestMethod]
      public void GetEncryptionStatus()
      {
         Console.WriteLine("File.GetEncryptionStatus()");

         Decrypt();
      }

      #endregion // GetEncryptionStatus

      #region GetFileInformationByHandle

      [TestMethod]
      public void GetFileInformationByHandle()
      {
         Console.WriteLine("File.GetFileInformationByHandle()");

         __Class_ByHandleFileInfo();
      }

      #endregion // GetFileInformationByHandle

      #region GetFileSystemEntryInfo

      [TestMethod]
      public void GetFileSystemEntryInfo()
      {
         Console.WriteLine("File.GetFileSystemEntryInfo()");

         __Class_FileSystemEntryInfo();
      }

      #endregion // GetFileSystemEntryInfo

      #region GetFileType

      [TestMethod]
      public void GetFileType()
      {
         Console.WriteLine("File.GetFileType()");
         Console.WriteLine("Also see Shell32.GetFileType(string)");

         #region Stream

         string path = Path.GetTempPath("AlphaFS File.GetFileType().xyz");
         string collectStrings = string.Empty;
         int cnt = 0;
         int ten = TenNumbers.Length;
         string fileType;

         using (FileStream stream = File.Create(path))
         {
            // According to NotePad++, creates a file type: "ANSI", which is reported as: "Unicode (UTF-8)".
            stream.Write(StringToByteArray(TenNumbers), 0, ten);

            fileType = File.GetFileType(path);
            collectStrings += fileType;

            Console.WriteLine("\n\tTemp stream: [{0}]", path);
            Console.WriteLine("\tFile.GetFileType() == [{0}]", fileType);
            cnt++;
         }
         Assert.IsTrue(cnt > 0 && !string.IsNullOrEmpty(collectStrings));

         File.Delete(path);
         bool fileNotExists = !File.Exists(path);
         Console.WriteLine("\n\t(Deleted tempfile == [{0}]: {1})", TextTrue, fileNotExists);
         Assert.IsTrue(fileNotExists);

         #endregion // Stream

         #region File

         Console.WriteLine("\n\tDirectory: [{0}]\n", SysRoot);

         collectStrings = string.Empty;
         cnt = 0;
         StopWatcher(true);
         foreach (string file in Directory.EnumerateFiles(SysRoot))
         {
            fileType = File.GetFileType(file);
            collectStrings += file;

            Console.WriteLine("\t\t#{0:000}\tFile.GetFileType() == {1, -20}\t{2, -40}", ++cnt, fileType, file);
         }
         Console.WriteLine("\n{0}\n", Reporter());
         Assert.IsTrue(cnt > 0 && !string.IsNullOrEmpty(collectStrings));

         string uncPath = Path.LocalToUnc(SysRoot);
         if (Directory.Exists(uncPath))
         {
            Console.WriteLine("\tDirectory: [{0}]\n", uncPath);

            collectStrings = string.Empty;
            cnt = 0;
            StopWatcher(true);
            foreach (string uncFile in Directory.EnumerateFiles(uncPath))
            {
               fileType = File.GetFileType(uncFile);
               collectStrings += fileType;

               Console.WriteLine("\t\t#{0:000}\tFile.GetFileType() == {1, -20}\t{2, -40}", ++cnt, fileType, uncFile);
            }
            Console.WriteLine("\n{0}\n", Reporter());
            Assert.IsTrue(cnt > 0 && !string.IsNullOrEmpty(collectStrings));
         }
         else
         {
            Console.Write("\n\tShare inaccessible: {0}\n", uncPath);
         }

         #endregion // File
      }

      #endregion // GetFileType

      #region GetLinkTargetInfo

      [TestMethod]
      public void GetLinkTargetInfo()
      {
         Console.WriteLine("File.GetLinkTargetInfo()");
         Console.WriteLine("\n\tPlease see Volume.SetVolumeMountPoint() UnitTest.");
      }

      #endregion // GetLinkTargetInfo

      #region GetSize

      [TestMethod]
      public void GetSize()
      {
         Console.WriteLine("File.GetSize()");

         Compress();
      }

      #endregion // GetSize

      #region GetStreamsSize

      [TestMethod]
      public void GetStreamsSize()
      {
         Console.WriteLine("File.GetStreamsSize()\n");

         string path = SysRoot;
         int cnt = 0;

         foreach (string file in Directory.EnumerateFiles(path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly))
         {
            try
            {
               Console.WriteLine("\t#{0:000} File: [{1}]\t\t = [{2}]", ++cnt, file, File.GetStreamsSize(file));
            }
            catch (Exception ex)
            {
               Console.WriteLine("\nException: [{0}]\n", ex.Message.Replace(Environment.NewLine, string.Empty));
            }
         }

      }

      #endregion // GetStreamsSize

      #endregion // AlphaFS
   }
}