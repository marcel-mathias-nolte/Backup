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
using System.Security.Principal;
using Alphaleonis.Win32.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlphaFS.UnitTest
{
   /// <summary>This is a test class for Host and is intended to contain all Host Unit Tests.</summary>
   [TestClass]
   public class HostTest
   {
      #region HostTest Helpers

      private const string LocalHost = null; //"192.168.1.201";

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

      #endregion // FileTest Helpers

      #region AlphaFS

      #region EnumerateDisks

      [TestMethod]
      public void EnumerateDisks()
      {
         Console.WriteLine("Host.EnumerateDisks()");

         if (!IsAdmin())
            Assert.Fail();
         
         string host = LocalHost;
         Console.Write("\n\tEnumerating disks from host: [{0}]\n", host);
         int cnt = 0;
         StopWatcher(true);
         foreach (string disk in Host.EnumerateDisks(host))
         {
            Console.Write("\n\t#{0:000} Disk: [{1}]", ++cnt, disk);
            //Dump(disk, -21);
         }
         Console.Write("\n{0}", Reporter());


         host = "NonExistingHost";
         bool caughtException = false;
         Console.Write("\n\n\tEnumerating disks from host: [{0}]\n", host);
         StopWatcher(true);
         try
         {
            Host.EnumerateDisks(host).Any();
         }
         catch (Exception ex)
         {
            caughtException = true;
            Console.Write("\n\t\tCaught Exception: [{0}]", ex.Message);
         }
         Console.Write(Reporter());
         Assert.IsTrue(caughtException);
      }

      #endregion // EnumerateDisks

      #region EnumerateShares

      [TestMethod]
      public void EnumerateShares()
      {
         Console.WriteLine("Host.EnumerateShares()");

         string host = LocalHost;
         Console.Write("\n\tEnumerating shares from host: [{0}]\n", host);
         int cnt = 0;
         StopWatcher(true);
         foreach (ShareInfo share in Host.EnumerateShares(host))
         {
            Console.Write("\n\t#{0:000} Share: [{1}]", ++cnt, share);
            Dump(share, -18);
         }
         Console.Write("\n{0}", Reporter());


         host = "NonExistingHost";
         bool caughtException = false;
         Console.Write("\n\n\n\tEnumerating shares from host: [{0}]\n", host);
         StopWatcher(true);
         try
         {
            Host.EnumerateShares(host).Any();
         }
         catch (Exception ex)
         {
            caughtException = true;
            Console.Write("\n\t\tCaught Exception: [{0}]", ex.Message);
         }
         Console.Write(Reporter());
         Assert.IsTrue(caughtException);
      }

      #endregion // EnumerateShares

      #region GetShareLocalPath

      [TestMethod]
      public void GetShareLocalPath()
      {
         Console.WriteLine("Path.GetShareLocalPath()");

         string host = LocalHost;
         Console.Write("\n\tEnumerating shares from host: [{0}]\n\n", host);
         int cnt = 0;
         StopWatcher(true);
         foreach (ShareInfo share in Host.EnumerateShares(host))
         {
            string shareLocalPath = Host.GetShareLocalPath(share.ToString());

            Console.WriteLine("\t#{0:000} Share: [{1}]\t\tHost Localpath: [{2}]", ++cnt, share, shareLocalPath);

            // GetShareLocalPath() only works correctly for shares defined on the local host.
            if (share.IsLocal)
               Assert.AreEqual(share.Path, shareLocalPath);
         }
         Console.WriteLine("\n{0}", Reporter());
      }

      #endregion // GetShareLocalPath

      #endregion // AlphaFS
   }
}