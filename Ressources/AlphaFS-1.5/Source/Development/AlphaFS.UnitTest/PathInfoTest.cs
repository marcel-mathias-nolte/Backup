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
using System.Globalization;
using System.Linq;
using Alphaleonis.Win32.Filesystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlphaFS.UnitTest
{
   /// <summary>This is a test class for PathInfo and is intended to contain all PathInfo Unit Tests.</summary>
   [TestClass]
   public class PathInfoTest
   {
      #region Setup

      ///// <summary>Gets or sets the test context which provides information about and functionality for the current test run.</summary>
      //public TestContext TestContext { get; set; }

      //[TestInitialize]
      //public void Init()
      //{
      //}

      private static readonly string SysDrive = Environment.GetEnvironmentVariable("SystemDrive");

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

      private static void Dump__Class_PathInfo(bool isLocal)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");

         string[] paths = new[]
            {
               isLocal ? SysDrive : Path.LocalToUnc(SysDrive),
               isLocal ? SysDrive + Path.DirectorySeparatorChar : Path.LocalToUnc(SysDrive + Path.DirectorySeparatorChar),
               isLocal ? Path.GetTempFileName() : Path.LocalToUnc(Path.GetTempFileName())
            };

         string path = paths[0];
         Console.WriteLine("\n\tInput Path: [{0}]", path);
         PathInfo pathInfo = new PathInfo(paths[0]);
         Dump(pathInfo, -32);

         path = paths[1];
         Console.WriteLine("\n\tInput Path: [{0}]", path);
         pathInfo = new PathInfo(path);
         Dump(pathInfo, -32);

         path = paths[2];
         Console.WriteLine("\n\tInput Path: [{0}]", path);
         pathInfo = new PathInfo(path);
         Dump(pathInfo, -32);
         Assert.IsTrue(File.Delete(path));
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

      #endregion // Setup

      #region .NET

      #region DirectoryName

      [TestMethod]
      public void DirectoryName()
      {
         Console.WriteLine("PathInfo.DirectoryName");

         int pathCnt = 0;
         bool allOk = true;
         int errorCnt = 0;

         foreach (string input in InputPaths)
         {
            try
            {
               string actual = new PathInfo(input).DirectoryName;
               string expected = System.IO.Path.GetDirectoryName(input);
               Console.WriteLine("\n\t#{0:000}\tInput Path: [{1}]\t\tDirectoryName: [{2}]\t\tSystem.IO: [{3}]", ++pathCnt, input, actual, expected);
               Assert.AreEqual(expected, actual);
            }
            catch (Exception ex)
            {
               Console.WriteLine("\n\t\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));

               // Exception to the Exception; System.IO.GetDirectoryName() handles this but throws Exception.
               if (input != null && !input.StartsWith(@"\\?\GlobalRoot", StringComparison.OrdinalIgnoreCase))
               {
                  allOk = false;
                  errorCnt++;
               }
            }
         }

         Assert.AreEqual(true, allOk, "Encountered: [{0}] paths where AlphaFS != System.IO", errorCnt);
      }

      #endregion // DirectoryName

      #region Extension

      [TestMethod]
      public void Extension()
      {
         Console.WriteLine("PathInfo.Extension");

         int pathCnt = 0;
         bool allOk = true;
         int errorCnt = 0;

         foreach (string input in InputPaths)
         {
            try
            {
               string actual = new PathInfo(input).Extension;
               string expected = System.IO.Path.GetExtension(input);
               Console.WriteLine("\n\t#{0:000}\tInput Path: [{1}]\t\tExtension: [{2}]\t\tSystem.IO: [{3}]", ++pathCnt, input, actual, expected);
               Assert.AreEqual(expected, actual);
            }
            catch (Exception ex)
            {
               Console.WriteLine("\n\t\tException: [{0}]", ex.Message);
               allOk = false;
               errorCnt++;
            }
         }

         Assert.AreEqual(true, allOk, "Encountered: [{0}] paths where AlphaFS != System.IO", errorCnt);
      }

      #endregion // Extension

      #region FileName

      [TestMethod]
      public void FileName()
      {
         Console.WriteLine("PathInfo.FileName");

         int pathCnt = 0;
         bool allOk = true;
         int errorCnt = 0;

         foreach (string input in InputPaths)
         {
            try
            {
               string actual = new PathInfo(input).FileName;
               string expected = System.IO.Path.GetFileName(input);
               Console.WriteLine("\n\t#{0:000}\tInput Path: [{1}]\t\tFileName: [{2}]\t\tSystem.IO: [{3}]", ++pathCnt, input, actual, expected);
               Assert.AreEqual(expected, actual);
            }
            catch (Exception ex)
            {
               Console.WriteLine("\n\t\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));
               allOk = false;
               errorCnt++;
            }
         }

         Assert.AreEqual(true, allOk, "Encountered: [{0}] paths where AlphaFS != System.IO", errorCnt);
      }

      #endregion // FileName

      #region FilenameWithoutExtension

      [TestMethod]
      public void FilenameWithoutExtension()
      {
         Console.WriteLine("PathInfo.FileNameWithoutExtension");

         int pathCnt = 0;
         bool allOk = true;
         int errorCnt = 0;

         foreach (string input in InputPaths)
         {
            try
            {
               string actual = new PathInfo(input).FileNameWithoutExtension;
               string expected = System.IO.Path.GetFileNameWithoutExtension(input);
               Console.WriteLine("\n\t#{0:000}\tInput Path: [{1}]\t\tFileNameWithoutExtension: [{2}]\t\tSystem.IO: [{3}]", ++pathCnt, input, actual, expected);
               Assert.AreEqual(expected, actual);
            }
            catch (Exception ex)
            {
               Console.WriteLine("\n\t\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));
               allOk = false;
               errorCnt++;
            }
         }

         Assert.AreEqual(true, allOk, "Encountered: [{0}] paths where AlphaFS != System.IO", errorCnt);
      }

      #endregion // FilenameWithoutExtension

      #region GetFullPath

      [TestMethod]
      public void GetFullPath()
      {
         Console.WriteLine("PathInfo.GetFullPath()");

         int pathCnt = 0;
         bool allOk = true;
         int errorCnt = 0;

         string originalPath = System.IO.Directory.GetCurrentDirectory();

         foreach (string input in InputPaths)
         {
            try
            {
               System.IO.Directory.SetCurrentDirectory(@"C:\");

               string actual = new PathInfo(input).GetFullPath();
               string expected = System.IO.Path.GetFullPath(input);
               Console.WriteLine("\n\t#{0:000}\tInput Path: [{1}]\t\tGetFullPath(): [{2}]\t\tSystem.IO: [{3}]", ++pathCnt, input, actual, expected);
               Assert.AreEqual(expected, actual);

               System.IO.Directory.SetCurrentDirectory(originalPath);
            }
            catch (Exception ex)
            {
               Console.WriteLine("\n\t\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));

               // Exception to the Exception; System.IO.GetFullPath() handles this but throws Exception.
               if (input != null && !input.StartsWith(@"\\?\GlobalRoot", StringComparison.OrdinalIgnoreCase) && 
                  !ex.Message.Equals("Illegal characters in path.", StringComparison.OrdinalIgnoreCase))
               {
                  allOk = false;
                  errorCnt++;
               }
            }
         }

         Assert.AreEqual(true, allOk, "Encountered: [{0}] paths where AlphaFS != System.IO", errorCnt);
      }

      #endregion // GetFullPath

      #region HasExtension

      [TestMethod]
      public void HasExtension()
      {
         Console.WriteLine("PathInfo.HasExtension");

         int pathCnt = 0;
         bool allOk = true;
         int errorCnt = 0;

         foreach (string input in InputPaths)
         {
            try
            {
               bool actual = new PathInfo(input).HasExtension;
               bool expected = System.IO.Path.HasExtension(input);
               Console.WriteLine("\n\t#{0:000}\tInput Path: [{1}]\t\tHasExtension: [{2}]\t\tSystem.IO: [{3}]", ++pathCnt, input, actual, expected);
               Assert.AreEqual(expected, actual);
            }
            catch (Exception ex)
            {
               Console.WriteLine("\n\t\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));
               allOk = false;
               errorCnt++;
            }
         }

         Assert.AreEqual(true, allOk, "Encountered: [{0}] paths where AlphaFS != System.IO", errorCnt);
      }

      #endregion // HasExtension

      #region IsRooted

      [TestMethod]
      public void IsRooted()
      {
         Console.WriteLine("PathInfo.IsRooted");

         int pathCnt = 0;
         bool allOk = true;
         int errorCnt = 0;

         foreach (string input in InputPaths)
         {
            try
            {
               bool actual = new PathInfo(input).IsRooted;
               bool expected = System.IO.Path.IsPathRooted(input);
               Console.WriteLine("\n\t#{0:000}\tInput Path: [{1}]\t\tIsRooted: [{2}]\t\tSystem.IO: [{3}]", ++pathCnt, input, actual, expected);
               Assert.AreEqual(expected, actual);
            }
            catch (Exception ex)
            {
               Console.WriteLine("\n\t\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));
               allOk = false;
               errorCnt++;
            }
         }

         Assert.AreEqual(true, allOk, "Encountered: [{0}] paths where AlphaFS != System.IO", errorCnt);
      }

      #endregion // IsRooted

      #region Root

      [TestMethod]
      public void Root()
      {
         Console.WriteLine("PathInfo.Root");

         int pathCnt = 0;
         bool allOk = true;
         int errorCnt = 0;

         foreach (string input in InputPaths)
         {
            try
            {
               string actual = new PathInfo(input).Root;
               string expected = System.IO.Path.GetPathRoot(input);
               Console.WriteLine("\n\t#{0:000}\tInput Path: [{1}]\t\tRoot: [{2}]\t\tSystem.IO: [{3}]", ++pathCnt, input, actual, expected);
               Assert.AreEqual(expected, actual);
            }
            catch (Exception ex)
            {
               Console.WriteLine("\n\t\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));

               // Exception to the Exception; System.IO.GetDirectoryName() handles this but throws Exception.
               if (input != null && !input.StartsWith(@"\\?\GlobalRoot", StringComparison.OrdinalIgnoreCase))
               {
                  allOk = false;
                  errorCnt++;
               }
            }
         }

         Assert.AreEqual(true, allOk, "Encountered: [{0}] paths where AlphaFS != System.IO", errorCnt);
      }

      #endregion // Root

      #endregion // .NET

      #region AlphaFS

      [TestMethod]
      public void __Class_PathInfo()
      {
         Console.WriteLine("PathInfo() Class");

         Dump__Class_PathInfo(true);
         Dump__Class_PathInfo(false);
      }

      #region DirectoryNameWithoutRoot

      [TestMethod]
      public void DirectoryNameWithoutRoot()
      {
         Console.WriteLine("PathInfo.DirectoryNameWithoutRoot");
         Console.WriteLine("\nNo checking against .NET or for valid results; just output.");

         int pathCnt = 0;
         bool allOk = true;
         int errorCnt = 0;

         foreach (string input in InputPaths)
         {
            try
            {
               string actual = new PathInfo(input).DirectoryNameWithoutRoot;
               //string expected = TestContext.DataRow["DirectoryNameWithoutRoot"] as string;
               Console.WriteLine("\n\t#{0:000}\tInput Path: [{1}]\t\tDirectoryNameWithoutRoot: [{2}]", ++pathCnt, input, actual);
               //Assert.AreEqual(expected, actual);
            }
            catch (Exception ex)
            {
               Console.WriteLine("\n\t\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));
               allOk = false;
               errorCnt++;
            }
         }

         Console.WriteLine(Environment.NewLine);

         Assert.AreEqual(true, allOk, "Encountered: [{0}] errors.", errorCnt);
      }

      #endregion // DirectoryNameWithoutRoot

      #region GetLongPath

      [TestMethod]
      public void GetLongPath()
      {
         Console.WriteLine("PathInfo.GetLongPath()");
         Console.WriteLine("\nNo checking against .NET or for valid results; just output.");


         int pathCnt = 0;
         bool allOk = true;
         int errorCnt = 0;

         string originalPath = System.IO.Directory.GetCurrentDirectory();

         foreach (string input in InputPaths)
         {
            try
            {
               System.IO.Directory.SetCurrentDirectory(@"C:\");

               string actual = new PathInfo(input).GetLongPath();
               //string expected = TestContext.DataRow["LongFullPath"] as string;
               Console.WriteLine("\n\t#{0:000}\tInput Path: [{1}]\t\tGetLongPath: [{2}]", ++pathCnt, input, actual);
               //Assert.AreEqual(expected, actual);

               System.IO.Directory.SetCurrentDirectory(originalPath);
            }
            catch (Exception ex)
            {
               Console.WriteLine("\n\t\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));
               allOk = false;
               errorCnt++;
            }
         }

         Console.WriteLine(Environment.NewLine);

         Assert.AreEqual(true, allOk, "Encountered: [{0}] errors.", errorCnt);
      }

      #endregion // GetLongPath

      #region HasFileName

      [TestMethod]
      public void HasFileName()
      {
         Console.WriteLine("PathInfo.HasFileName");
         Console.WriteLine("\nChecked against .NET System.IO.Path.GetFileName() method.");

         int pathCnt = 0;
         bool allOk = true;
         int errorCnt = 0;

         foreach (string input in InputPaths)
         {
            try
            {
               PathInfo pi = new PathInfo(input);
               bool actual = pi.HasFileName;
               string expected = System.IO.Path.GetFileName(input);

               Console.WriteLine("\n\t#{0:000}\tInput Path: [{1}]\t\tHasFileName: [{2}]\t\tSystem.IO: [{3}]", ++pathCnt, input, actual, expected);
               Assert.IsTrue(actual ? !string.IsNullOrEmpty(expected) : string.IsNullOrEmpty(expected));
            }
            catch (Exception ex)
            {
               Console.WriteLine("\n\t\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));
               allOk = false;
               errorCnt++;
            }
         }

         Console.WriteLine(Environment.NewLine);

         Assert.AreEqual(true, allOk, "Encountered: [{0}] errors.", errorCnt);
      }

      #endregion // HasFileName

      #region Parent

      [TestMethod]
      public void Parent()
      {
         Console.WriteLine("PathInfo.Parent");
         Console.WriteLine("\nNo checking against .NET or for valid results; just output.");

         int pathCnt = 0;
         bool allOk = true;
         int errorCnt = 0;

         foreach (string input in InputPaths)
         {
            try
            {
               string actual = new PathInfo(input).Parent.ToString();
               //string expected = TestContext.DataRow["Parent"] as string;
               Console.WriteLine("\n\t#{0:000}\tInput Path: [{1}]\t\tParent: [{2}]", ++pathCnt, input, actual);
               //Assert.AreEqual(expected, actual);
            }
            catch (Exception ex)
            {
               Console.WriteLine("\n\t\tException: [{0}]", ex.Message.Replace(Environment.NewLine, string.Empty));
               allOk = false;
               errorCnt++;
            }
         }

         Console.WriteLine(Environment.NewLine);

         Assert.AreEqual(true, allOk, "Encountered: [{0}] errors.", errorCnt);
      }

      #endregion // Parent

      #endregion // AlphaFS
   }
}
