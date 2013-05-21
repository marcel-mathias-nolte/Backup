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
using System.Security.Principal;
using Alphaleonis.Win32.Filesystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using DriveInfo = Alphaleonis.Win32.Filesystem.DriveInfo;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace AlphaFS.UnitTest
{
   /// <summary>This is a test class for Volume and is intended to contain all Volume Unit Tests.</summary>
   [TestClass]
   public class VolumeTest
   {
      #region VolumeTest Helpers

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

      private static bool IsAdmin()
      {
         bool isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

         if (!isAdmin)
            Console.WriteLine("\n\tThis Unit Test must be run as Administrator.");

         return isAdmin;
      }
      
      private static readonly string TempFolder = Path.GetTempPath();
      
      private string _driveLetter;
      /// <summary>This method returns a random lowercase letter between 'a' and 'z'
      /// http://www.dotnetperls.com/random-lowercase-letter
      /// </summary>
      private static class RandomLetter
      {
         private static readonly Random Random = new Random();
         public static char CurrentDriveLetter = '\0';

         /// <summary>Get a Drive letter that doesn't exist on the Local System.</summary>
         /// <param name="options">When options[0]==true, return existing Drive letter, when found.</param>
         /// <returns>Drive letter as <see cref="char"/></returns>
         public static Char GetNonExistingDriveLetter(params bool[] options)
         {
            bool isBusy = true;

            // Revert method result; get EXISTING Drive letter.
            bool doesExist = options != null && (options.Any() && options[0]);

            char driveLetter = '\0';

            while (isBusy)
            {
               int num = Random.Next(0, 26); // 0 to 25
               driveLetter = (char)('a' + num);
               string drive = string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}", driveLetter, Path.VolumeSeparatorChar, Path.DirectorySeparatorChar);

               DriveInfo di = new DriveInfo(drive);

               if (doesExist)
               {
                  //if (Directory.Exists(drive))
                  if (di.IsVolume && di.IsReady)
                     isBusy = false;
               }
               else
               {
                  //if (!Directory.Exists(drive))
                  if (!di.IsVolume && !di.IsReady)
                     isBusy = false;
               }
            }

            CurrentDriveLetter = driveLetter;
            return driveLetter;
         }
      }

      private static void Dump__Class_VolumeInfo(bool doUnc)
      {
         int cnt = 0;
         foreach (string drive in Directory.GetLogicalDrives())
         {
            string drv = doUnc ? Path.LocalToUnc(drive) : drive;

            StopWatcher(true);
            try
            {
               VolumeInfo volInfo = Volume.GetVolumeInformation(drv);
               Console.WriteLine("\n\t#{0:000} Logical Drive: [{1}]\tVolumeInfo: [{2}]\n{3}", ++cnt, drv, volInfo, Reporter());
               Assert.AreEqual(drv, volInfo.ToString());
               
               Dump(volInfo, -27);
            }
            catch
            {
            }
         }
         Assert.IsTrue(cnt > 0);
      }
      #endregion // VolumeTest Helpers

      [TestMethod]
      public void __Class_VolumeInfo()
      {
         Console.WriteLine("VolumeInfo() Class");
         Console.WriteLine("\nIf your mapped network drives don't show up, take a look at this: http://alphafs.codeplex.com/discussions/397693");

         Dump__Class_VolumeInfo(false);
         Dump__Class_VolumeInfo(true);
      }

      #region DosDevice

      [TestMethod]
      public void DefineDosDevice()
      {
         Console.WriteLine("Volume.DefineDosDevice()");

         if (!IsAdmin())
            Assert.Fail();

         #region Regular Drive Mapping

         _driveLetter = string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}", RandomLetter.GetNonExistingDriveLetter(), Path.VolumeSeparatorChar, Path.DirectorySeparatorChar).ToUpper(CultureInfo.CurrentCulture);
         StopWatcher(true);
         // Create Regular drive mapping.
         bool actionIsTrue = Volume.DefineDosDevice(_driveLetter, TempFolder);
         Console.WriteLine("\n\tCreated Regular drive mapping: [{0}]: {1}\n\tDrive letter: [{2}] now points to: [{3}]\n{4}", TextTrue, actionIsTrue, _driveLetter, TempFolder, Reporter());

         DriveInfo di;

         di = new DriveInfo(_driveLetter);
         Assert.IsTrue(Dump(di, -27));
         // A Regular drive mapping that should be visible immediately in Explorer.
         Assert.IsTrue(actionIsTrue);
         Assert.IsTrue(Directory.Exists(_driveLetter));

         StopWatcher(true);
         // Remove Regular drive mapping.
         actionIsTrue = Volume.DeleteDosDevice(_driveLetter);
         Console.WriteLine("\n\tRemoved Regular drive mapping: [{0}]: {1}\n\tDrive letter: [{2}] has been set free.\n{3}\n", TextTrue, actionIsTrue, _driveLetter, Reporter());
         Assert.IsTrue(actionIsTrue);
         Assert.IsTrue(!Directory.Exists(_driveLetter));

         #endregion // Regular Drive Mapping

         #region Symbolic Link Drive Mapping

         // Create Symbolic Link.
         _driveLetter = string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}", RandomLetter.GetNonExistingDriveLetter(), Path.VolumeSeparatorChar, Path.DirectorySeparatorChar).ToUpper(CultureInfo.CurrentCulture);
         StopWatcher(true);
         actionIsTrue = Volume.DefineDosDevice(_driveLetter, TempFolder, DosDeviceAttributes.RawTargetPath);
         Console.WriteLine("\n\tCreated Symbolic link mapping: [{0}]: {1}\n\tDrive letter: [{2}] now points to: [{3}]\n{4}", TextTrue, actionIsTrue, _driveLetter, TempFolder, Reporter());
         di = new DriveInfo(_driveLetter);
         Assert.IsTrue(Dump(di, -27));
         // The link is created in the NT Device Name namespace and thus not visibile in Explorer.
         Assert.IsTrue(actionIsTrue);
         Assert.IsTrue(!Directory.Exists(_driveLetter));

         // Remove Symbolic Link, no exact match: fail.
         StopWatcher(true);
         actionIsTrue = !Volume.DeleteDosDevice(_driveLetter, "NonExistingFolder", true);
         Console.WriteLine("\n\tRemoved Symbolic link mapping: [{0}]: {1}\n\tDrive letter: [{2}] has NOT been set free.\tNo exactMatch MS-DOS device name found.\n{3}", TextFalse, actionIsTrue, _driveLetter, Reporter());
         // The Symbolic Link has been created, but is not visible in Explorer.
         Assert.IsFalse(!actionIsTrue);
         Assert.IsTrue(!Directory.Exists(_driveLetter));

         // Remove Symbolic Link, exact match: success.
         StopWatcher(true);
         actionIsTrue = Volume.DeleteDosDevice(_driveLetter, TempFolder, true);
         Console.WriteLine("\n\tRemoved Symbolic link mapping: [{0}]: {1}\n\tDrive letter: [{2}] has been set free.\tFound exactMatch MS-DOS device name.\n{3}", TextTrue, actionIsTrue, _driveLetter, Reporter());
         Assert.IsTrue(actionIsTrue);
         Assert.IsTrue(!Directory.Exists(_driveLetter));

         #endregion // Symbolic Link Drive Mapping
      }

      [TestMethod]
      public void DeleteDosDevice()
      {
         Console.WriteLine("Volume.DeleteDosDevice()");

         DefineDosDevice();
      }

      [TestMethod]
      public void QueryAllDosDevices()
      {
         Console.WriteLine("Volume.QueryAllDosDevices()");

         #region QueryAllDosDevices: Sorted

         StopWatcher(true);

         IEnumerable<string> query = Volume.QueryAllDosDevices("sort").ToArray();

         Console.WriteLine("\n\tRetrieved Sorted list: [{0}]\n\tList .Count(): [{1}]\n{2}\n", query.Any(), query.Count(), Reporter());

         int cnt = 0;
         foreach (string dosDev in query)
         {
            Console.WriteLine("\t\t#{0:000} {1}", ++cnt, dosDev);
         }

         Assert.IsTrue(query.Any());
         Assert.IsTrue(cnt > 0);

         #endregion // QueryAllDosDevices: Sorted

         #region QueryAllDosDevices: UnSorted

         Console.Write("\n");

         StopWatcher(true);

         query = Volume.QueryAllDosDevices().ToArray();

         Console.WriteLine("\n\tRetrieved UnSorted list: [{0}]\n\tList .Count(): [{1}]\n{2}\n", query.Any(), query.Count(), Reporter());

         cnt = 0;
         foreach (string dosDev in query)
         {
            Console.WriteLine("\t\t#{0:000} {1}", ++cnt, dosDev);
         }

         Assert.IsTrue(query.Any());
         Assert.IsTrue(cnt > 0);

         #endregion // QueryAllDosDevices: UnSorted
      }

      [TestMethod]
      public void QueryDosDevice()
      {
         Console.WriteLine("Volume.QueryDosDevice()");
         Console.WriteLine("\nIf your mapped network drives don't show up, take a look at this: http://alphafs.codeplex.com/discussions/397693");

         #region Filtered, UnSorted List Drives

         string filter = "hard";

         StopWatcher(true);

         IEnumerable<string> query = Volume.QueryDosDevice(filter).ToArray();

         Console.WriteLine("\n\tQueryDosDevice(\"" + filter + "\")\n\tRetrieved Filtered, UnSorted list: [{0}]\n\tList .Count(): [{1}]\n{2}\n", query.Any(), query.Count(), Reporter());

         int cnt = 0;

         foreach (string dosDev in query)
            Console.WriteLine("\t\t#{0:000} MS-Dos Name: [{1}]", ++cnt, dosDev);

         Assert.IsTrue(query.Any() && cnt > 0);

         #endregion // Filtered, UnSorted List Drives

         #region Filtered, Sorted List Volumes

         filter = "volume";

         cnt = 0;

         StopWatcher(true);

         query = Volume.QueryDosDevice(filter, "sort").ToArray();

         Console.WriteLine("\n\n\tQueryDosDevice(\"" + filter + "\")\n\tRetrieved Filtered, Sorted list: [{0}]\n\tList .Count(): [{1}]\n", query.Any(), query.Count(), Reporter());

         foreach (string dosDev in query)
            Console.WriteLine("\t\t#{0:000} MS-Dos Name: [{1}]", ++cnt, dosDev);

         Assert.IsTrue(query.Any() && cnt > 0);

         #endregion // Filtered, Sorted List Volumes

         #region Get from Logical Drives

         Console.WriteLine("\n\n\tQueryDosDevice (from Logical Drives)\n");

         cnt = 0;

         // Get Logical Drives from Local Host, .IsReady Drives only, strip backslash.
         foreach (string existingDriveLetter in Directory.GetLogicalDrives(false, true, true))
         {
            foreach (string dosDev in Volume.QueryDosDevice(existingDriveLetter))
            {
               bool hasLogicalDrive = !string.IsNullOrEmpty(dosDev);

               Console.WriteLine("\t\t#{0:000} Logical Drive [{1}] MS-Dos Name: [{2}]", ++cnt, existingDriveLetter, dosDev);

               Assert.IsTrue(hasLogicalDrive);
            }
         }

         Assert.IsTrue(cnt > 0, "No entries read.");

         #endregion // Get from Logical Drives
      }

      #endregion // DosDevice

      #region Drive

      [TestMethod]
      public void GetCurrentDriveType()
      {
         Console.WriteLine("Volume.GetCurrentDriveType()");

         GetDriveFormat();
      }

      [TestMethod]
      public void GetDriveFormat()
      {
         Console.WriteLine("Volume.GetDriveFormat()");
         Console.WriteLine("\nIf your mapped network drives don't show up, take a look at this: http://alphafs.codeplex.com/discussions/397693\n");

         int cnt = 0;

         // Get Logical Drives from Environment.
         foreach (string drive in Directory.GetLogicalDrives())
         {
            StopWatcher(true);

            try
            {
               // GetDriveType() can read an empty cdrom drive.
               // SetCurrentDirectory() wil fail on an empty cdrom drive.
               Directory.SetCurrentDirectory(drive);
            }
            catch
            {
               Console.WriteLine("\t\tDirectory.SetCurrentDirectory() failed, GetDriveType() != GetCurrentDriveType()");
            }

            DriveInfo driveInfo = new DriveInfo(drive);
            DriveType driveTypeDrive = driveInfo.DriveType;
            DriveType driveTypeCurrent = Volume.GetCurrentDriveType();

            Console.WriteLine(string.Format(CultureInfo.CurrentCulture,
                                            "\t#{0:000} Logical Drive: [{1}]\tDrive Format: [{2}]\tGetDriveType(): [{3}]\tGetCurrentDriveType(): [{4}]\n{5}\n",
                                            ++cnt, drive, driveInfo.DriveFormat, driveTypeDrive, driveTypeCurrent,
                                            Reporter()));

            if (driveInfo.IsReady && driveInfo.DriveType != DriveType.CDRom)
               Assert.IsTrue(!driveInfo.DriveFormat.Equals(DriveType.Unknown.ToString()), "Encountered drive of type: [Unknown].");

            // GetDriveType() can read an empty cdrom drive.
            // SetCurrentDirectory() wil fail on an empty cdrom drive.
         }

         Assert.IsTrue(cnt > 0);
      }
      
      [TestMethod]
      public void GetDriveType()
      {
         Console.WriteLine("Volume.GetDriveType()");

         GetDriveFormat();
      }

      [TestMethod]
      public void GetDiskFreeSpace()
      {
         Console.WriteLine("Volume.GetDiskFreeSpace()");
         Console.WriteLine("\nIf your mapped network drives don't show up, take a look at this: http://alphafs.codeplex.com/discussions/397693");

         int cnt = 0;

         // Get all Logical Drives, .IsReady Drives only.
         foreach (string drive in Directory.GetLogicalDrives(false, true))
         {
            StopWatcher(true);
            DiskSpaceInfoExtended dsie = Volume.GetDiskFreeSpace(drive);
            Console.WriteLine("\n\t#{0:000} Logical Drive: [{1}]\n{2}", ++cnt, drive, Reporter());
            dsie.Refresh();
            Assert.IsTrue(Dump(dsie, -22));
         }

         Assert.IsTrue(cnt > 0);
      }

      [TestMethod]
      public void GetDiskFreeSpaceClusters()
      {
         Console.WriteLine("Volume.GetDiskFreeSpaceClusters()");
         Console.WriteLine("\nIf your mapped network drives don't show up, take a look at this: http://alphafs.codeplex.com/discussions/397693");

         int cnt = 0;

         // Get all Logical Drives, .IsReady Drives only.
         foreach (string drive in Directory.GetLogicalDrives(false, true))
         {
            StopWatcher(true);
            DiskSpaceInfoExtended dsie = Volume.GetDiskFreeSpaceClusters(drive);
            Console.WriteLine("\n\t#{0:000} Logical Drive: [{1}]\n{2}", ++cnt, drive, Reporter());
            dsie.Refresh();
            Assert.IsTrue(Dump(dsie, -22));
         }

         Assert.IsTrue(cnt > 0);
      }

      [TestMethod]
      public void IsReady()
      {
         Console.WriteLine("Volume.IsReady()");
         Console.WriteLine("\n\tPlease see Directory.GetLogicalDrives() UnitTest.");
      }

      #endregion // Drive

      #region Volume

      #region Label

      [TestMethod]
      public void DeleteCurrentVolumeLabel()
      {
         Console.WriteLine("Volume.DeleteCurrentVolumeLabel()");

         SetCurrentVolumeLabel();
      }

      [TestMethod]
      public void DeleteVolumeLabel()
      {
         Console.WriteLine("Volume.GetVolumeLabel()");

         SetVolumeLabel();
      }

      [TestMethod]
      public void GetVolumeLabel()
      {
         Console.WriteLine("Volume.GetVolumeLabel()");
         Console.WriteLine("\nIf your mapped network drives don't show up, take a look at this: http://alphafs.codeplex.com/discussions/397693");

         int cnt;
         bool looped;
         string label;

         #region Logical Drives

         Console.WriteLine("\n\tLogical Drives\n");

         cnt = 0;
         looped = false;

         // Get Logical Drives from Local host, .IsReady Drives only.
         foreach (string drive in Directory.GetLogicalDrives(false, true))
         {
            StopWatcher(true);

            label = Volume.GetVolumeLabel(drive);

            Console.WriteLine("\t#{0:000} Logical Drive: [{1}]\t\tLabel: [{2}]\n{3}\n", ++cnt, drive, label, Reporter());

            looped = true;
         }

         Assert.IsTrue(looped && cnt > 0);

         #endregion // Logical Drives

         #region Volumes

         Console.WriteLine("\n\tVolumes\n");

         cnt = 0;
         looped = false;

         foreach (string volume in Volume.GetVolumes())
         {
            StopWatcher(true);

            label = Volume.GetVolumeLabel(volume);

            Console.WriteLine("\t#{0:000} Volume: [{1}]\t\tLabel: [{2}]\n{3}\n", ++cnt, volume, label, Reporter());

            looped = true;
         }

         Assert.IsTrue(looped && cnt > 0);

         #endregion // Volumes

         #region DosDevices

         Console.WriteLine("\n\tDosDevices\n");

         cnt = 0;
         looped = false;
         List<string> devices = new List<string>(new[] { "volume", "hard", "physical", "storage" });

         foreach (string dd in devices)
         {
            cnt = 0;

            foreach (string dosDevice in Volume.QueryDosDevice(dd))
            {
               StopWatcher(true);

               // Need LongPathPrefix when querying MS-Dos Namespace.
               label = Volume.GetVolumeLabel(Path.LongPathPrefix + dosDevice);

               Console.WriteLine("\t#{0:000} DosDevice: [{1}]\t\tLabel: [{2}]\n{3}\n", ++cnt, dosDevice, label, Reporter());

               looped = true;
            }
         }

         Assert.IsTrue(looped && cnt > 0);

         #endregion // DosDevices
      }

      [TestMethod]
      public void SetCurrentVolumeLabel()
      {
         Console.WriteLine("Volume.SetCurrentVolumeLabel()");

         SetVolumeLabel();
      }

      [TestMethod]
      public void SetVolumeLabel()
      {
         Console.WriteLine("Volume.SetVolumeLabel()");

         if (!IsAdmin())
            Assert.Fail();

         const string newLabel = "ÂĽpĥæƑŞ ŠëtVőlümèĻāßƩl() Ťest";
         const string template = "\n\tSystem Drive:   [{0}]\t\tCurrent Label: [{1}]\n{2}\n";

         #region Get Label

         StopWatcher(true);
         string originalLabel = Volume.GetVolumeLabel(SysDrive);
         Console.WriteLine(template, SysDrive, originalLabel, Reporter());

         Assert.IsTrue(originalLabel.Equals(Volume.GetVolumeLabel(SysDrive)));

         #endregion // Get Label

         #region Set Label

         StopWatcher(true);

         bool isLabelSet;
         string currentLabel = Volume.GetVolumeLabel(SysDrive);
         try
         {
            isLabelSet = Volume.SetVolumeLabel(SysDrive, newLabel);

            Console.WriteLine("\tLabel Set: [{0}]: {1}", TextTrue, isLabelSet);
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, template, SysDrive, newLabel, Reporter()));

            Assert.IsTrue(isLabelSet);
            Assert.IsTrue(!currentLabel.Equals(newLabel));
         }
         catch (Exception ex)
         {
            Console.WriteLine("\nException: [{0}]\n", ex.Message.Replace(Environment.NewLine, string.Empty));
         }

         #endregion // Set Label

         #region Remove Label

         StopWatcher(true);

         try
         {
            bool isLabelRemoved = Volume.DeleteVolumeLabel(SysDrive);

            currentLabel = Volume.GetVolumeLabel(SysDrive);

            Console.WriteLine("\tLabel Removed: [{0}]: {1}", TextTrue, isLabelRemoved);
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, template, SysDrive, currentLabel, Reporter()));

            Assert.IsTrue(isLabelRemoved);
            Assert.IsTrue(currentLabel.Equals(string.Empty));
         }
         catch (Exception ex)
         {
            Console.WriteLine("\nException: [{0}]\n", ex.Message.Replace(Environment.NewLine, string.Empty));
         }

         #endregion // Remove Label

         #region Restore Label

         StopWatcher(true);

         try
         {
            isLabelSet = Volume.SetVolumeLabel(SysDrive, originalLabel);

            currentLabel = Volume.GetVolumeLabel(SysDrive);

            Console.WriteLine("\tLabel Restored: [{0}]: {1}", TextTrue, isLabelSet);
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, template, SysDrive, currentLabel, Reporter()));

            Assert.IsTrue(isLabelSet && currentLabel.Equals(originalLabel));
         }
         catch (Exception ex)
         {
            Console.WriteLine("\nException: [{0}]\n", ex.Message.Replace(Environment.NewLine, string.Empty));
         }

         #endregion // Restore Label
      }

      #endregion // Label
      
      [TestMethod]
      public void GetVolumeInformation()
      {
         Console.WriteLine("Volume.GetVolumeInformation()");

         __Class_VolumeInfo();
      }

      [TestMethod]
      public void GetDeviceForVolumeName()
      {
         Console.WriteLine("Volume.GetDeviceForVolumeName()");
         Console.WriteLine("\nIf your mapped network drives don't show up, take a look at this: http://alphafs.codeplex.com/discussions/397693");

         #region Logical Drives

         Console.WriteLine("\n\tLogical Drives\n");

         int cnt = 0;
         bool gotData;

         // Get Logical Drives from Local Host.
         foreach (string q in Directory.GetLogicalDrives())
         {
            StopWatcher(true);
            string path = Volume.GetDeviceForVolumeName(q);
            gotData = !string.IsNullOrEmpty(path);
            Console.WriteLine("\t#{0:000} Logical Drive: [{1}]\tGetDeviceForVolumeName(): [{2}]\n{3}\n", ++cnt, q, gotData ? path : "<nodata>", Reporter());
         }

         Assert.IsTrue(cnt > 0);

         #endregion // Logical Drives

         #region Volumes

         Console.WriteLine("\n\tVolumes\n");

         cnt = 0;
         gotData = false;

         foreach (string q in Volume.GetVolumes())
         {
            StopWatcher(true);
            string path = Volume.GetDeviceForVolumeName(q);
            gotData = !string.IsNullOrEmpty(path);
            Console.WriteLine("\t#{0:000} Volume: [{1}]\tGetDeviceForVolumeName(): [{2}]\n{3}\n", ++cnt, q, gotData ? path : "<nodata>", Reporter());
         }
         Assert.IsTrue(gotData);
         Assert.IsTrue(cnt > 0);

         #endregion // Volumes
      }

      [TestMethod]
      public void GetDisplayNameForVolume()
      {
         Console.WriteLine("Volume.GetDisplayNameForVolume()");
         Console.WriteLine("\nShould give the same enumeration as \"mountvol.exe\"\n");

         int cnt = 0;
         foreach (string volume in Volume.GetVolumes())
         {
            StopWatcher(true);
            string displayName = Volume.GetDisplayNameForVolume(volume);
            bool deviceOk = !string.IsNullOrEmpty(displayName);
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture,
                                            "\t#{0:000} Volume: [{1}]\t\tGetDisplayNameForVolume(): [{2}]\n{3}\n", ++cnt,
                                            volume, displayName, Reporter()));
            Assert.IsTrue(deviceOk);
         }
         Assert.IsTrue(cnt > 0);
      }

      [TestMethod]
      public void GetVolumePathNamesForVolume()
      {
         Console.WriteLine("Volume.GetVolumePathNamesForVolume()");
         Console.WriteLine("\nShould give the same enumeration as \"mountvol.exe\"\n");

         int cnt = 0;
         foreach (string volume in Volume.GetVolumes())
         {
            StopWatcher(true);
            foreach (string displayName in Volume.GetVolumePathNamesForVolume(volume))
            {
               bool deviceOk = !string.IsNullOrEmpty(displayName);
               Console.WriteLine(string.Format(CultureInfo.CurrentCulture,
                                               "\t#{0:000} Volume: [{1}]\t\tGetVolumePathNamesForVolume(): [{2}]\n{3}\n",
                                               ++cnt, volume, displayName, Reporter()));
               Assert.IsTrue(deviceOk);
            }
         }
         Assert.IsTrue(cnt > 0);
      }

      [TestMethod]
      public void GetUniqueVolumeNameForPath()
      {
         Console.WriteLine("Volume.GetUniqueVolumeNameForPath()");
         Console.WriteLine("\nIf your mapped network drives don't show up, take a look at this: http://alphafs.codeplex.com/discussions/397693");

         #region Logical Drives

         Console.WriteLine("\n\tLogical Drives\n");

         int cnt = 0;

         // Get Logical Drives from Local Host.
         foreach (string q in Directory.GetLogicalDrives())
         {
            StopWatcher(true);

            string path = Volume.GetUniqueVolumeNameForPath(q);

            bool gotData = !string.IsNullOrEmpty(path);

            Console.WriteLine("\t#{0:000} Logical Drive [{1}]\tGetUniqueVolumeNameForPath(): [{2}]\n{3}\n", ++cnt, q, gotData ? path : "<nodata>", Reporter());
         }

         Assert.IsTrue(cnt > 0);

         #endregion // Logical Drives

         #region Volumes

         Console.WriteLine("\n\tVolumes\n");

         cnt = 0;

         foreach (string q in Volume.GetVolumes())
         {
            StopWatcher(true);

            // true: Strip trailing backslash.
            string path = Volume.GetUniqueVolumeNameForPath(q, true);

            bool gotData = !string.IsNullOrEmpty(path);

            Console.WriteLine("\t#{0:000} Volume: [{1}]: [{2}]\n{3}\n", ++cnt, q, gotData ? path : "", Reporter());

            Assert.IsTrue(gotData);
         }


         Assert.IsTrue(cnt > 0);

         #endregion // Volumes

         #region DosDevices

         Console.WriteLine("\n\tDosDevices\n");

         cnt = 0;

         List<string> devices = new List<string>(new[] { "volume", "hard", "physical", "storage" });

         foreach (string dd in devices)
         {
            cnt = 0;

            foreach (string q in Volume.QueryDosDevice(dd))
            {
               StopWatcher(true);

               // true: Strip trailing backslash.
               string path = Volume.GetUniqueVolumeNameForPath(Path.LongPathPrefix + q, true);

               if (!string.IsNullOrEmpty(path))
               {
                  Console.WriteLine("\t#{0:000} DosDevice: [{1}]: [{2}]\n{3}\n", ++cnt, dd, path, Reporter());
               }
            }
         }

         Assert.IsTrue(cnt > 0);

         #endregion // DosDevices
      }

      [TestMethod]
      public void GetVolumes()
      {
         Console.WriteLine("Volume.GetVolumes()\n");

         GetVolumePathNamesForVolume();
      }

      #region IsSame

      [TestMethod]
      public void IsSame()
      {
         Console.WriteLine("Volume.IsSame()");

         // My otherVolume is mapped to host.

         const string drive = "Z:";
         const string otherVolume = drive + @"\Movies";
         const string host = @"\\192.168.1.101\video\Movies";

         string file1 = Path.GetTempFileName();
         string file2 = Path.GetTempFileName();
         string fileTmp = file2;

         // Same C:
         StopWatcher(true);
         bool isSame = Volume.IsSame(file1, fileTmp);
         Console.WriteLine("\n\tFile1: [{0}]\ton same Volume: [{1}]\n\tFile2: [{2}]\n{3}", file1, isSame, fileTmp, Reporter());
         Assert.IsTrue(isSame);

         // Not same C: -> otherVolume
         fileTmp = file2.Replace(SysDrive, otherVolume);
         StopWatcher(true);
         isSame = Volume.IsSame(file1, fileTmp);
         Console.WriteLine("\n\tFile1: [{0}]\ton same Volume: [{1}]\n\tFile2: [{2}]\n{3}", file1, isSame, fileTmp, Reporter());
         Assert.IsFalse(isSame);

         // Same C: -> C$
         fileTmp = Path.LocalToUnc(file2);
         StopWatcher(true);
         isSame = Volume.IsSame(file1, fileTmp);
         Console.WriteLine("\n\tFile1: [{0}]\ton same Volume: [{1}]\n\tFile2: [{2}]\n{3}", file1, isSame, fileTmp, Reporter());
         Assert.IsTrue(isSame);

         // Same Z: -> \\192.168.1.101
         fileTmp = host;
         StopWatcher(true);
         isSame = Volume.IsSame(otherVolume, fileTmp);
         Console.WriteLine("\n\tFile1: [{0}]\ton same Volume: [{1}]\n\tFile2: [{2}]\n{3}", otherVolume, isSame, fileTmp, Reporter());
         Assert.IsTrue(isSame);

         // Not same C: -> Z:
         fileTmp = otherVolume.Replace(drive, SysDrive);
         StopWatcher(true);
         isSame = Volume.IsSame(otherVolume, fileTmp);
         Console.WriteLine("\n\tFile1: [{0}]\ton same Volume: [{1}]\n\tFile2: [{2}]\n{3}", otherVolume, isSame, fileTmp, Reporter());
         Assert.IsFalse(isSame);
      }

      #endregion // IsSame

      [TestMethod]
      public void IsVolume()
      {
         Console.WriteLine("Volume.IsVolume()");
         Console.WriteLine("\nIf your mapped network drives don't show up, take a look at this: http://alphafs.codeplex.com/discussions/397693");

         #region Logical Drives

         Console.WriteLine("\n\tLogical Drives\n");

         int cnt = 0;
         bool isVolume;

         // Get Logical Drives from Local Host.
         foreach (string drive in Directory.GetLogicalDrives())
         {
            StopWatcher(true);
            isVolume = Volume.IsVolume(drive);
            Console.WriteLine("\t\t#{0:000} Logical Drive [{1}] == [IsVolume]: [{2}]\n{3}\n", ++cnt, drive, isVolume, Reporter());
         }

         Assert.IsTrue(cnt > 0);

         #endregion // Logical Drives

         #region DosDevices

         Console.WriteLine("\n\tDosDevices\n");

         cnt = 0;

         List<string> devices = new List<string>(new[] { "volume", "hard", "physical", "storage" });

         foreach (string dd in devices)
         {
            cnt = 1;

            foreach (string dosDev in Volume.QueryDosDevice(dd))
            {
               StopWatcher(true);
               isVolume = Volume.IsVolume(Path.LongPathPrefix + dosDev);
               Console.WriteLine("\t\t#{0:000} DosDevice: [{1}*] == [IsVolume]: [{2}]\t[{3}]\n\t{4}\n", ++cnt, dd, isVolume, dosDev, Reporter());
            }
         }

         Assert.IsTrue(cnt > 1);

         #endregion // DosDevices
      }

      #region Volume Mount Point

      [TestMethod]
      public void DeleteVolumeMountPoint()
      {
         Console.WriteLine("Volume.DeleteVolumeMountPoint()");
         SetVolumeMountPoint();
      }

      [TestMethod]
      public void GetVolumeMountPoints()
      {
         Console.WriteLine("Volume.GetVolumeMountPoints()");
         Console.WriteLine("\nIf your mapped network drives don't show up, take a look at this: http://alphafs.codeplex.com/discussions/397693");

         #region Logical Drives

         int cnt = 0;
         Console.WriteLine("\n\tLogical Drives\n");

         // Get Logical Drives from Local Host, .IsReady Drives only.
         foreach (string q in Directory.GetLogicalDrives(false, true))
         {
            // Logical Drives --> Volumes --> Volume Mount Points.
            string guid = Volume.GetUniqueVolumeNameForPath(q);
            if (!string.IsNullOrEmpty(guid))
            {
               foreach (string q2 in Volume.GetVolumeMountPoints(guid).Where(mp => !string.IsNullOrEmpty(mp)))
               {
                  StopWatcher(true);

                  Console.WriteLine(string.Format(CultureInfo.CurrentCulture,
                                                  "\t#{0:000} Logical Drive: [{1}] = GUID: [{2}]\n\t\tMount Point: [{3}]\n{4}\n",
                                                  ++cnt, q, guid, q2, Reporter()));
               }
            }
         }

         // No Assert(); there might be no Mount Points.
         if (cnt == 0)
            Console.WriteLine("No Volume Mount Points found.");

         #endregion // Logical Drives
      }

      [TestMethod]
      public void GetUniqueVolumeNameForVolumeMountPoint()
      {
         Console.WriteLine("Volume.GetUniqueVolumeNameForVolumeMountPoint()");
         Console.WriteLine("\nIf your mapped network drives don't show up, take a look at this: http://alphafs.codeplex.com/discussions/397693");

         if (!IsAdmin())
            Assert.Fail();

         #region Logical Drives

         int cnt = 0;
         Console.WriteLine("\n\tLogical Drives\n");

         // Get Logical Drives from Local Host, .IsReady Drives only.
         foreach (string q in Directory.GetLogicalDrives(false, true))
         {
            // Logical Drives --> Volumes --> Volume Mount Points.
            string guid = Volume.GetUniqueVolumeNameForPath(q);
            if (!string.IsNullOrEmpty(guid))
            {
               try
               {
                  foreach (string q2 in Volume.GetVolumeMountPoints(guid).Where(mp => !string.IsNullOrEmpty(mp)))
                  {
                     StopWatcher(true);

                     string q3 = Volume.GetUniqueVolumeNameForVolumeMountPoint(Path.Combine(q, q2));

                     Console.WriteLine(
                        "\t#{0:000} Logical Drive: [{1}] = Mount Point: [{2}]\n\t     GUID: [{3}]\n{4}\n", ++cnt, q, q2,
                        q3, Reporter());
                  }
               }
               catch (Exception ex)
               {
                  Console.WriteLine("\nException: [{0}]\n", ex.Message.Replace(Environment.NewLine, string.Empty));
               }
            }
         }

         // No Assert(); there might be no Mount Points.
         if (cnt == 0)
            Console.WriteLine("No Volume Mount Points found.");

         #endregion // Logical Drives
      }

      [TestMethod]
      public void SetVolumeMountPoint()
      {
         Console.WriteLine("Volume.SetVolumeMountPoint()\n");

         if (!IsAdmin())
            Assert.Fail();

         #region Logical Drives

         int cnt = 0;
         string destFolder = Path.Combine(TempFolder, Path.GetRandomFileName());
         Directory.CreateDirectory(destFolder);

         string guid = Volume.GetUniqueVolumeNameForPath(SysDrive);
         if (!string.IsNullOrEmpty(guid))
         {
            try
            {
               StopWatcher(true);
               bool setOk = Volume.SetVolumeMountPoint(destFolder, guid);
               Console.WriteLine(string.Format(CultureInfo.CurrentCulture,
                                               "\t#{0:000} System Drive: [{1}] = GUID: [{2}]\n\t Created Mount Point: [{3}]\n{4}\n",
                                               ++cnt, SysDrive, guid, setOk, Reporter()));
               Assert.IsTrue(setOk, "Failed creating VolumeMountPoint: [{0}] [{1}]", destFolder, guid);

               GetVolumeMountPoints();

               Console.WriteLine("File.GetLinkTargetInfo()");

               LinkTargetInfo lti = File.GetLinkTargetInfo(destFolder);
               Assert.IsTrue(!string.IsNullOrEmpty(lti.PrintName));
               Assert.IsTrue(!string.IsNullOrEmpty(lti.SubstituteName));
               Assert.IsTrue(Dump(lti, -14), "Unable to dump object.");

               // Cleanup.
               StopWatcher(true);
               bool deleteOk = Volume.DeleteVolumeMountPoint(destFolder);
               Console.WriteLine(string.Format(CultureInfo.CurrentCulture,
                                               "\nVolume.DeleteVolumeMountPoint() == [{0}]: [{1}]\tFolder: [{2}]\n{3}\n",
                                               TextTrue, deleteOk, destFolder, Reporter()));

               Assert.IsTrue(Directory.Delete(destFolder, true, true));
               Assert.IsTrue(deleteOk && !Directory.Exists(destFolder));

               GetVolumeMountPoints();
            }
            catch (Exception ex)
            {
               Console.WriteLine("\nException: [{0}]\n", ex.Message.Replace(Environment.NewLine, string.Empty));
            }
         }

         Assert.IsTrue(cnt > 0);

         #endregion // Logical Drives
      }

      #endregion // Volume Mount Point

      #endregion // Volume
   }
}