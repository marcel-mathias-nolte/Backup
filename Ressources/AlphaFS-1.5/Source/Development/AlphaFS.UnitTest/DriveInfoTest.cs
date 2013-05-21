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
using DriveInfo = Alphaleonis.Win32.Filesystem.DriveInfo;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace AlphaFS.UnitTest
{
   /// <summary>This is a test class for DriveInfo and is intended to contain all DriveInfo Unit Tests.</summary>
   [TestClass]
   public class DriveInfoTest
   {
      #region DriveInfoTest Helpers

      private static readonly string StartupFolder = AppDomain.CurrentDomain.BaseDirectory;
      private static readonly string SysDrive = Environment.GetEnvironmentVariable("SystemDrive");
      private static readonly string SysRoot = Environment.GetEnvironmentVariable("SystemRoot");

      private const string SpecificX3 = "Windows XP and Windows Server 2003 specific.";
      private const string TextTrue = "IsTrue";
      private const string TextFalse = "IsFalse";
      private const string TenNumbers = "0123456789";
      private const string TextHelloWorld = "Hëllõ Wørld!";
      private const string TextGoodBye = "GóödByé Wôrld!";
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

      private static void Dump__Class_DriveInfo(bool compare, bool isLocal, string path)
      {
         Console.WriteLine("\n\t{0}", isLocal ? "Local" : "Network");
         if (!isLocal)
            path = Path.LocalToUnc(path);

         StopWatcher(true);
         DriveInfo actual = new DriveInfo(path);
         Console.WriteLine("\n\tInput Path: [{0}]\t\tLogical Drive: [{1}]\n{2}", path, actual.Name, Reporter());

         if (compare && isLocal)
         {
            System.IO.DriveInfo expected = new System.IO.DriveInfo(path);
            Assert.IsTrue(Dump(expected, -18));

            // Even 1 byte more or less results in failure, so do these tests asap.
            Assert.AreEqual(expected.AvailableFreeSpace, actual.AvailableFreeSpace);
            Assert.AreEqual(expected.TotalFreeSpace, actual.TotalFreeSpace);
            Assert.AreEqual(expected.TotalSize, actual.TotalSize);

            Assert.AreEqual(expected.DriveFormat, actual.DriveFormat);
            Assert.AreEqual(expected.DriveType, actual.DriveType);
            Assert.AreEqual(expected.IsReady, actual.IsReady);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.RootDirectory.ToString(), actual.RootDirectory.ToString());
            Assert.AreEqual(expected.VolumeLabel, actual.VolumeLabel);

            #region Class Equality

            //int getHashCode1 = actual.GetHashCode();

            DriveInfo driveInfo2 = new DriveInfo(path);
            //int getHashCode2 = driveInfo2.GetHashCode();

            DriveInfo clone = actual;
            //int getHashCode3 = clone.GetHashCode();

            bool isTrue1 = clone.Equals(actual);
            bool isTrue2 = clone == actual;
            bool isTrue3 = !(clone != actual);
            bool isTrue4 = actual == driveInfo2;
            bool isTrue5 = !(actual != driveInfo2);

            //Console.WriteLine("\n\t\t actual.GetHashCode() : [{0}]", getHashCode1);
            //Console.WriteLine("\t\t     clone.GetHashCode() : [{0}]", getHashCode3);
            //Console.WriteLine("\t\tdriveInfo2.GetHashCode() : [{0}]\n", getHashCode2);

            //Console.WriteLine("\t\t obj clone.ToString() == [{0}]", clone.ToString());
            //Console.WriteLine("\t\t obj clone.Equals()   == [{0}] : {1}", TextTrue, isTrue1);
            //Console.WriteLine("\t\t obj clone ==         == [{0}] : {1}", TextTrue, isTrue2);
            //Console.WriteLine("\t\t obj clone !=         == [{0}]: {1}\n", TextFalse, isTrue3);

            //Console.WriteLine("\t\tdriveInfo == driveInfo2 == [{0}] : {1}", TextTrue, isTrue4);
            //Console.WriteLine("\t\tdriveInfo != driveInfo2 == [{0}] : {1}", TextFalse, isTrue5);

            Assert.IsTrue(isTrue1, "clone.Equals(actual)");
            Assert.IsTrue(isTrue2, "clone == actual");
            Assert.IsTrue(isTrue3, "!(clone != actual)");
            Assert.IsTrue(isTrue4, "actual == driveInfo2");
            Assert.IsTrue(isTrue5, "!(actual != driveInfo2)");

            #endregion // Class Equality
         }

         StopWatcher(true);

         Assert.IsTrue(Dump(actual, -24));
         
         // DiskSpaceInfoExtended struct.
         Console.WriteLine("\n\n\t\tProperty DiskSpaceInfoExtended:");
         Assert.IsTrue(Dump(actual.DiskSpaceInfoExtended, -27));

         // DirectoryInfo class.
         Console.WriteLine("\n\n\t\tProperty RootDirectory:");
         Assert.IsTrue(Dump(actual.RootDirectory, -19));

         // VolumeInfo class.
         Console.WriteLine("\n\n\t\tProperty VolumeInfo:");
         
         // Will fail on, for example, a TrueCrypt volume.
         //Assert.IsTrue(Dump(actual.VolumeInfo, -27));
         Dump(actual.VolumeInfo, -27);
         
         Console.WriteLine(Reporter());
      }

      #endregion // DriveInfoTest Helpers

      [TestMethod]
      public void __Class_DriveInfo()
      {
         Console.WriteLine("DriveInfo() Class");

         Dump__Class_DriveInfo(true, true, SysRoot);
         Dump__Class_DriveInfo(false, false, SysRoot);
      }

      [TestMethod]
      public void GetDrives()
      {
         Console.WriteLine("DriveInfo.GetDrives()");
         Console.WriteLine("\nIf your mapped network drives don't show up, take a look at this: http://alphafs.codeplex.com/discussions/397693");

         //Console.WriteLine("\n\t{0}", new Win32Exception());
         foreach (DriveInfo actual in DriveInfo.GetDrives())
         {
            //Console.WriteLine("\n\t#{0:000} Logical Drive: [{1}]", ++cnt, actual.Name);

            if (actual.IsReady && actual.DriveType != DriveType.CDRom)
            {
               // GetFreeSpaceEx()
               Assert.IsTrue(actual.AvailableFreeSpace > 0 && actual.TotalSize > 0 && actual.TotalFreeSpace > 0);

               // GetFreeSpace()
               Assert.IsTrue(actual.DiskSpaceInfoExtended.SectorsPerCluster > 0 && actual.DiskSpaceInfoExtended.BytesPerSector > 0 && actual.DiskSpaceInfoExtended.TotalNumberOfClusters > 0);

               // DriveInfo()
               Assert.IsTrue(actual.DiskSpaceInfoExtended.ClusterSize > 0 &&
                             !String.IsNullOrEmpty(actual.DiskSpaceInfoExtended.TotalSizeUnitSize) &&
                             !String.IsNullOrEmpty(actual.DiskSpaceInfoExtended.UsedSpaceUnitSize) &&
                             !String.IsNullOrEmpty(actual.DiskSpaceInfoExtended.AvailableFreeSpaceUnitSize));

               //Console.WriteLine("\n\tDriveInfo() object details:");
               Dump__Class_DriveInfo(false, true, actual.Name);

               // Also dump Network variant.
               Dump__Class_DriveInfo(false, false, Path.LocalToUnc(actual.Name));
            }

            StopWatcher(true);
         }
      }
   }
}