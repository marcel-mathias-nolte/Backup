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
using Alphaleonis.Win32.Filesystem;
using Alphaleonis.Win32.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using File = Alphaleonis.Win32.Filesystem.File;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using FileSystemInfo = Alphaleonis.Win32.Filesystem.FileSystemInfo;
using NativeMethods = Alphaleonis.Win32.Filesystem.NativeMethods;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace AlphaFS.UnitTest
{
   /// <summary>This is a test class for Directory and is intended to contain all Directory Unit Tests.</summary>
   [TestClass]
   public class DirectoryTest
   {
      #region DirectoryTest Helpers

      private static readonly string StartupFolder = AppDomain.CurrentDomain.BaseDirectory;
      private static readonly string SysDrive = Environment.GetEnvironmentVariable("SystemDrive");
      private static readonly string SysRoot = Environment.GetEnvironmentVariable("SystemRoot");
      private static readonly string SysRoot32 = Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "System32");

      private const string TextTrue = "IsTrue";
      private const string TextFalse = "IsFalse";
      private readonly string TextHelloWorld = "Hëllõ Wørld!-" + Path.GetRandomFileName();
      private readonly string TextGoodByeWorld = "GóödByé Wôrld!-" + Path.GetRandomFileName();

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

      #region Dumpers

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
      private static void DumpAccessRules(int cntCheck, DirectorySecurity dsSystem, DirectorySecurity dsAlpha)
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
      private void CreateFoldersAndFiles(string rootPath, int max, bool recurse)
      {
         for (int i = 0; i < max; i++)
         {
            string file = Path.Combine(rootPath, Path.GetRandomFileName());
            string dir = file + "-" + i + "-dir";

            Directory.CreateDirectory(dir);

            File.WriteAllText(file, TextHelloWorld);
            File.WriteAllText(Path.Combine(dir, Path.GetFileName(file)), TextGoodByeWorld);
         }

         if (recurse)
         {
            foreach (string dir in Directory.EnumerateDirectories(rootPath))
               CreateFoldersAndFiles(dir, max, false);
         }
      }

      private static void DumpCreateDirectory(bool isLocal)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");
         string path = Path.GetTempPath(Path.GetRandomFileName());
         if (!isLocal)
            path = Path.LocalToUnc(path);

         string report = "";

         DirectoryInfo dirInfo = new DirectoryInfo(path);
         System.IO.DirectoryInfo dirInfoSysIO = new System.IO.DirectoryInfo(path);

         // Should be false.
         if (!dirInfo.Exists)
         {
            StopWatcher(true);
            dirInfo.Create();
            report = Reporter();
         }

         StopWatcher(true);
         bool exist = Directory.Exists(path);
         report += "\n" + Reporter();
         Console.WriteLine("\n\tInput Path: [{0}]", path);
         Console.WriteLine("\t\tExist == [{0}]: {1}{2}", TextTrue, exist, Reporter());
         Assert.IsTrue(exist, "Folder doesn't exists: [{0}]", path);

         StopWatcher(true);
         string pathSub = dirInfo.CreateSubdirectory(Path.Combine(path, "A Sub Directory")).FullName;
         report = Reporter();

         Assert.AreEqual(dirInfoSysIO.Exists, dirInfo.Exists, "DirectoryInfo.Exists doesn't match System.IO");

         StopWatcher(true);
         dirInfo.Refresh();
         // After the refresh, should be true.
         exist = dirInfo.Exists;
         report += "\n" + Reporter();
         Assert.IsTrue(exist, "DirectoryInfo.Exists should report true.");

         Console.WriteLine("\n\tSubDirectory: [{0}]", pathSub);
         Console.WriteLine("\t\tExist == [{0}]: {1}{2}", TextTrue, exist, Reporter());
         Assert.IsTrue(pathSub != null);
         Assert.IsTrue(exist, "Folder doesn't exists: [{0}]", pathSub);

         string root = Path.Combine(path, "Another Sub Directory");

         // MAX_PATH hit the road.
         int level = new Random().Next(100, 300);
         for (int i = 0; i < level; i++)
            root = Path.Combine(root, "-" + (i + 1) + "-subdir");


         StopWatcher(true);
         Directory.CreateDirectory(root);
         report = Reporter();
         StopWatcher(true);
         exist = Directory.Exists(root);
         report += "\n" + Reporter();
         Console.WriteLine("\n\n\tSubdirectory depth: [{0}], path length: [{1}] characters.\n\n[{2}]", level, root.Length, root);
         Console.WriteLine("\n\t\tExist == [{0}]: {1}{2}", TextTrue, exist, Reporter());
         Assert.IsTrue(root != null);
         Assert.IsTrue(exist, "Folder doesn't exists: [{0}]", root);


         StopWatcher(true);
         Directory.Delete(path, true);
         report = Reporter();
         StopWatcher(true);
         bool deleteOk = !Directory.Exists(path);
         report += "\n" + Reporter();
         Console.WriteLine("\n\tDirectory.Delete(true) recursive == [{0}]: {1}\n{2}\n\n", TextTrue, deleteOk, report);
         Assert.IsTrue(deleteOk, "Folder was not deleted: [{0}]", path);
      }

      private static void DumpGetAttributes(bool isLocal)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");
         string path = isLocal ? SysRoot : Path.LocalToUnc(SysRoot);

         int cnt = 0;
         StopWatcher(true);
         foreach (string file in Directory.EnumerateDirectories(path))
         {
            FileAttributes attrs = Directory.GetAttributes(file);
            FileAttributes attrsSysIO = System.IO.File.GetAttributes(file);   // System.IO.Directory.GetAttributes() doesn't exist in .NET.
            Console.WriteLine("\t\t#{0:000}\tDirectory:\t[{1}]\t\tAttributes: [{2}]\t\tSysIO: [{3}]", ++cnt, file, attrs, attrsSysIO);
            Assert.IsTrue(cnt > 0);
            Assert.AreEqual(attrs, attrsSysIO);
         }
         Console.WriteLine("\n\t{0}\n", Reporter());
      }

      private static void DumpGetDirectoryRoot()
      {
         int pathCnt = 0;
         bool allOk = true;
         int errorCnt = 0;
         
         string orgPathActual = Directory.GetCurrentDirectory();
         string orgPathExpected = System.IO.Directory.GetCurrentDirectory();
         Assert.AreEqual(orgPathExpected, orgPathActual, "Directory.GetCurrentDirectory() doesn't match System.IO");

         Directory.SetCurrentDirectory(Path.GetTempPath());

         foreach (string input in InputPaths)
         {
            string path = input;

            try
            {
               string actual = Directory.GetDirectoryRoot(path);
               string expected = System.IO.Directory.GetDirectoryRoot(path);
               Console.WriteLine("\n\t#{0:000}\tInput Path: [{1}]\t\tGetDirectoryRoot(): [{2}]\t\tSystem.IO: [{3}]", ++pathCnt, input, actual, expected);
               Assert.AreEqual(expected, actual);

               Directory.SetCurrentDirectory(path);
               Assert.AreEqual(Directory.GetCurrentDirectory(), System.IO.Directory.GetCurrentDirectory(), "Directory.GetCurrentDirectory() doesn't match System.IO");

            }
            catch (Exception ex)
            {
               Console.WriteLine("\n\t\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));

               // Exception to the Exception; System.IO.GetDirectoryRoot() handles this but throws Exception.
               if (path != null && !path.StartsWith(@"\\?\GlobalRoot", StringComparison.OrdinalIgnoreCase) &&
                  !ex.Message.Equals("Illegal characters in path.", StringComparison.OrdinalIgnoreCase))
               {
                  allOk = false;
                  errorCnt++;
               }
            }
         }

         Directory.SetCurrentDirectory(orgPathActual);
         Assert.AreEqual(Directory.GetCurrentDirectory(), System.IO.Directory.GetCurrentDirectory(), "Directory.GetCurrentDirectory() doesn't match System.IO");

         Assert.AreEqual(true, allOk, "Encountered: [{0}] paths where AlphaFS != System.IO", errorCnt);
      }

      private static void DumpGetParent()
      {
         int pathCnt = 0;
         bool allOk = true;
         int errorCnt = 0;

         foreach (string input in InputPaths)
         {
            string path = input;

            try
            {
               string actual = Directory.GetParent(path).ToString();
               string expected = System.IO.Directory.GetParent(path).ToString();
               Console.WriteLine("\n\t#{0:000}\tInput Path: [{1}]\t\tGetParent(): [{2}]\t\tSystem.IO: [{3}]", ++pathCnt, path, actual, expected);
               Assert.AreEqual(expected, actual);
            }
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
         }

         Assert.AreEqual(true, allOk, "Encountered: [{0}] paths where AlphaFS != System.IO", errorCnt);
      }

      private static void DumpEnumerateDirectories(bool isLocal)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");
         string path = isLocal ? SysRoot32 : Path.LocalToUnc(SysRoot32);

         DirectoryInfo dirInfo = new DirectoryInfo(path);
         DateTime currentDate = DateTime.Now;
         DateTime dateStart = new DateTime(currentDate.Year - 2, currentDate.Month, currentDate.Day);

         int cntAlphaFS = 0;
         string searchPattern;
         SearchOption searchOption = SearchOption.TopDirectoryOnly;

         Console.WriteLine("\n\t[AlphaFS] (Date) Enumerate files > [{0}], using \"SearchOption.{1}\", from directory: [{2}]\n", dateStart, searchOption, path);

         StopWatcher(true);
         foreach (DirectoryInfo di in dirInfo.GetDirectories().Where(dir => dir.CreationTime > dateStart))
         {
            string dir = di.FullName;
            FileAttributes attr = di.Attributes;

            Console.WriteLine("\t\t#{0:000}\t[{1}]\t[{2}]\t\t[{3}]", ++cntAlphaFS, attr, dir, di.CreationTime);
            Assert.IsTrue(NativeMethods.HasFileAttribute(attr, FileAttributes.Directory), "Not a Directory: [{0}]", dir);
         }
         Console.WriteLine("\n\t{0}\n", Reporter());
         Assert.IsTrue(cntAlphaFS > 0);


         searchPattern = "b*";
         searchOption = SearchOption.AllDirectories;

         #region Exception

         bool gotException = false;
         StopWatcher(true);
         try
         {
            foreach (DirectoryInfo di in dirInfo.EnumerateDirectories(searchPattern, searchOption))
            {
            }
         }
         catch (Exception ex)
         {
            gotException = true;
            Console.WriteLine("\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));
         }
         Console.WriteLine("\n\tCaught Exception: [{0}]\t{1}", gotException, Reporter());
         Assert.IsTrue(gotException);

         #endregion // Exception

         Console.WriteLine("\n\t[AlphaFS] (Pattern) Enumerate directories that start with: [{0}], using \"SearchOption.{1}\" from directory: [{2}]\n", searchPattern, searchOption, path);

         cntAlphaFS = 0;
         StopWatcher(true);
         foreach (DirectoryInfo di in dirInfo.EnumerateDirectories(searchPattern, searchOption, true))
         {
            string dir = di.FullName;
            FileAttributes attr = di.Attributes;

            Console.WriteLine("\t\t#{0:000}\t[{1}]\t[{2}]", ++cntAlphaFS, attr, dir);

            Assert.IsTrue(NativeMethods.HasFileAttribute(attr, FileAttributes.Directory), "Not a Directory: [{0}]", dir);
            Assert.IsTrue(di.Name.StartsWith(searchPattern[0].ToString(CultureInfo.CurrentCulture), StringComparison.OrdinalIgnoreCase));
         }
         Console.WriteLine("\n\t{0}\n", Reporter());
         Assert.IsTrue(cntAlphaFS > 0);
      }

      private static void DumpEnumerateFiles(bool isLocal)
      {
         //try
         //{
            Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");
            string path = isLocal ? SysRoot32 : Path.LocalToUnc(SysRoot32);
            //string path = isLocal ? @"d:\" : Path.LocalToUnc(@"d:\");

            DirectoryInfo dirInfo = new DirectoryInfo(path);

            #region Date

            DateTime currentDate = DateTime.Now;
            DateTime dateStart = new DateTime(currentDate.Year - 2, currentDate.Month, currentDate.Day);

            int cntAlphaFS = 0;
            string searchPattern;
            SearchOption searchOption = SearchOption.TopDirectoryOnly;

            Console.WriteLine("\n\t[AlphaFS] (Date) Enumerate files > [{0}], using \"SearchOption.{1}\", from directory: [{2}]\n", dateStart, searchOption, path);

            StopWatcher(true);
            foreach (FileInfo fi in dirInfo.EnumerateFiles(true).Where(file => file.CreationTime > dateStart))
            {
               string file = fi.FullName;
               FileAttributes attr = File.GetAttributes(file);
               Assert.IsTrue(attr == System.IO.File.GetAttributes(file));

               Console.WriteLine("\t\t#{0:000}\t[{1}]\t[{2}]\t\t[{3}]", ++cntAlphaFS, attr, fi.CreationTime, file);
               Assert.IsTrue(!NativeMethods.HasFileAttribute(attr, FileAttributes.Directory), "Not a File: [{0}]", file);
            }
            Console.WriteLine("\n\t{0}\n", Reporter());
            Assert.IsTrue(cntAlphaFS > 0);


            //Console.WriteLine("\t[System.IO] (Date) Enumerate files > [{0}], using \"SearchOption.TopDirectoryOnly\", from directory: [{1}]\n", dateStart, path);
            //int cntSysIo = 0;
            //StopWatcher(true);
            //foreach (System.IO.FileInfo fi in dirInfoSysIO.EnumerateFiles().Where(file => file.CreationTimeUtc > dateStart))
            //{
            //   string file = fi.FullName;
            //   FileAttributes attr = fi.Attributes;
            //   Assert.IsTrue(attr == System.IO.File.GetAttributes(file));

            //   Console.WriteLine("\t\t#{0:000}\t[{1}]\t[{2}]\t[{3}]", ++cntSysIo, attr, fi.CreationTimeUtc, file);
            //   Assert.IsTrue(!NativeMethods.HasFileAttribute(attr, FileAttributes.Directory), "Not a File: [{0}]", file);
            //}
            //Console.WriteLine("\n\t{0}\n", Reporter());
            //Assert.IsTrue(cntSysIo > 0);


            //Assert.AreEqual(cntSysIo, cntAlphaFS);

            #endregion // Date

            #region Pattern

            searchPattern = "b*";
            searchOption = SearchOption.AllDirectories;

            #region Exception

            bool gotException = false;
            StopWatcher(true);
            try
            {
               foreach (FileInfo fi in dirInfo.EnumerateFiles(searchPattern, searchOption))
               {
               }
            }
            catch (Exception ex)
            {
               gotException = true;
               Console.WriteLine("\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));
            }
            Console.WriteLine("\n\tCaught Exception: [{0}]\t{1}", gotException, Reporter());
            Assert.IsTrue(gotException);

            #endregion // Exception

            Console.WriteLine("\n\t[AlphaFS] (Pattern) Enumerate files that start with: [{0}], using \"SearchOption.{1}\" from directory: [{2}]\n", searchPattern, searchOption, path);
            cntAlphaFS = 0;
            StopWatcher(true);
            foreach (FileInfo fi in dirInfo.EnumerateFiles(searchPattern, searchOption, true))
            {
               string file = fi.FullName;
               FileAttributes attr = fi.Attributes;

               Console.WriteLine("\t\t#{0:000}\t[{1}]\t[{2}]", ++cntAlphaFS, attr, file);
               Assert.IsTrue(!NativeMethods.HasFileAttribute(attr, FileAttributes.Directory), "Not a File: [{0}]", file);
               Assert.IsTrue(fi.Name.StartsWith(searchPattern[0].ToString(CultureInfo.CurrentCulture), StringComparison.OrdinalIgnoreCase));
            }
            Console.WriteLine("\n\t{0}\n", Reporter());
            Assert.IsTrue(cntAlphaFS > 0);


            //searchPattern = "b*";
            //Console.WriteLine("\t[System.IO] (Pattern) Enumerate files that start with: [{0}], using \"SearchOption.AllDirectories\" from directory: [{1}]\n", searchPattern, path);
            //cntSysIo = 0;
            //StopWatcher(true);
            //foreach (System.IO.FileInfo fi in dirInfoSysIO.EnumerateFiles(searchPattern, searchOption))
            //{
            //   Console.WriteLine("\t\t#{0:000}\t[File]\t[{1}]", ++cntSysIo, fi.FullName);
            //   Assert.IsTrue(fi.Name.StartsWith(searchPattern[0].ToString(CultureInfo.CurrentCulture), StringComparison.OrdinalIgnoreCase));
            //}
            //Console.WriteLine("\n\t{0}\n", Reporter());
            //Assert.IsTrue(cntSysIo > 0);


            //Assert.AreEqual(cntSysIo, cntAlphaFS);
         //}
         //catch (Exception ex)
         //{
            //Console.WriteLine("\n\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));
         //}

            #endregion // Pattern
      }

      private static void DumpGetAccessControl(bool isLocal)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");
         string path = SysRoot;
         if (!isLocal)
            path = Path.LocalToUnc(path);

         DirectorySecurity gac;
         AuthorizationRuleCollection accessRules;

         Directory.CreateDirectory(path);

         bool foundRules = false;

         StopWatcher(true);
         gac = Directory.GetAccessControl(path);
         StopWatcher();
         accessRules = gac.GetAccessRules(true, true, typeof(NTAccount));

         Console.WriteLine("\n\tInput Path: [{0}]\tGetAccessControl() rules found: [{1}]\n{2}", path, accessRules.Count, Reporter());

         foreach (FileSystemAccessRule far in accessRules)
         {
            Dump(far, 17);
            foundRules = true;
         }
         Assert.IsTrue(foundRules);
      }

      private static void DumpGetFiles(bool isLocal)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");
         string path = isLocal ? SysRoot : Path.LocalToUnc(SysRoot);

         Console.WriteLine("\n\tGet files using \"SearchOption.TopDirectoryOnly\", from directory: [{0}]\n", path);

         int cnt = 0;
         StopWatcher(true);
         foreach (string file in Directory.GetFiles(path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly))
         {
            Console.WriteLine("\t\t#{0:000}\t[{1}]", ++cnt, file);
            Assert.IsTrue((Directory.GetAttributes(file) & FileAttributes.Directory) != FileAttributes.Directory);
         }
         Console.WriteLine("\n\t{0}\n", Reporter());
         Assert.IsTrue(cnt > 0);
      }

      private static void DumpGetFileIdBothDirectoryInfo(bool isLocal)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");
         string path = isLocal ? Path.GetTempPath() : Path.LocalToUnc(Path.GetTempPath());

         long folders = Directory.CountDirectories(path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true);
         long files = Directory.CountFiles(path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true);
         Console.WriteLine("\n\tInput Path: [{0}]", path);
         Console.WriteLine("\tFolders: [{0}], Files: [{1}]", folders, files);

         bool foundFse = false;
         long numFolders = 0;
         long numFiles = 0;

         StopWatcher(true);
         foreach (FileIdBothDirectoryInfo fse in Directory.GetFileIdBothDirectoryInfo(path))
         {
            // Moved check to method: Directory.GetFileIdBothDirectoryInfo();
            //if (fse.FileName == "." || fse.FileName == "..")
            //continue;

            if (NativeMethods.HasFileAttribute(fse.FileAttributes, FileAttributes.Directory))
               numFolders++;
            else
               numFiles++;

            foundFse = Dump(fse, -22);
         }

         bool matchAll = folders == numFolders && files == numFiles;
         Console.WriteLine("\n\tFolders = [{0}], Files = [{1}]\n{2}", numFolders, numFiles, Reporter());
         Assert.IsTrue(foundFse);
         Assert.IsTrue(matchAll, "Number of Folders and/or Files don't match.");
      }
      
      private static void DumpGetProperties(bool isLocal)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");
         string path = isLocal ? SysRoot32 : Path.LocalToUnc(SysRoot32);

         Console.WriteLine("\n\tAggregated properties of file system objects from Directory: [{0}]\n", path);

         StopWatcher(true);
         Dictionary<string, long> props = Directory.GetProperties(path, true, SearchOption.AllDirectories);
         string report = Reporter();

         long total = props["Total"];
         long file = props["File"];
         long size = props["Size"];
         int cnt = 0;

         foreach (var key in props.Keys)
            Console.WriteLine("\t\t#{0:000}\t{1, -17} = [{2}]", ++cnt, key, props[key]);

         Console.WriteLine("\n\t{0}", report);

         Assert.IsTrue(cnt > 0);
         Assert.IsTrue(total > 0, "0 Objects.");
         Assert.IsTrue(file > 0, "0 Files.");
         Assert.IsTrue(size > 0, "0 Size.");
      }

      private static void DumpGetCreationTime(bool isLocal)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");
         string path = isLocal ? SysRoot : Path.LocalToUnc(SysRoot);
         // Somehow System.IO seems to disagree with SystemRoot32 directory.

         Console.WriteLine("\n\tInput Path: [{0}]\n", path);

         try
         {
            DateTime actual = Directory.GetCreationTime(path);
            DateTime expected = System.IO.Directory.GetCreationTime(path);
            Console.WriteLine("\t\tGetCreationTime(): [{0}]\t\tSystem.IO: [{1}]", actual, expected);
            Assert.AreEqual(actual, expected, "Directory.GetCreationTime()");

            actual = Directory.GetCreationTimeUtc(path);
            expected = System.IO.Directory.GetCreationTimeUtc(path);
            Console.WriteLine("\t\tGetCreationTimeUtc(): [{0}]\t\tSystem.IO: [{1}]\n", actual, expected);
            Assert.AreEqual(actual, expected, "Directory.GetCreationTimeUtc()");

            actual = Directory.GetLastAccessTime(path);
            expected = System.IO.Directory.GetLastAccessTime(path);
            Console.WriteLine("\t\tGetLastAccessTime(): [{0}]\t\tSystem.IO: [{1}]", actual, expected);
            Assert.AreEqual(actual, expected, "Directory.GetLastAccessTime()");

            actual = Directory.GetLastAccessTimeUtc(path);
            expected = System.IO.Directory.GetLastAccessTimeUtc(path);
            Console.WriteLine("\t\tGetLastAccessTimeUtc(): [{0}]\t\tSystem.IO: [{1}]\n", actual, expected);
            Assert.AreEqual(actual, expected, "Directory.GetLastAccessTimeUtc()");

            actual = Directory.GetLastWriteTime(path);
            expected = System.IO.Directory.GetLastWriteTime(path);
            Console.WriteLine("\t\tGetLastWriteTime(): [{0}]\t\tSystem.IO: [{1}]", actual, expected);
            Assert.AreEqual(actual, expected, "Directory.GetLastWriteTime()");

            actual = Directory.GetLastWriteTimeUtc(path);
            expected = System.IO.Directory.GetLastWriteTimeUtc(path);
            Console.WriteLine("\t\tGetLastWriteTimeUtc() : [{0}]\t\tSystem.IO: [{1}]", actual, expected);
            Assert.AreEqual(actual, expected, "Directory.GetLastWriteTimeUtc()");
         }
         catch (Exception ex)
         {
            Console.WriteLine("\n\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));
         }
      }

      private static void DumpGetFileSystemInfos(bool isLocal)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");
         string path = isLocal ? SysRoot : Path.LocalToUnc(SysRoot);

         DirectoryInfo dirInfo = new DirectoryInfo(path);
         DateTime currentDate = DateTime.Now;
         DateTime dateStart = new DateTime(currentDate.Year - 2, currentDate.Month, currentDate.Day);

         Console.WriteLine("\n\tEnumerate FS entries > [{0}], using \"SearchOption.TopDirectoryOnly\", from directory: [{1}]\n", dateStart, path);

         int cnt = 0;
         StopWatcher(true);
         foreach (FileSystemInfo di in dirInfo.GetFileSystemInfos().Where(fso => fso.SystemInfo.Created > dateStart))
         {
            string fsoType = (di.SystemInfo.IsDirectory) ? "DirectoryInfo" : "FileInfo";
            Console.WriteLine("\t\t#{0:000}\t[{1, -6}]\t[{2}]\t\t[{3}]", ++cnt, fsoType, di.SystemInfo.Created, di.SystemInfo.FullPath);
         }
         Console.WriteLine("\n\t{0}\n", Reporter());
         Assert.IsTrue(cnt > 0);
      }

      #endregion // Dumpers

      #endregion // DirectoryTest Helpers

      #region .NET

      #region CreateDirectory

      [TestMethod]
      public void CreateDirectory()
      {
         Console.WriteLine("Directory.CreateDirectory()");

         DumpCreateDirectory(true);
         DumpCreateDirectory(false);
      }

      #endregion // CreateDirectory

      #region Delete

      [TestMethod]
      public void Delete()
      {
         Console.WriteLine("Directory.Delete()");

         CreateDirectory();
      }

      #endregion // Delete

      #region EnumerateDirectories

      [TestMethod]
      public void EnumerateDirectories()
      {
         Console.WriteLine("Directory.EnumerateDirectories()");
         Console.WriteLine("DirectoryInfo.EnumerateDirectories()");

         DumpEnumerateDirectories(true);
         DumpEnumerateDirectories(false);
      }

      #endregion // EnumerateDirectories
      
      #region EnumerateFiles

      [TestMethod]
      public void EnumerateFiles()
      {
         Console.WriteLine("Directory.EnumerateFiles()");
         Console.WriteLine("DirectoryInfo.EnumerateFiles()");

         DumpEnumerateFiles(true);
         DumpEnumerateFiles(false);
      }

      #endregion // EnumerateFiles

      #region EnumerateFileSystemInfos

      [TestMethod]
      public void EnumerateFileSystemInfos()
      {
         Console.WriteLine("DirectoryInfo.EnumerateFileSystemInfos()");

         GetFileSystemInfos();
      }

      #endregion // EnumerateFileSystemInfos

      #region GetFileSystemInfos

      [TestMethod]
      public void GetFileSystemInfos()
      {
         Console.WriteLine("DirectoryInfo.GetFileSystemInfos()");

         DumpGetFileSystemInfos(true);
         DumpGetFileSystemInfos(false);
      }

      #endregion // GetFileSystemInfos

      #region Exists

      [TestMethod]
      public void Exists()
      {
         Console.WriteLine("Directory.Exists()");

         CreateDirectory();
      }

      #endregion // Exists

      #region GetAccessControl

      [TestMethod]
      public void GetAccessControl()
      {
         Console.WriteLine("Directory.GetAccessControl()");
         Console.WriteLine("DirectoryInfo.GetAccessControl()");

         DumpGetAccessControl(true);
         DumpGetAccessControl(false);
      }

      #endregion // GetAccessControl

      #region GetCreationTime

      [TestMethod]
      public void GetCreationTime()
      {
         Console.WriteLine("Directory.GetCreationTime()");

         DumpGetCreationTime(true);
         DumpGetCreationTime(false);
      }

      #endregion // GetCreationTime

      #region GetCreationTimeUtc

      [TestMethod]
      public void GetCreationTimeUtc()
      {
         Console.WriteLine("Directory.GetCreationTimeUtc()");

         GetCreationTime();
      }

      #endregion // GetCreationTimeUtc

      #region GetCurrentDirectory

      [TestMethod]
      public void GetCurrentDirectory()
      {
         Console.WriteLine("Directory.GetCurrentDirectory()");
         Console.WriteLine("\n\tThe .NET method is used.");
      }

      #endregion // GetCurrentDirectory

      #region GetDirectories

      [TestMethod]
      public void GetDirectories()
      {
         Console.WriteLine("Directory.GetDirectories()");

         string path = SysRoot;
         Console.WriteLine("\n\tGet directories from Local, using \"SearchOption.TopDirectoryOnly\", from directory: [{0}]\n", path);

         int cnt = 0;
         StopWatcher(true);
         foreach (string folder in Directory.GetDirectories(path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly))
         {
            Console.WriteLine("\t\t#{0:000}\t[{1}]", ++cnt, folder);
            Assert.IsTrue((Directory.GetAttributes(folder) & FileAttributes.Directory) == FileAttributes.Directory);
         }
         Console.WriteLine("\n\t{0}\n", Reporter());
         Assert.IsTrue(cnt > 0);


         path = Path.LocalToUnc(SysRoot);
         Console.WriteLine("\n\tGet directories from Network, using \"SearchOption.TopDirectoryOnly\", from directory: [{0}]\n", path);

         cnt = 0;
         StopWatcher(true);
         foreach (string folder in Directory.GetDirectories(path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly))
         {
            Console.WriteLine("\t\t#{0:000}\t[{1}]", ++cnt, folder);
         }
         Console.WriteLine("\n\t{0}\n", Reporter());
         Assert.IsTrue(cnt > 0);
      }

      #endregion // GetDirectories

      #region GetDirectoryRoot

      [TestMethod]
      public void GetDirectoryRoot()
      {
         Console.WriteLine("Directory.GetDirectoryRoot()");

         DumpGetDirectoryRoot();
      }

      #endregion // GetDirectoryRoot

      #region GetFiles

      [TestMethod]
      public void GetFiles()
      {
         Console.WriteLine("Directory.GetFiles()");

         DumpGetFiles(true);
         DumpGetFiles(false);
      }

      #endregion // GetFiles

      #region GetFileSystemEntries

      #endregion // GetFileSystemEntries

      #region GetLastAccessTime

      [TestMethod]
      public void GetLastAccessTime()
      {
         Console.WriteLine("Directory.GetLastAccessTime()");

         GetCreationTime();
      }

      #endregion // GetLastAccessTime

      #region GetLastAccessTimeUtc

      [TestMethod]
      public void GetLastAccessTimeUtc()
      {
         Console.WriteLine("Directory.GetLastAccessTimeUtc()");

         GetCreationTime();
      }

      #endregion // GetLastAccessTimeUtc

      #region GetLastWriteTime

      [TestMethod]
      public void GetLastWriteTime()
      {
         Console.WriteLine("Directory.GetLastWriteTime()");

         GetCreationTime();
      }

      #endregion // GetLastWriteTime

      #region GetLastWriteTimeUtc

      [TestMethod]
      public void GetLastWriteTimeUtc()
      {
         Console.WriteLine("Directory.GetLastWriteTimeUtc()");

         GetCreationTime();
      }

      #endregion // GetLastWriteTimeUtc

      #region GetLogicalDrives

      [TestMethod]
      public void GetLogicalDrives()
      {
         Console.WriteLine("Directory.GetLogicalDrives()");
         Console.WriteLine("\nIf your mapped network drives don't show up, take a look at this: {0}\n", "http://alphafs.codeplex.com/discussions/397693");

         #region Logical Drives

         int cnt = 0;
         bool isReady;

         // Get Logical Drives from Local Host.
         foreach (string drive in Directory.GetLogicalDrives())
         {
            StopWatcher(true);

            isReady = Volume.IsReady(drive);

            Console.WriteLine("\t#{0:000} Logical Drive = [{1}]\tIsReady = [{2}]\n{3}\n", ++cnt, drive, isReady, Reporter());
         }

         Assert.IsTrue(cnt > 0);

         #endregion // Logical Drives

         #region Logical Drives from Environment

         Console.WriteLine("\n\n\tGet from Environment: .IsReady, strip trailing backslash.\n");

         cnt = 0;

         // Get Logical Drives from Environment, .IsReady Drives only, strip trailing backslash.
         foreach (string drive in Directory.GetLogicalDrives(true, true, true))
         {
            StopWatcher(true);

            isReady = Volume.IsReady(drive);

            Console.WriteLine("\t#{0:000} Logical Drive [{1}] IsReady = {2}\n{3}\n", ++cnt, drive, isReady, Reporter());

            Assert.IsTrue(isReady);
         }

         Assert.IsTrue(cnt > 0);

         #endregion // Logical Drives from Environment
      }

      #endregion // GetLogicalDrives

      #region GetParent

      [TestMethod]
      public void GetParent()
      {
         Console.WriteLine("Directory.GetParent()");

         DumpGetParent();
      }

      #endregion // GetParent

      #region Move

      // The AlphaFS implementation replaces the .NET implementation.

      #endregion // Move

      #region SetAccessControl

      [TestMethod]
      public void SetAccessControl()
      {
         Console.WriteLine("Directory.SetAccessControl()");
         Console.WriteLine("DirectoryInfo.SetAccessControl()");

         //string path = SysDrive + @"\AlphaDirectory-" + Path.GetRandomFileName();
         string path = Path.Combine(Path.GetTempPath(), "Directory.GetAccessControl()-" + Path.GetRandomFileName());
         string pathAlpha = Path.PrefixLongPath(path);
         Directory.CreateDirectory(path);

         Console.WriteLine("\n\tDirectory: [{0}]", path);

         // Initial read.
         Console.WriteLine("\n\tInitial read.");
         DirectorySecurity dsAlpha = Directory.GetAccessControl(pathAlpha, AccessControlSections.Access);
         DirectorySecurity dsSystem = System.IO.Directory.GetAccessControl(path, AccessControlSections.Access);
         AuthorizationRuleCollection accessRulesSystem = dsSystem.GetAccessRules(true, true, typeof(NTAccount));
         StopWatcher(true);
         AuthorizationRuleCollection accessRulesAlpha = dsAlpha.GetAccessRules(true, true, typeof(NTAccount));
         Console.WriteLine("\t\tDirectory.GetAccessControl() rules found: [{0}]\n\t{1}", accessRulesAlpha.Count, Reporter());
         Assert.AreEqual(accessRulesSystem.Count, accessRulesAlpha.Count);

         // Sanity check.
         DumpAccessRules(1, dsSystem, dsAlpha);

         // Remove inherited properties.
         // Passing true for first parameter protects the new permission from inheritance, and second parameter removes the existing inherited permissions 
         Console.WriteLine("\n\tRemove inherited properties and persist it.");
         dsAlpha.SetAccessRuleProtection(true, false);
         Assert.IsTrue(Directory.SetAccessControl(pathAlpha, dsAlpha, AccessControlSections.Access));

         // Re-read, using instance methods.
         System.IO.DirectoryInfo diSystem = new System.IO.DirectoryInfo(Path.LocalToUnc(path));
         DirectoryInfo diAlpha = new DirectoryInfo(Path.PrefixLongPath(Path.LocalToUnc(path)));

         dsSystem = diSystem.GetAccessControl(AccessControlSections.Access);
         dsAlpha = diAlpha.GetAccessControl(AccessControlSections.Access);

         // Sanity check.
         DumpAccessRules(2, dsSystem, dsAlpha);

         // Restore inherited properties.
         Console.WriteLine("\n\tRestore inherited properties and persist it.");
         dsAlpha.SetAccessRuleProtection(false, true);
         Assert.IsTrue(Directory.SetAccessControl(pathAlpha, dsAlpha, AccessControlSections.Access));

         // Re-read.
         dsSystem = System.IO.Directory.GetAccessControl(path, AccessControlSections.Access);
         dsAlpha = Directory.GetAccessControl(pathAlpha, AccessControlSections.Access);

         // Sanity check.
         DumpAccessRules(3, dsSystem, dsAlpha);

         diAlpha.Delete();
         bool folderNotExists = !diAlpha.Exists;
         Console.WriteLine("\n\t(Deleted tempdirectory == [{0}]: {1})", TextTrue, folderNotExists);
         Assert.IsTrue(folderNotExists);
      }

      #endregion // SetAccessControl

      #region SetCurrentDirectory

      [TestMethod]
      public void SetCurrentDirectory()
      {
         Console.WriteLine("Directory.SetCurrentDirectory()");
         Console.WriteLine("\n\tThe .NET method is used.");

         Directory.SetCurrentDirectory(Path.GetTempPath());

         string currentFolder1 = Directory.GetCurrentDirectory();
         string currentFolder2 = System.IO.Directory.GetCurrentDirectory();
         Console.WriteLine("\n\t          Directory.GetCurrentDirectory(): [{0}]", currentFolder1);
         Console.WriteLine("\n\tSystem.IO.Directory.GetCurrentDirectory(): [{0}]\n", currentFolder2);
         // Not much of a test since AlphaFS Directory.GetCurrentDirectory() actually calls System.IO.Directory.GetCurrentDirectory().
         Assert.IsTrue(currentFolder1.Equals((currentFolder2)));

         Directory.SetCurrentDirectory(SysRoot);
         currentFolder1 = Directory.GetCurrentDirectory();
         currentFolder2 = System.IO.Directory.GetCurrentDirectory();
         Console.WriteLine("\n\t          Directory.SetCurrentDirectory(): [{0}]\n", SysRoot);
         Console.WriteLine("\n\t          Directory.GetCurrentDirectory(): [{0}]", currentFolder1);
         Console.WriteLine("\n\tSystem.IO.Directory.GetCurrentDirectory(): [{0}]", currentFolder2);
         // Not much of a test since AlphaFS Directory.GetCurrentDirectory() actually calls System.IO.Directory.GetCurrentDirectory().
         Assert.IsTrue(currentFolder1.Equals((currentFolder2)));
      }

      #endregion // SetCurrentDirectory

      #endregion // .NET

      #region AlphaFS

      #region Compress/Decompress

      [TestMethod]
      public void Compress()
      {
         Console.WriteLine("Directory.Compress()");

         int cnt = 0;
         string tempFolder = Path.Combine(Path.GetTempPath(), TextHelloWorld);
         if (!Directory.Exists(tempFolder))
            Directory.CreateDirectory(tempFolder);

         FileAttributes attrs = File.GetAttributes(tempFolder);
         //bool notCompressed = ((attrs & FileAttributes.Compressed) == 0);
         bool notCompressed = !NativeMethods.HasFileAttribute(attrs, FileAttributes.Compressed);


         Console.WriteLine("\n\t\tFolder: [{0}]\t\t\t\tCompressed == [{1}]: {2}\t\t{3}", tempFolder, TextFalse, notCompressed, attrs);
         Assert.IsTrue(notCompressed && !NativeMethods.HasFileAttribute(attrs, FileAttributes.Compressed));

         // Create some folders and files.
         for (int i = 0; i < 5; i++)
         {
            string file = Path.Combine(tempFolder, Path.GetRandomFileName());

            string dir = file + "-dir";
            Directory.CreateDirectory(dir);

            // using() == Dispose() == Close() = deletable.
            using (File.Create(file)) { }
            using (File.Create(Path.Combine(dir, Path.GetFileName(file)))) { }

            attrs = File.GetAttributes(file);
            notCompressed = !NativeMethods.HasFileAttribute(attrs, FileAttributes.Compressed);

            Console.WriteLine("\t#{0:000}\t  File: [{1}]\t\tCompressed == [{2}]: {3}\t\t{4}", ++cnt, Path.GetFullPath(file), TextFalse, notCompressed, attrs);
            Assert.IsTrue(cnt > 0 && notCompressed);
         }


         StopWatcher(true);

         // Compress directory recursively.
         bool compressOk = Directory.Compress(tempFolder, Path.WildcardStarMatchAll, SearchOption.AllDirectories);

         // Can also be applied on an instance of DirectoryInfo():
         //    DirectoryInfo dirInfo = new DirectoryInfo(tempFolder);
         //    dirInfo.Compress();

         attrs = File.GetAttributes(tempFolder);
         Console.WriteLine("\n\tDirectory compressed recursively successful == [{0}]: {1}\t\t{2}\n{3}\n", TextTrue, compressOk, attrs, Reporter());
         Assert.IsTrue(compressOk && NativeMethods.HasFileAttribute(attrs, FileAttributes.Compressed));

         // Check if everything is compressed.
         cnt = 0;
         foreach (FileSystemInfo fso in Directory.EnumerateFileSystemInfos(tempFolder, Path.WildcardStarMatchAll, SearchOption.AllDirectories))
         {
            attrs = fso.Attributes;
            bool isCompressed = NativeMethods.HasFileAttribute(attrs, FileAttributes.Compressed);

            Console.WriteLine("\t#{0:000}\tFS Entry: [{1}]\t\tCompressed == [{2}]: {3}\t\t{4}", ++cnt, fso.SystemInfo.FullPath.Replace(tempFolder, ""), TextTrue, isCompressed, attrs);
            Assert.IsTrue(cnt > 0);
            Assert.IsTrue(isCompressed);
         }


         // Decompress directory recursively.
         StopWatcher(true);
         bool decompressOk = Directory.Decompress(tempFolder, Path.WildcardStarMatchAll, SearchOption.AllDirectories);
         attrs = File.GetAttributes(tempFolder);
         Console.WriteLine("\n\tDirectory decompressed recursively successful == [{0}]: {1}\t\t{2}\n{3}\n", TextTrue, decompressOk, attrs, Reporter());
         Assert.IsTrue(decompressOk && !NativeMethods.HasFileAttribute(attrs, FileAttributes.Compressed));

         // Check if everything is decompressed.
         cnt = 0;
         foreach (FileSystemInfo fso in Directory.EnumerateFileSystemInfos(tempFolder, Path.WildcardStarMatchAll, SearchOption.AllDirectories))
         {
            attrs = fso.Attributes;
            notCompressed = !NativeMethods.HasFileAttribute(attrs, FileAttributes.Compressed);

            Console.WriteLine("\t#{0:000}\tFS Entry: [{1}]\t\tDecompressed == [{2}]: {3}\t\t{4}", ++cnt, fso.SystemInfo.FullPath.Replace(tempFolder, ""), TextTrue, notCompressed, attrs);
            Assert.IsTrue(cnt > 0 && notCompressed);
         }


         Directory.Delete(tempFolder, true);
         bool deleteOk = !Directory.Exists(tempFolder);
         Console.WriteLine("\n\t(Deleted tempfolder == [{0}]: {1})", TextTrue, deleteOk);
         Assert.IsTrue(deleteOk);
      }

      [TestMethod]
      public void Decompress()
      {
         Console.WriteLine("Directory.Decompress()");

         Compress();
      }

      [TestMethod]
      public void CompressionDisable()
      {
         Console.WriteLine("Directory.CompressionDisable()");

         CompressionEnable();
      }

      [TestMethod]
      public void CompressionEnable()
      {
         Console.WriteLine("Directory.CompressionEnable()");

         Directory.SetCurrentDirectory(Path.GetTempPath());
         string currentFolder = Directory.GetCurrentDirectory();

         string tempFolder = Path.Combine(currentFolder, TextHelloWorld);

         Directory.CreateDirectory(tempFolder);
         FileAttributes attrs = File.GetAttributes(tempFolder);

         Console.WriteLine("\n\tCurrent folder        = [{0}]", currentFolder);
         Console.WriteLine("\tCreated subfolder     = [{0}]", tempFolder);
         Console.WriteLine("\tFile.GetAttributes() == [{0}]", attrs);
         Assert.IsTrue(!NativeMethods.HasFileAttribute(attrs, FileAttributes.Compressed));

         bool compressOk = Directory.CompressionEnable(tempFolder);
         attrs = File.GetAttributes(tempFolder);
         Console.WriteLine("\n\tDirectory.CompressionEnable() successful   == [{0}]: {1}", TextTrue, compressOk);
         Console.WriteLine("\tFile.GetAttributes()                       == [{0}]", attrs);
         Assert.IsTrue(compressOk && NativeMethods.HasFileAttribute(attrs, FileAttributes.Compressed));

         bool decompressOk = Directory.CompressionDisable(tempFolder);
         attrs = File.GetAttributes(tempFolder);
         Console.WriteLine("\n\tDirectory.CompressionDisable() successful   == [{0}]: {1}", TextTrue, decompressOk);
         Console.WriteLine("\tFile.GetAttributes()                        == [{0}]", attrs);
         Assert.IsTrue(decompressOk && !NativeMethods.HasFileAttribute(attrs, FileAttributes.Compressed));

         Directory.Delete(tempFolder);
         bool deleteOk = !Directory.Exists(tempFolder);
         Console.WriteLine("\n\t(Deleted tempfolder == [{0}]: {1})", TextTrue, deleteOk);
         Assert.IsTrue(deleteOk);
      }

      #endregion Compress/Decompress

      #region Copy

      [TestMethod]
      public void Copy()
      {
         Console.WriteLine("Directory.Copy()");
         Console.WriteLine("DirectoryInfo.CopyTo()");

         string path = Path.GetTempPath("Directory-Copy()-" + Path.GetRandomFileName());

         DirectoryInfo sourceDir = new DirectoryInfo(Path.GetTempPath(path + @"\sourceDir"));
         DirectoryInfo destDir = new DirectoryInfo(Path.GetTempPath(path + @"\destinationDir"));
         string the3rdDir = Path.LocalToUnc(Path.GetTempPath(path + @"\the3rdDirectory"));

         // Create some folders and files.
         CreateFoldersAndFiles(sourceDir.FullName, 10, true);

         Console.WriteLine("\n\tSRC Folder: [{0}]", sourceDir.FullName);

         #region CopyTo

         StopWatcher(true);
         bool copyOk = sourceDir.CopyTo(destDir.FullName);
         Console.WriteLine("\n\tDirectoryInfo.CopyTo() == [{0}]: {1}", TextTrue, copyOk);
         Console.WriteLine("\t\tDST Folder: [{0}]\n\t{1}", destDir.FullName, Reporter());
         Assert.IsTrue(copyOk);

         #endregion // CopyTo

         #region Copy

         StopWatcher(true);
         copyOk = Directory.Copy(sourceDir.FullName, the3rdDir);
         Console.WriteLine("\n\tDirectory.Copy() == [{0}]: {1}", TextTrue, copyOk);
         Console.WriteLine("\t\t3RD Folder: [{0}]\n\t{1}", the3rdDir, Reporter());
         Assert.IsTrue(copyOk);

         #endregion // Copy

         #region Copy, delete destination first.

         // Fail.
         copyOk = false;
         try
         {
            copyOk = sourceDir.CopyTo(new DirectoryInfo(the3rdDir).FullName);
         }
         catch (Exception ex)
         {
            Console.WriteLine("\n\tException: [{0}] == {1}", ex.Message, TextTrue);
            Console.WriteLine("\n\tTry again, Remove destination first.");
         }
         Assert.IsFalse(copyOk);

         StopWatcher(true);
         copyOk = sourceDir.CopyTo(new DirectoryInfo(the3rdDir).FullName, true);
         Console.WriteLine("\n\tDirectoryInfo.CopyTo(true) == [{0}]: {1}", TextTrue, copyOk);
         Console.WriteLine("\t\t3RD Folder: [{0}]\n\t{1}", the3rdDir, Reporter());
         Assert.IsTrue(copyOk);

         #endregion // Copy, delete destination first.

         sourceDir.Delete(true);
         destDir.Delete(true);
         Assert.IsTrue(!sourceDir.Exists);
         Assert.IsTrue(!destDir.Exists);

         Directory.Delete(path, true);
         Assert.IsTrue(!Directory.Exists(path));
      }

      #endregion // Copy

      #region CountDirectories

      [TestMethod]
      public void CountDirectories()
      {
         Console.WriteLine("Directory.CountDirectories()");

         string path = SysRoot;
         long files = 0;
         Console.WriteLine("\n\tSearchOption.AllDirectories, abort on error.");

         #region Exception

         bool gotException = false;
         try
         {
            files = Directory.CountDirectories(path, Path.WildcardStarMatchAll, SearchOption.AllDirectories);
         }
         catch (Exception ex)
         {
            gotException = true;
            Console.WriteLine("\n\t\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));
         }
         Console.WriteLine("\n\t\tCaught Exception: [{0}]", gotException);
         Assert.IsTrue(gotException);
         Console.WriteLine("\n\tFolder\t    = [{0}]\n\tSubFolders = [{1}]\n{2}\n", path, files, Reporter());

         #endregion // Exception

         Console.WriteLine("\n\tSearchOption.AllDirectories, continue on error. Running as Administrator will count more directories.");
         StopWatcher(true);
         files = Directory.CountDirectories(path, Path.WildcardStarMatchAll, SearchOption.AllDirectories, true);
         Console.WriteLine("\n\tFolder\t    = [{0}]\n\tSubFolders = [{1}]\n{2}\n", path, files, Reporter());
         Assert.IsTrue(files > 0);

         Console.WriteLine("\n\tSearchOption.AllDirectories, continue on error. Running as Administrator and using PrivilegeEnabler(Privilege.Backup) will count even more directories.");
         StopWatcher(true);
         using (new PrivilegeEnabler(Privilege.Backup))
         {
            files = Directory.CountDirectories(path, Path.WildcardStarMatchAll, SearchOption.AllDirectories, true);
            Console.WriteLine("\n\tFolder\t    = [{0}]\n\tSubFolders = [{1}]\n{2}\n", path, files, Reporter());
            Assert.IsTrue(files > 0);
         }

      }

      #endregion // CountDirectories

      #region CountFiles

      [TestMethod]
      public void CountFiles()
      {
         Console.WriteLine("Directory.CountFiles()");

         string path = SysRoot32;
         long files = 0;
         Console.WriteLine("\n\tSearchOption.AllDirectories, abort on error.");

         #region Exception

         bool gotException = false;
         try
         {
            files = Directory.CountFiles(path, Path.WildcardStarMatchAll, SearchOption.AllDirectories);
         }
         catch (Exception ex)
         {
            gotException = true;
            Console.WriteLine("\n\t\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));
         }
         Console.WriteLine("\n\t\tCaught Exception: [{0}]", gotException);
         Assert.IsTrue(gotException);
         Console.WriteLine("\n\tFolder = [{0}]\n\tFiles  = [{1}]\n{2}\n", path, files, Reporter());

         #endregion // Exception

         StopWatcher(true);
         files = Directory.CountFiles(path, Path.WildcardStarMatchAll, SearchOption.AllDirectories, true);
         Console.WriteLine("\n\tFolder = [{0}]\n\tFiles  = [{1}]\n{2}\n", path, files, Reporter());
         Assert.IsTrue(files > 0);

         Console.WriteLine("\n\tSearchOption.AllDirectories, continue on error. Running as Administrator will count more files.");
         StopWatcher(true);
         files = Directory.CountFiles(path, Path.WildcardStarMatchAll, SearchOption.AllDirectories, true);
         Console.WriteLine("\n\tFolder = [{0}]\n\tFiles  = [{1}]\n{2}\n", path, files, Reporter());
         Assert.IsTrue(files > 0);

         Console.WriteLine("\n\tSearchOption.AllDirectories, continue on error. Running as Administrator and using PrivilegeEnabler(Privilege.Backup) will count even more files.");
         StopWatcher(true);
         using (new PrivilegeEnabler(Privilege.Backup))
         {
            files = Directory.CountFiles(path, Path.WildcardStarMatchAll, SearchOption.AllDirectories, true);
            Console.WriteLine("\n\tFolder = [{0}]\n\tFiles  = [{1}]\n{2}\n", path, files, Reporter());
            Assert.IsTrue(files > 0);
         }

      }

      #endregion // CountFiles
      
      #region GetProperties

      [TestMethod]
      public void GetProperties()
      {
         Console.WriteLine("Directory.GetProperties()");

         DumpGetProperties(true);
         DumpGetProperties(false);
      }

      #endregion // GetProperties

      #region Encrypt/Decrypt

      [TestMethod]
      public void Encrypt()
      {
         Console.WriteLine("Directory.Encrypt()");

         int cnt = 0;
         string tempFolder = Path.Combine(Path.GetTempPath(), TextHelloWorld);
         if (!Directory.Exists(tempFolder))
            Directory.CreateDirectory(tempFolder);

         FileAttributes attrs = File.GetAttributes(tempFolder);
         bool notEncypted = !NativeMethods.HasFileAttribute(attrs, FileAttributes.Encrypted);

         Console.WriteLine("\n\t\tFolder: [{0}]\t\t\t\tEncrypted == [{1}]: {2}\t\t{3}", tempFolder, TextFalse, notEncypted, attrs);
         Assert.IsTrue(notEncypted && !NativeMethods.HasFileAttribute(attrs, FileAttributes.Encrypted));

         // Create some folders and files.
         for (int i = 0; i < 5; i++)
         {
            string file = Path.Combine(tempFolder, Path.GetRandomFileName());

            string dir = file + "-dir";
            Directory.CreateDirectory(dir);

            // using() == Dispose() == Close() = deletable.
            using (File.Create(file)) { }
            using (File.Create(Path.Combine(dir, Path.GetFileName(file)))) { }

            attrs = File.GetAttributes(file);
            notEncypted = !NativeMethods.HasFileAttribute(attrs, FileAttributes.Encrypted);

            Console.WriteLine("\t#{0:000}\t  File: [{1}]\t\tEncrypted == [{2}]: {3}\t\t{4}", ++cnt, Path.GetFullPath(file), TextFalse, notEncypted, attrs);
            Assert.IsTrue(cnt > 0 && notEncypted);
         }


         StopWatcher(true);

         // Encrypt directory recursively.
         bool encryptOk = Directory.Encrypt(tempFolder, Path.WildcardStarMatchAll, SearchOption.AllDirectories);

         // Can also be applied on an instance of DirectoryInfo():
         //    DirectoryInfo dirInfo = new DirectoryInfo(tempFolder);
         //    dirInfo.Encrypt();

         attrs = File.GetAttributes(tempFolder);
         Console.WriteLine("\n\tDirectory encrypted recursively successful == [{0}]: {1}\t\t{2}\n{3}\n", TextTrue, encryptOk, attrs, Reporter());
         Assert.IsTrue(encryptOk && NativeMethods.HasFileAttribute(attrs, FileAttributes.Encrypted));

         // Check if everything is encrypted.
         cnt = 0;
         foreach (FileSystemInfo fso in Directory.EnumerateFileSystemInfos(tempFolder, Path.WildcardStarMatchAll, SearchOption.AllDirectories))
         {
            attrs = fso.Attributes;
            bool isEncrypted = NativeMethods.HasFileAttribute(attrs, FileAttributes.Encrypted);

            Console.WriteLine("\t#{0:000}\tFS Entry: [{1}]\t\tEncrypted == [{2}]: {3}\t\t{4}", ++cnt, fso.SystemInfo.FullPath.Replace(tempFolder, ""), TextTrue, isEncrypted, attrs);
            Assert.IsTrue(cnt > 0 && isEncrypted);
         }


         // Decrypt directory recursively.
         StopWatcher(true);
         bool decryptOk = Directory.Decrypt(tempFolder, Path.WildcardStarMatchAll, SearchOption.AllDirectories);
         attrs = File.GetAttributes(tempFolder);
         Console.WriteLine("\n\tDirectory decrypted recursively successful == [{0}]: {1}\t\t{2}\n{3}\n", TextTrue, decryptOk, attrs, Reporter());
         Assert.IsTrue(decryptOk && !NativeMethods.HasFileAttribute(attrs, FileAttributes.Encrypted));

         // Check if everything is decrypted.
         cnt = 0;
         foreach (FileSystemInfo fso in Directory.EnumerateFileSystemInfos(tempFolder, Path.WildcardStarMatchAll, SearchOption.AllDirectories))
         {
            attrs = fso.Attributes;
            notEncypted = !NativeMethods.HasFileAttribute(attrs, FileAttributes.Encrypted);

            Console.WriteLine("\t#{0:000}\tFS Entry: [{1}]\t\tDecrypted == [{2}]: {3}\t\t{4}", ++cnt, fso.SystemInfo.FullPath.Replace(tempFolder, ""), TextTrue, notEncypted, attrs);
            Assert.IsTrue(cnt > 0 && notEncypted);
         }


         Directory.Delete(tempFolder, true);
         bool deleteOk = !Directory.Exists(tempFolder);
         Console.WriteLine("\n\t(Deleted tempfolder == [{0}]: {1})", TextTrue, deleteOk);
         Assert.IsTrue(deleteOk);
      }

      [TestMethod]
      public void Decrypt()
      {
         Console.WriteLine("Directory.Decrypt()");

         Encrypt();
      }

      [TestMethod]
      public void EncryptionDisable()
      {
         Console.WriteLine("Directory.EncryptionDisable()");

         EncryptionEnable();
      }

      [TestMethod]
      public void EncryptionEnable()
      {
         Console.WriteLine("Directory.EncryptionEnable()");

         Directory.SetCurrentDirectory(Path.GetTempPath());
         string currentFolder = Directory.GetCurrentDirectory();
         string tempFolder = Path.Combine(currentFolder, TextHelloWorld);
         string disabled = "Disable=0";
         string enabled = "Disable=1";
         string lineDisable = string.Empty;
         string deskTopIni = Path.Combine(tempFolder, "Desktop.ini");

         Directory.CreateDirectory(tempFolder);
         FileAttributes attrs = File.GetAttributes(tempFolder);

         Console.WriteLine("\n\tCurrent folder        = [{0}]", currentFolder);
         Console.WriteLine("\tCreated subfolder     = [{0}]", tempFolder);

         // This method will create/change the file "Desktop.ini" and wil set Encryption value: "Disable=1"
         bool encryptOk = Directory.EncryptionEnable(tempFolder);

         // Read filestream contents, get the last line.
         using (StreamReader streamRead = File.OpenText(deskTopIni))
         {
            string line;
            while ((line = streamRead.ReadLine()) != null)
            {
               lineDisable = line;
            }
         }
         bool isDisabled = lineDisable.Equals(disabled);
         Console.WriteLine("\n\tDirectory.EncryptionEnable() successful == [{0}]: {1}", TextTrue, encryptOk);
         Console.WriteLine("\n\t\tContents: [{0}] of file: [{1}]", lineDisable, deskTopIni);
         Console.WriteLine("\n\t\t{0} == [{1}]: {2}", disabled, TextTrue, isDisabled);
         Assert.IsTrue(encryptOk && File.Exists(deskTopIni) && isDisabled);

         // This method will create/change the file "Desktop.ini" and wil set Encryption value: "Disable=0"
         bool decryptOk = Directory.EncryptionDisable(tempFolder);

         // Read filestream contents, get the last line.
         using (StreamReader streamRead = File.OpenText(deskTopIni))
         {
            string line;
            while ((line = streamRead.ReadLine()) != null)
            {
               lineDisable = line;
            }
         }
         bool isEnabled = lineDisable.Equals(enabled);
         Console.WriteLine("\n\tDirectory.EncryptionDisable() successful   == [{0}]: {1}", TextTrue, decryptOk);
         Console.WriteLine("\n\t\tContents: [{0}] of file: [{1}]", lineDisable, deskTopIni);
         Console.WriteLine("\n\t\t{0} == [{1}]: {2}\n", enabled, TextTrue, isEnabled);
         Assert.IsTrue(decryptOk && File.Exists(deskTopIni) && isEnabled);

         Directory.Delete(tempFolder, true);
         bool deleteOk = !Directory.Exists(tempFolder);
         Console.WriteLine("\n\t(Deleted tempfolder == [{0}]: {1})", TextTrue, deleteOk);
         Assert.IsTrue(deleteOk);
      }

      #endregion // Encrypt/Decrypt

      #region EnumerateStreams

      [TestMethod]
      public void EnumerateStreams()
      {
         Console.WriteLine("Directory.EnumerateStreams()");
         Console.WriteLine("DirectoryInfo.EnumerateStreams()");

         string extraStream = ":ThisIsAnExtraStream";
         string addedStream = " - Extra Stream Information added: ";
         string path = Path.Combine(Path.GetTempPath(), "Directory.EnumerateStreams()-" + Path.GetRandomFileName());
         DirectoryInfo dirInfo = new DirectoryInfo(path);
         dirInfo.Create();

         // Create some files.
         const int numFiles = 6;
         for (int i = 0; i < numFiles; i++)
         {
            string newPath = Path.Combine(path, i + "-" + Path.GetRandomFileName());
            File.WriteAllText(newPath, TextHelloWorld);

            // Create additional stream for "odd" files.
            if (i % 2 == 1)
            {
               string txtToStream = "Index #" + i + " = \"" + TextGoodByeWorld + "\"" + addedStream + DateTime.Now + "." + DateTime.Now.Ticks;
               File.WriteAllText(newPath + extraStream, txtToStream);
            }
         }

         Console.WriteLine("\n\tDirectory: [{0}]\n", path);

         Console.WriteLine("\t\tCreated {0} files, {1} of them have an Alternate Data Stream.\n", numFiles, numFiles / 2);
         List<string> streamNames = new List<string>();


         int cnt = 0;
         StopWatcher(true);
         foreach (BackupStreamInfo file in dirInfo.EnumerateStreams())
         {
            // Filter out "normal-data streams; the actual file as we know it.
            // Filter out security-data streams.
            if ((file.StreamType & (BackupStreamTypes.Data | BackupStreamTypes.SecurityData)) == 0)
            {
               // This will fail, since the handle is still in use.
               // string txtFromStream = File.ReadAllText(file.Source + file.Name);

               // Instead, collect alle file stream-names and process them later.
               streamNames.Add(file.Source + file.Name);

               Console.WriteLine("\t\t#{0:000}\tFile with ADS: [{1}]", ++cnt, file.Source);
               Console.WriteLine("\t\t\t  Stream name: [{0}]", file.Name);
               Console.WriteLine("\t\t\t  Stream type: [{0}]", file.StreamType);
               Console.WriteLine("\t\t\t  Stream size: [{0}]", file.Size);
               Console.WriteLine("\t\t\t  Stream attr: [{0}]\n", file.Attributes);
            }
         }
         Console.WriteLine("{0}\n", Reporter());
         Assert.IsTrue(cnt > 0);
         Assert.IsTrue(streamNames.Count == numFiles / 2);


         // Show the content of the alternate streams.
         Console.WriteLine("\n\t\tShow the content of the alternate streams.\n");
         cnt = 0;
         StopWatcher(true);
         foreach (string name in streamNames)
         {
            string txtFromStream = File.ReadAllText(name);

            Console.WriteLine("\t\t#{0:000}\tFile with ADS : [{1}]", ++cnt, name.Replace(extraStream + ":$DATA", ""));
            Console.WriteLine("\t\t\tStream content: [{0}]\n", txtFromStream);
            Assert.IsTrue(txtFromStream.Contains(addedStream));
         }
         Console.WriteLine("{0}\n", Reporter());
         Assert.IsTrue(cnt == streamNames.Count);


         Directory.Delete(path, true);
         bool folderNotExists = !Directory.Exists(path);
         Console.WriteLine("\n\t(Deleted tempdirectory == [{0}]: {1})", TextTrue, folderNotExists);
         Assert.IsTrue(folderNotExists);
      }

      #endregion // EnumerateStreams

      #region GetAttributes

      [TestMethod]
      public void GetAttributes()
      {
         Console.WriteLine("Directory.GetAttributes()");

         DumpGetAttributes(true);
         DumpGetAttributes(false);
      }

      #endregion // GetAttributes

      #region GetFileIdBothDirectoryInfo

      [TestMethod]
      public void GetFileIdBothDirectoryInfo()
      {
         Console.WriteLine("Directory.GetFileIdBothDirectoryInfo()");

         DumpGetFileIdBothDirectoryInfo(true);
         DumpGetFileIdBothDirectoryInfo(false);
      }

      #endregion // GetFileIdBothDirectoryInfo

      #region Move

      [TestMethod]
      public void Move()
      {
         Console.WriteLine("Directory.Move()");
         Console.WriteLine("DirectoryInfo.MoveTo()");

         string path = Path.GetTempPath("Move()-" + Path.GetRandomFileName());

         DirectoryInfo sourceDir = new DirectoryInfo(Path.GetTempPath(path + @"\sourceDir"));
         DirectoryInfo destDir = new DirectoryInfo(Path.GetTempPath(path + @"\destinationDir"));
         string the3rdDir = Path.LocalToUnc(Path.GetTempPath(path + @"\the3rdDirectory"));

         // Create some folders and files.
         CreateFoldersAndFiles(sourceDir.FullName, 10, true);

         Console.WriteLine("\n\tSRC Folder: [{0}]", sourceDir.FullName);

         #region MoveTo

         StopWatcher(true);
         bool moveOk = sourceDir.MoveTo(destDir.FullName);
         Console.WriteLine("\n\tDirectoryInfo.MoveTo() == [{0}]: {1}", TextTrue, moveOk);
         Console.WriteLine("\t\tDST Folder: [{0}]\n\t{1}", destDir.FullName, Reporter());
         Assert.IsTrue(moveOk);

         #endregion // MoveTo

         #region Move

         string pathUnc = Path.LocalToUnc(sourceDir.FullName);
         if (Directory.Exists(pathUnc))
         {
            StopWatcher(true);
            moveOk = Directory.Move(sourceDir.FullName, the3rdDir);
            Console.WriteLine("\n\tDirectory.Move() == [{0}]: {1}", TextTrue, moveOk);
            Console.WriteLine("\t\t3RD Folder: [{0}]\n\t{1}", the3rdDir, Reporter());
            Directory.Delete(the3rdDir, true);
            Assert.IsTrue(moveOk);
            Assert.IsTrue(!Directory.Exists(the3rdDir));
         }
         else
            Console.Write("\n\tShare inaccessible: {0}\n", pathUnc);

         #endregion // Move

         #region Move, delete destination first.

         // Create some folders and files.
         CreateFoldersAndFiles(sourceDir.FullName, 10, true);

         StopWatcher(true);
         moveOk = sourceDir.MoveTo(new DirectoryInfo(the3rdDir).FullName, true);
         Console.WriteLine("\n\tDirectory.MoveTo() == [{0}]: {1}", TextTrue, moveOk);
         Console.WriteLine("\t\t3RD Folder: [{0}]\n\t{1}", the3rdDir, Reporter());
         Assert.IsTrue(moveOk);

         #endregion // Move, delete destination first.

         sourceDir.Delete(true);
         Assert.IsTrue(!sourceDir.Exists && !destDir.Exists);

         Directory.Delete(path, true);
         Assert.IsTrue(!Directory.Exists(path));
      }

      #endregion // Move

      #endregion // AlphaFS
   }
}