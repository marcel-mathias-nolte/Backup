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
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace AlphaFS.UnitTest
{
   /// <summary>This is a test class for BackupFileStream and is intended to contain all BackupFileStream UnitTests.</summary>
   [TestClass]
   public class BackupFileStreamTest
   {
      #region BackupFileStreamTest Helpers

      private const string TextTrue = "IsTrue";

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
            Console.WriteLine("{0}", nulll);
            return false;
         }

         Console.WriteLine("\n\t\tPointer object to: \"{0}\"\n", obj.GetType().FullName);

         bool loopOk = false;
         foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj).Sort().Cast<PropertyDescriptor>().Where(descriptor => descriptor != null))
         {
            string propValue = null;
            try
            {
               object value = descriptor.GetValue(obj);
               if (value != null) propValue = value.ToString();

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

      #endregion // BackupFileStreamTest Helpers

      [TestMethod]
      public void __Class_BackupFileStream()
      {
         Console.WriteLine("BackupFileStream() Class");

         string path = Path.Combine(Path.GetTempPath(), "BackupFileStream() Class-" + Path.GetRandomFileName());

         Console.WriteLine("\n\tFile: \"{0}\"", path);

         StopWatcher(true);
         using (BackupFileStream bfs = new BackupFileStream(path, FileMode.Create))
         {
            Console.WriteLine("{0}", Reporter());

            Assert.IsTrue(Dump(bfs, -10));
         }

         bool fileNotExists = File.Delete(path);
         Console.WriteLine("\n\t(Deleted tempfile == \"{0}\": {1})", TextTrue, fileNotExists);
         Assert.IsTrue(fileNotExists);
      }

      [TestMethod]
      public void ReadStreamInfo()
      {
         Console.WriteLine("BackupFileStream.ReadStreamInfo() Class");

         string path = Path.Combine(Path.GetTempPath(), "BackupFileStream.ReadStreamInfo()-" + Path.GetRandomFileName());

         Console.WriteLine("\n\tFile: \"{0}\"", path);

         StopWatcher(true);
         using (BackupFileStream bfs = new BackupFileStream(path, FileMode.Create))
         {
            Console.WriteLine(Reporter());

            Assert.IsTrue(Dump(bfs.ReadStreamInfo(), -11));
         }

         bool fileNotExists = File.Delete(path);
         Console.WriteLine("\n\t(Deleted tempfile == \"{0}\": {1})", TextTrue, fileNotExists);
         Assert.IsTrue(fileNotExists);
      }
   }
}