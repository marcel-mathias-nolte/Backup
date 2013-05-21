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
using System.Linq;
using System.Text;
using Alphaleonis.Win32.Filesystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace AlphaFS.UnitTest
{
   /// <summary>This is a test class for Shell32 and is intended to contain all Shell32 Unit Tests.</summary>
   [TestClass]
   public class Shell32Test
   {
      #region Shell32Test Helpers

      private static readonly string StartupFolder = AppDomain.CurrentDomain.BaseDirectory;
      private static readonly string SysDrive = Environment.GetEnvironmentVariable("SystemDrive");
      private static readonly string SysRoot = Environment.GetEnvironmentVariable("SystemRoot");
      private static readonly string SysRoot32 = Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "System32");
      private static readonly string AppData = Environment.GetEnvironmentVariable("AppData");
      private static readonly string ProgramFiles = Environment.GetEnvironmentVariable("ProgramFiles");

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
         string template = "\t#{0:000}\t{1, " + width + "} ==\t[{2}]";

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

      private static byte[] StringToByteArray(string str, params Encoding[] encoding)
      {
         Encoding encode = encoding != null && encoding.Any() ? encoding[0] : new UTF8Encoding(true, true);
         return encode.GetBytes(str);
      }

      private static void DumpFindExecutable(string path)
      {
         Console.WriteLine("\n\tPath: [{0}]\n", path);

         if (Directory.Exists(path))
         {
            string collectStrings = string.Empty;
            int cnt = 0;
            StopWatcher(true);
            foreach (string file in Directory.EnumerateFiles(path))
            {
               string fileName = Path.GetFileName(file);
               string filePath = Shell32.FindExecutable(fileName);
               collectStrings += filePath;

               Console.WriteLine("\t#{0:000}\t[{1}]\t\t\"Open With\"\t[{2}]", ++cnt, fileName, filePath);
            }
            Console.WriteLine("\n{0}", Reporter());
            Assert.IsTrue(cnt > 0, "No files enumerated.");
            Assert.IsTrue(!string.IsNullOrEmpty(collectStrings), "Shell32.FindExecutable() failed.");
         }
         else
            Console.Write("\n\tPath inaccessible: {0}\n", path);
      }

      private static void DumpSHGetFileInfo(string path)
      {
         Console.WriteLine("\n\tPath: [{0}]\n", path);

         if (Directory.Exists(path))
         {
            string collectStrings = string.Empty;
            int cnt = 0;
            StopWatcher(true);
            foreach (string file in Directory.EnumerateFiles(path))
            {
               Shell32.FileInfo shFileType = Shell32.SHGetFileInfo(file);
               collectStrings += shFileType.TypeName;

               Console.WriteLine(string.Format(CultureInfo.CurrentCulture,
                                               "\t#{0:000}\t[{1, -20}]\tTypeName: [{2, -20}]\t\tDisplayName: [{3}]\t\t\tAttributes: [{4}]",
                                               ++cnt, file, shFileType.TypeName, shFileType.DisplayName, shFileType.Attributes));
            }
            Console.WriteLine("\n{0}", Reporter());
            Assert.IsTrue(cnt > 0, "No files enumerated.");
            Assert.IsTrue(!string.IsNullOrEmpty(collectStrings));
         }
         else
            Console.Write("\n\tPath inaccessible: {0}\n", path);
      }

      private static void DumpPathFileExists(string path, bool doesExist)
      {
         Console.WriteLine("\n\tPath: [{0}]\n", path);

         bool fileExists = Shell32.PathFileExists(path);
         Console.WriteLine("\t\tShell32.PathFileExists() == [{0}]: {1}\t\t[{2}]", doesExist ? TextTrue : TextFalse, doesExist == fileExists, path);
         Console.WriteLine("\t\tFile.Exists()            == [{0}]: {1}\t\t[{2}]", doesExist ? TextTrue : TextFalse, doesExist == File.Exists(path), path);
         Console.WriteLine("\t\tDirectory.Exists()       == [{0}]: {1}\t\t[{2}]", doesExist ? TextTrue : TextFalse, doesExist == Directory.Exists(path), path);

         if (doesExist)
            Assert.IsTrue(fileExists);

         if (!doesExist)
            Assert.IsTrue(!fileExists);
      }

      #endregion // Shell32Test Helpers

      [TestMethod]
      public void FindExecutable()
      {
         Console.WriteLine("Shell32.FindExecutable()");

         string path = SysRoot;
         DumpFindExecutable(path);
         DumpFindExecutable(Path.LocalToUnc(path));
      }

      [TestMethod]
      public void GetAssociation()
      {
         Console.WriteLine("Shell32.GetAssociation()");

         string path = SysRoot;

         #region Verify / Executable

         Console.WriteLine("\n\tDirectory: [{0}]", path);
         Shell32.AssociationAttributes associationAttributes = Shell32.AssociationAttributes.Verify;
         Shell32.AssociationString associationString = Shell32.AssociationString.Executable;
         Console.WriteLine("\tShell32.GetAssociation(file, {0}, {1})\n", associationAttributes, associationString);

         string shFileType;
         string collectStrings = string.Empty;
         int cnt = 0;
         StopWatcher(true);
         foreach (string file in Directory.EnumerateFiles(path))
         {
            shFileType = Shell32.GetAssociation(file);

            collectStrings += shFileType;

            Console.WriteLine("\t#{0:000}\t[{1}]\t\t[{2, -30}]", ++cnt, file, shFileType);
         }
         Console.WriteLine("\n{0}", Reporter());
         Assert.IsTrue(cnt > 0, "No entries enumerated.");
         Assert.IsTrue(!string.IsNullOrEmpty(collectStrings), "shFileType is empty.");

         #endregion // Verify / Executable

         #region Verify / FriendlyAppName

         Console.WriteLine("\n\tDirectory: [{0}]", path);
         associationAttributes = Shell32.AssociationAttributes.Verify;
         associationString = Shell32.AssociationString.FriendlyAppName;
         Console.WriteLine("\tShell32.GetAssociation(file, {0}, {1})\n", associationAttributes, associationString);


         collectStrings = string.Empty;
         cnt = 0;
         StopWatcher(true);
         foreach (string file in Directory.EnumerateFiles(path))
         {
            shFileType = Shell32.GetAssociation(file, Shell32.AssociationAttributes.Verify, Shell32.AssociationString.FriendlyAppName);
            
            if (!string.IsNullOrEmpty(shFileType))
               collectStrings += shFileType;

            Console.WriteLine("\t#{0:000}\t[{1}]\t\t[{2, -30}]", ++cnt, file, shFileType);
         }
         Console.WriteLine("\n{0}", Reporter());
         Assert.IsTrue(cnt > 0, "No entries enumerated.");
         Assert.IsTrue(!string.IsNullOrEmpty(collectStrings), "shFileType is empty.");

         #endregion // Verify / FriendlyAppName

         #region OpenByExeName / FriendlyAppName

         Console.WriteLine("\n\tDirectory: [{0}]", path);
         associationAttributes = Shell32.AssociationAttributes.OpenByExeName;
         associationString = Shell32.AssociationString.FriendlyAppName;
         Console.WriteLine("\tShell32.GetAssociation(file, {0}, {1})\n", associationAttributes, associationString);

         collectStrings = string.Empty;
         cnt = 0;
         StopWatcher(true);
         foreach (string file in Directory.EnumerateFiles(path))
         {
            shFileType = Shell32.GetAssociation(file, Shell32.AssociationAttributes.OpenByExeName, Shell32.AssociationString.FriendlyAppName);

            if (!string.IsNullOrEmpty(shFileType))
               collectStrings += shFileType;

            Console.WriteLine("\t#{0:000}\t[{1}]\t\t[{2, -30}]", ++cnt, file, shFileType);
         }
         Console.WriteLine("\n{0}", Reporter());
         Assert.IsTrue(cnt > 0, "No entries enumerated.");
         Assert.IsTrue(!string.IsNullOrEmpty(collectStrings), "shFileType is empty.");

         #endregion // OpenByExeName / FriendlyAppName
      }

      [TestMethod]
      public void GetFileType()
      {
         Console.WriteLine("Shell32.GetFileType()");
         Console.WriteLine("Also see File.GetFileType(handle)");

         string path = Path.GetTempPath("AlphaFS Shell32.GetFileType().xyz");
         string collectStrings = string.Empty;
         int cnt = 0;
         int ten = TenNumbers.Length;
         string fileType;

         Console.WriteLine("\n\tDirectory: [{0}]\n", SysRoot);

         collectStrings = string.Empty;
         cnt = 0;
         StopWatcher(true);
         foreach (string file in Directory.EnumerateFiles(SysRoot))
         {
            fileType = Shell32.GetFileType(file);
            collectStrings += file;

            Console.WriteLine("\t#{0:000}\t[{1}]\t\tShell32.GetFileType()\t[{2, -30}]", ++cnt, file, fileType);
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
               fileType = Shell32.GetFileType(uncFile);
               collectStrings += fileType;

               Console.WriteLine("\t#{0:000}\t[{1}]\t\tShell32.GetFileType()\t[{2, -30}]", ++cnt, uncFile, fileType);
            }
            Console.WriteLine("\n{0}\n", Reporter());
            Assert.IsTrue(cnt > 0);
            Assert.IsTrue(!string.IsNullOrEmpty(collectStrings), "Shell32.GetFileType() failed.");
         }
         else
         {
            Console.Write("\n\tShare inaccessible: {0}\n", uncPath);
         }
      }

      [TestMethod]
      public void PathCreateFromUrl()
      {
         Console.WriteLine("Shell32.PathCreateFromUrl() - {0}", SpecificX3);

         string urlPath = Shell32.UrlCreateFromPath(AppData);
         string filePath = Shell32.PathCreateFromUrl(urlPath);

         Console.WriteLine("\n\tDirectory                  : [{0}]", AppData);
         Console.WriteLine("\n\tShell32.UrlCreateFromPath(): [{0}]", urlPath);
         Console.WriteLine("\n\tShell32.PathCreateFromUrl() == [{0}]\n", filePath);

         bool startsWith = urlPath.StartsWith("file:///");
         bool equalsAppData = filePath.Equals(AppData);

         Console.WriteLine("\n\turlPath.StartsWith(\"file:///\") == [{0}]: {1}", TextTrue, startsWith);
         Console.WriteLine("\n\tfilePath.Equals(AppData)       == [{0}]: {1}\n", TextTrue, equalsAppData);
         Assert.IsTrue(startsWith);
         Assert.IsTrue(equalsAppData);
      }

      [TestMethod]
      public void PathCreateFromUrlAlloc()
      {
         Console.WriteLine("Shell32.PathCreateFromUrlAlloc()");

         string urlPath = Shell32.UrlCreateFromPath(AppData);
         string filePath = Shell32.PathCreateFromUrlAlloc(urlPath);

         Console.WriteLine("\n\tDirectory                  : [{0}]", AppData);
         Console.WriteLine("\n\tShell32.UrlCreateFromPath(): [{0}]", urlPath);
         Console.WriteLine("\n\tShell32.PathCreateFromUrlAlloc() == [{0}]\n", filePath);

         bool startsWith = urlPath.StartsWith("file:///");
         bool equalsAppData = filePath.Equals(AppData);

         Console.WriteLine("\n\turlPath.StartsWith(\"file:///\") == [{0}]: {1}", TextTrue, startsWith);
         Console.WriteLine("\n\tfilePath.Equals(AppData)       == [{0}]: {1}\n", TextTrue, equalsAppData);
         Assert.IsTrue(startsWith);
         Assert.IsTrue(equalsAppData);
      }

      [TestMethod]
      public void PathFileExists()
      {
         Console.WriteLine("Shell32.PathFileExists()");

         string path = SysRoot;
         DumpPathFileExists(path, true);
         DumpPathFileExists(Path.LocalToUnc(path), true);
         DumpPathFileExists("BlaBlaBla", false);
         DumpPathFileExists(Path.Combine(SysRoot, "BlaBlaBla"), false);

         int cnt = 0;
         StopWatcher(true);
         foreach (string file in Directory.EnumerateFiles(SysRoot))
         {
            bool fileExists = Shell32.PathFileExists(file);

            Console.WriteLine("\t#{0:000}\tShell32.PathFileExists() == [{1}]: {2}\t\t[{3}]", ++cnt, TextTrue, fileExists, file);
            Assert.IsTrue(fileExists);
         }
         Console.WriteLine("\n{0}", Reporter());
      }

      [TestMethod]
      public void SHGetFileInfo()
      {
         Console.WriteLine("Shell32.SHGetFileInfo()");

         string path = SysRoot;
         DumpSHGetFileInfo(path);
         DumpSHGetFileInfo(Path.LocalToUnc(path));
      }

      [TestMethod]
      public void UrlCreateFromPath()
      {
         Console.WriteLine("Shell32.UrlCreateFromPath()");

         PathCreateFromUrl();
         PathCreateFromUrlAlloc();
      }

      [TestMethod]
      public void UrlIs()
      {
         Console.WriteLine("Shell32.UrlIs()");

         string urlPath = Shell32.UrlCreateFromPath(AppData);
         string filePath = Shell32.PathCreateFromUrlAlloc(urlPath);

         bool isFileUrl1 = Shell32.UrlIsFileUrl(urlPath);
         bool isFileUrl2 = Shell32.UrlIsFileUrl(filePath);
         bool isNoHistory = Shell32.UrlIs(filePath, Shell32.UrlTypes.IsNoHistory);
         bool isOpaque = Shell32.UrlIs(filePath, Shell32.UrlTypes.IsOpaque);

         Console.WriteLine("\n\tDirectory: [{0}]", AppData);
         Console.WriteLine("\n\tShell32.UrlCreateFromPath()      == IsFileUrl == [{0}] : {1}\t\t[{2}]", TextTrue, isFileUrl1, urlPath);
         Console.WriteLine("\n\tShell32.PathCreateFromUrlAlloc() == IsFileUrl == [{0}]: {1}\t\t[{2}]", TextFalse, isFileUrl2, filePath);

         Console.WriteLine("\n\tShell32.UrlIsFileUrl()   == [{0}]: {1}\t\t[{2}]", TextTrue, isFileUrl1, urlPath);
         Console.WriteLine("\n\tShell32.UrlIsNoHistory() == [{0}]: {1}\t\t[{2}]", TextTrue, isNoHistory, urlPath);
         Console.WriteLine("\n\tShell32.UrlIsOpaque()    == [{0}]: {1}\t\t[{2}]", TextTrue, isOpaque, urlPath);
         
         Assert.IsTrue(isFileUrl1);
         Assert.IsTrue(isFileUrl2 == false);
      }

      [TestMethod]
      public void UrlIsFileUrl()
      {
         Console.WriteLine("Shell32.UrlIsFileUrl()");

         UrlIs();
      }

      [TestMethod]
      public void UrlIsNoHistory()
      {
         Console.WriteLine("Shell32.UrlIsNoHistory()");

         UrlIs();
      }

      [TestMethod]
      public void UrlIsOpaque()
      {
         Console.WriteLine("Shell32.UrlIsOpaque()");

         UrlIs();
      }
   }
}