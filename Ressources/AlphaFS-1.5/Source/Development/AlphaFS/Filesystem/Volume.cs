/* Copyright (c) 2008-2012 Peter Palotas, Alexandr Normuradov
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Alphaleonis.Win32.Filesystem
{
   /// <summary>Static class providing utility methods for working with Microsoft Windows devices and volumes.</summary>
   public static class Volume
   {
      #region DosDevice

      #region DefineDosDevice

      /// <summary>Defines, redefines, or deletes MS-DOS device names.</summary>
      /// <param name="deviceName">A pointer to an MS-DOS device name string specifying the device the function is defining, redefining, or deleting.</param>
      /// <param name="targetPath">A pointer to a path string that will implement this device. The string is an MS-DOS path string unless the <see cref="DosDeviceAttributes.RawTargetPath"/> flag is specified, in which case this string is a path string.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool DefineDosDevice(string deviceName, string targetPath)
      {
         return DefineDosDevice(deviceName, targetPath, DosDeviceAttributes.None);
      }

      /// <summary>Defines, redefines, or deletes MS-DOS device names.</summary>
      /// <param name="deviceName">A pointer to an MS-DOS device name string specifying the device the function is defining, redefining, or deleting.</param>
      /// <param name="targetPath">A pointer to a path string that will implement this device. The string is an MS-DOS path string unless the <see cref="DosDeviceAttributes.RawTargetPath"/> flag is specified, in which case this string is a path string.</param>
      /// <param name="deviceAttributes">The controllable aspects of the DefineDosDevice function <see cref="DosDeviceAttributes"/>flags which will be combined with the default.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SuppressMessage("Microsoft.Interoperability", "CA1404:CallGetLastErrorImmediatelyAfterPInvoke")]
      [SecurityCritical]
      public static bool DefineDosDevice(string deviceName, string targetPath, DosDeviceAttributes deviceAttributes)
      {
         if (string.IsNullOrEmpty(deviceName))
            throw new ArgumentNullException("deviceName");

         // targetPath is allowed to be null.

         deviceName = Path.GetRegularPath(deviceName);
         deviceName = Path.DirectorySeparatorRemove(deviceName, false);

         // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
         // Minimize method calls from here.
         using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
            if (!NativeMethods.DefineDosDevice(deviceAttributes, deviceName, targetPath))
               NativeError.ThrowException(deviceName, targetPath);

         return true;
      }

      #endregion // DefineDosDevice

      #region DeleteDosDevice

      /// <summary>Deletes an MS-DOS device name.</summary>
      /// <param name="deviceName">An MS-DOS device name string specifying the device to delete.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool DeleteDosDevice(string deviceName)
      {
         return DeleteDosDevice(deviceName, null, false, DosDeviceAttributes.RemoveDefinition);
      }

      /// <summary>Deletes an MS-DOS device name.</summary>
      /// <param name="deviceName">An MS-DOS device name string specifying the device to delete.</param>
      /// <param name="targetPath">A pointer to a path string that will implement this device.
      ///  The string is an MS-DOS path string unless the <see cref="DosDeviceAttributes.RawTargetPath"/> flag is specified, in which case this string is a path string.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool DeleteDosDevice(string deviceName, string targetPath)
      {
         return DeleteDosDevice(deviceName, targetPath, false, DosDeviceAttributes.RemoveDefinition);
      }

      /// <summary>Deletes an MS-DOS device name.</summary>
      /// <param name="deviceName">An MS-DOS device name string specifying the device to delete.</param>
      /// <param name="targetPath">A pointer to a path string that will implement this device.
      /// The string is an MS-DOS path string unless the <see cref="DosDeviceAttributes.RawTargetPath"/> flag is specified, in which case this string is a path string.</param>
      /// <param name="exactMatch">Only delete MS-DOS device on an exact name match. If exactMatch is true, targetPath must be the same path used to create the mapping.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool DeleteDosDevice(string deviceName, string targetPath, bool exactMatch)
      {
         return DeleteDosDevice(deviceName, targetPath, exactMatch, DosDeviceAttributes.RemoveDefinition);
      }

      /// <summary>Deletes an MS-DOS device name.</summary>
      /// <param name="deviceName">An MS-DOS device name string specifying the device to delete.</param>
      /// <param name="targetPath">A pointer to a path string that will implement this device.
      /// The string is an MS-DOS path string unless the <see cref="DosDeviceAttributes.RawTargetPath"/> flag is specified, in which case this string is a path string.</param>
      /// <param name="exactMatch">Only delete MS-DOS device on an exact name match. If exactMatch is true, targetPath must be the same path used to create the mapping.</param>
      /// <param name="deviceAttributes">The controllable aspects of the DefineDosDevice function <see cref="DosDeviceAttributes"/> flags which will be combined with the default.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
      [SecurityCritical]
      public static bool DeleteDosDevice(string deviceName, string targetPath, bool exactMatch, DosDeviceAttributes deviceAttributes)
      {
         // A pointer to a path string that will implement this device.
         // The string is an MS-DOS path string unless the DDD_RAW_TARGET_PATH flag is specified, in which case this string is a path string.

         if (exactMatch && !string.IsNullOrEmpty(targetPath))
            deviceAttributes = deviceAttributes | DosDeviceAttributes.ExactMatchOnRemove | DosDeviceAttributes.RawTargetPath;

         // Remove the MS-DOS device name. First, get the name of the Windows NT device
         // from the symbolic link and then delete the symbolic link from the namespace.

         try
         {
            return DefineDosDevice(deviceName, targetPath, deviceAttributes);
         }
         catch (Exception)
         {
            return false;
         }
      }

      #endregion // DeleteDosDevice

      #region QueryAllDosDevices

      /// <summary>Retrieves a list of all existing MS-DOS device names.</summary>
      /// <returns>An IEnumerable list of Strings of one or more existing MS-DOS device names.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static IEnumerable<string> QueryAllDosDevices()
      {
         return QueryDosDevice(null, null);
      }

      /// <summary>Retrieves a list of all existing MS-DOS device names.</summary>
      /// <param name="deviceName">
      /// (Optional, default: null) An MS-DOS device name string specifying the target of the query.
      /// This parameter can be "sort". In that case a sorted list of all existing MS-DOS device names is returned.
      /// This parameter can be null. In that case, the <see cref="QueryDosDevice"/> function will store a list of all
      /// existing MS-DOS device names into the buffer.
      /// </param>
      /// <returns>An IEnumerable list of Strings of one or more existing MS-DOS device names.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static IEnumerable<string> QueryAllDosDevices(string deviceName)
      {
         return QueryDosDevice(null, deviceName);
      }

      #endregion // QueryAllDosDevices

      #region QueryDosDevice

      /// <summary>Retrieves information about MS-DOS device names. The function can obtain the current mapping for a
      /// particular MS-DOS device name. The function can also obtain a list of all existing MS-DOS device names.
      /// </summary>
      /// <param name="deviceName">
      /// An MS-DOS device name string specifying the target of the query.
      /// This parameter can be null. In that case, the QueryDosDevice function will store a list of all
      /// existing MS-DOS device names into the buffer.
      /// </param>
      /// <param name="options">(Optional, default: "false") If options[0] = "true", a sorted list will be returned.</param>
      /// <returns>An <see cref="IEnumerable{String}"/> object with one or more existing MS-DOS device names.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static IEnumerable<string> QueryDosDevice(string deviceName, params string[] options)
      {
         deviceName = Path.DirectorySeparatorRemove(deviceName, false);
         bool searchFilter = false;

         // Only process options if a device is supplied.
         if (deviceName != null)
         {
            // Check that at least one "options[]" has something to say. If so, rebuild them.
            options = options != null && options.Any() ? new[] { deviceName, options[0] } : new[] { deviceName, string.Empty };

            searchFilter = !Path.IsLogicalDrive(deviceName);

            if (searchFilter)
               deviceName = null;
         }

         // Choose sorted output.
         bool doSort = options != null &&
                       options.Any(s => s != null && s.Equals("sort", StringComparison.OrdinalIgnoreCase));

         // Start with a larger buffer when using a searchFilter.
         uint bufferSize = (uint)(searchFilter || doSort || (deviceName == null && options == null) ? 16384 : 256);
         uint bufferResult = 0;

         while (bufferResult == 0)
         {
            char[] buffer = new char[bufferSize];

            // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
            // Minimize method calls from here.
            using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
               bufferResult = NativeMethods.QueryDosDevice(deviceName, buffer, bufferSize);

            if (bufferResult > 0)
            {
               List<string> dosDev = new List<string>();
               StringBuilder sb = new StringBuilder();

               for (int i = 0; i < bufferResult; i++)
               {
                  if (buffer[i] != Path.StringTerminatorChar)
                     sb.Append(buffer[i]);
                  else if (sb.Length > 0)
                  {
                     dosDev.Add(sb.ToString());
                     sb.Length = 0;
                  }
               }

               // Choose the yield back query; filtered or list.
               IEnumerable<string> selectQuery = (searchFilter)
                                                    ? dosDev.Where(dev => options != null && dev.StartsWith(options[0], StringComparison.OrdinalIgnoreCase))
                                                    : dosDev;

               foreach (string dev in (doSort) ? selectQuery.OrderBy(n => n) : selectQuery)
                  yield return dev;
            }
            else
            {
               int lastError = Marshal.GetLastWin32Error();
               if (lastError == Win32Errors.ERROR_MORE_DATA || lastError == Win32Errors.ERROR_INSUFFICIENT_BUFFER)
                  bufferSize *= 2;
               else
                  NativeError.ThrowException(lastError, deviceName);
            }
         }
      }

      #endregion // QueryDosDevice

      #endregion // DosDevice

      #region Drive

      #region GetCurrentDriveType

      /// <summary>Determines, based on the root of the current directory, whether a disk drive is a removable, fixed, CD-ROM, RAM disk, or network drive.</summary>
      /// <returns>A <see cref="DriveType"/> object.</returns>
      [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      [SecurityCritical]
      public static DriveType GetCurrentDriveType()
      {
         return GetDriveType(null);
      }

      #endregion // GetCurrentDriveType

      #region GetDriveFormat

      /// <summary>Gets the name of the file system, such as NTFS or FAT32.</summary>
      /// <param name="rootPathName">The root directory for the drive.</param>
      /// <returns>The name of the file system on the specified drive or <see cref="string.Empty"/> on failure or if not available.</returns>
      /// <remarks>Use DriveFormat to determine what formatting a drive uses.</remarks>
      [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
      [SecurityCritical]
      public static string GetDriveFormat(string rootPathName)
      {
         try
         {
            return GetVolumeInformation(rootPathName).FileSystemName;
         }
         catch (Exception)
         {
            return string.Empty;
         }
      }

      #endregion // GetDriveFormat

      #region GetDriveType

      /// <summary>Determines whether a disk drive is a removable, fixed, CD-ROM, RAM disk, or network drive.</summary>
      /// <param name="rootPathName">The root directory for the drive. If this parameter is null, the function uses the root of the current directory.</param>
      /// <returns>A <see cref="DriveType"/> object.</returns>
      [SecurityCritical]
      public static DriveType GetDriveType(string rootPathName)
      {
         rootPathName = Path.DirectorySeparatorAdd(rootPathName, false);

         // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
         // Minimize method calls from here.
         using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
            return NativeMethods.GetDriveType(rootPathName);
      }

      #endregion // GetDriveType

      #region GetDiskFreeSpace

      /// <summary>Retrieves information about the amount of space that is available on a disk volume, which is the total amount of space, the total amount of free space, and the total amount of free space available to the user that is associated with the calling thread.</summary>
      /// <param name="drivePath">The root directory of the disk for which information is to be returned.</param>
      /// <returns>A <see ref="DiskSpaceInfoExtended"/> structure object containing the requested information or null if any of the depended methods fails.</returns>
      /// <remarks>The calling application must have FILE_LIST_DIRECTORY access rights for this directory.</remarks>
      [SecurityCritical]
      public static DiskSpaceInfoExtended GetDiskFreeSpace(string drivePath)
      {
         return new DiskSpaceInfoExtended(drivePath, false);
      }

      #endregion // GetDiskFreeSpace

      #region GetDiskFreeSpaceClusters

      /// <summary>Retrieves information about the specified disk, including the amount of free space on the disk.</summary>
      /// <param name="drivePath">The root directory of the disk for which information is to be returned.
      /// Furthermore, a drive specification must have a trailing backslash (for example, "C:\").</param>
      /// <returns>A <see ref="DiskSpaceInfoExtended"/> structure object containing the requested information or null if any of the depended methods fails.</returns>
      /// <remarks>The calling application must have FILE_LIST_DIRECTORY access rights for this directory.</remarks>
      [SecurityCritical]
      public static DiskSpaceInfoExtended GetDiskFreeSpaceClusters(string drivePath)
      {
         return new DiskSpaceInfoExtended(drivePath, true);
      }

      #endregion // GetDiskFreeSpaceClusters

      #region IsReady

      /// <summary>Gets a value indicating whether a drive is ready.</summary>
      /// <param name="drivePath">A valid drive path or drive letter. This can be either uppercase or lowercase, 'a' to 'z' or a network share in the format: \\server\share</param>
      /// <returns><c>true</c> if the drive is ready; <c>false</c> otherwise.</returns>
      /// <remarks>This function currently does not support Network share paths, instead a Patch.GetCurrentDirectory() will be performed to check if the Network share path is available. If yes: IsReady == true.</remarks>
      [SecurityCritical]
      public static bool IsReady(string drivePath)
      {
         return IsReady(null, drivePath);
      }

      #region Transacted

      /// <summary>Gets a value indicating whether a drive is ready.</summary>
      /// <param name="transaction"><para>The transaction.</para></param>
      /// <param name="drivePath">A valid drive path or drive letter. This can be either uppercase or lowercase, 'a' to 'z' or a network share in the format: \\server\share</param>
      /// <returns><c>true</c> if the drive is ready; <c>false</c> otherwise.</returns>
      /// <remarks>This function currently does not support Network share paths, instead a Patch.GetCurrentDirectory() will be performed to check if the Network share path is available. If yes: IsReady == true.</remarks>
      [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
      [SecurityCritical]
      public static bool IsReady(KernelTransaction transaction, string drivePath)
      {
         drivePath = Path.DirectorySeparatorRemove(drivePath, false);

         // If path is a UNC path, check if the directory or file can be accessed.
         if (Path.IsUnc(drivePath))
            return FileSystemInfo.ExistsInternal(true, transaction, drivePath);

         try
         {
            // Use .NET function.
            return new System.IO.DriveInfo(drivePath).IsReady;
         }
         catch (Exception)
         {
            return false;
         }
      }

      #endregion // Transacted

      #endregion // IsReady

      #endregion // Drive

      #region Volume

      #region Label

      #region DeleteCurrentVolumeLabel

      /// <summary>Deletes the label of the file system volume that is the root of the current directory.</summary>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"></exception>
      [SecurityCritical]
      public static bool DeleteCurrentVolumeLabel()
      {
         return SetVolumeLabel(null, null);
      }

      #endregion // DeleteCurrentVolumeLabel

      #region DeleteVolumeLabel

      /// <summary>Deletes the label of a file system volume.</summary>
      /// <param name="rootPathName">The root directory of a file system volume. This is the volume the function will label.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"></exception>
      [SecurityCritical]
      public static bool DeleteVolumeLabel(string rootPathName)
      {
         if (string.IsNullOrEmpty(rootPathName))
            throw new ArgumentNullException(rootPathName);

         return SetVolumeLabel(rootPathName, null);
      }

      #endregion // DeleteVolumeLabel

      #region GetVolumeLabel

      /// <summary>Retrieve the label of a file system volume.</summary>
      /// <param name="rootPathName">
      /// A pointer to a string that contains the volume's Drive letter (for example, X:\)
      /// or the path of a mounted folder that is associated with the volume (for example, Y:\MountX\).
      /// If this parameter is null, the root of the current directory is used.
      /// </param>
      /// <returns>The the label of the file system volume. This function can return an empty string since a volume label is generally not mandatory.</returns>
      [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
      [SecurityCritical]
      public static string GetVolumeLabel(string rootPathName)
      {
         try
         {
            return GetVolumeInformation(rootPathName).Name;
         }
         catch (Exception)
         {
            return string.Empty;
         }
      }

      #endregion // GetVolumeLabel

      #region SetCurrentVolumeLabel

      /// <summary>Sets the label of the file system volume that is the root of the current directory.</summary>
      /// <param name="volumeName">A name for the volume. A pointer to a string that contains
      /// the new label for the volume. If this parameter is null, the function deletes any
      /// existing label from the specified volume and does not assign a new label.
      /// </param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"></exception>
      [SecurityCritical]
      public static bool SetCurrentVolumeLabel(string volumeName)
      {
         // volumeName == null is allowed.

         //if (string.IsNullOrEmpty(volumeName))
         //throw new ArgumentNullException("volumeName");

         // NTFS uses a limit of 32 characters for the volume label as of Windows Server 2003.
         return SetVolumeLabel(null, volumeName);
      }

      #endregion // SetCurrentVolumeLabel

      #region SetVolumeLabel

      /// <summary>Sets the label of a file system volume.</summary>
      /// <param name="rootPathName">
      /// A pointer to a string that contains the volume's Drive letter (for example, X:\)
      /// or the path of a mounted folder that is associated with the volume (for example, Y:\MountX\).
      /// If this parameter is null, the root of the current directory is used.
      /// </param>
      /// <param name="volumeName">A name for the volume. A pointer to a string that contains
      /// the new label for the volume. If this parameter is null, the function deletes any
      /// existing label from the specified volume and does not assign a new label.
      /// </param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"></exception>
      [SecurityCritical]
      public static bool SetVolumeLabel(string rootPathName, string volumeName)
      {
         // rootPathName == null is allowed.

         // Setting volume label only applies to Logical Drives pointing to local resources.
         //if (!Path.IsLogicalDrive(rootPathName))
         //return false;

         rootPathName = Path.DirectorySeparatorAdd(rootPathName, false);

         // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
         // Minimize method calls from here.
         // NTFS uses a limit of 32 characters for the volume label as of Windows Server 2003.
         using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
            if (!NativeMethods.SetVolumeLabel(rootPathName, volumeName))
               NativeError.ThrowException(rootPathName, volumeName);

         return true;
      }

      #endregion // SetVolumeLabel

      #endregion // Label

      #region GetDeviceForVolumeName

      /// <summary>Retrieves the Win32 Device name from the Volume name.</summary>
      /// <param name="volumeName">Name of the Volume</param>
      /// <returns>The Win32 Device name from the Volume name or <see cref="string.Empty"/> on error or if unavailable.</returns>
      [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
      [SecurityCritical]
      public static string GetDeviceForVolumeName(string volumeName)
      {
         if (string.IsNullOrEmpty(volumeName))
            throw new ArgumentNullException("volumeName");

         volumeName = Path.DirectorySeparatorRemove(volumeName, false);

         #region GlobalRoot

         if (volumeName.StartsWith(Path.GlobalRootPrefix, StringComparison.OrdinalIgnoreCase))
            return volumeName.Substring(Path.GlobalRootPrefix.Length);

         #endregion // GlobalRoot

         bool doQueryDos = false;

         #region Volume

         if (volumeName.StartsWith(Path.VolumePrefix, StringComparison.OrdinalIgnoreCase))
         {
            // Isolate the DOS Device from the Volume name, in the format: Volume{GUID}
            volumeName = volumeName.Substring(Path.LongPathPrefix.Length);
            doQueryDos = true;
         }

         #endregion // Volume

         #region Logical Drive

         // Check for Logical Drives: C:, D:, ...
         else if (Path.IsLogicalDrive(volumeName))
            doQueryDos = true;

         #endregion // Logical Drive

         if (doQueryDos)
         {
            try
            {
               // Get the real Device underneath.
               string dev = QueryDosDevice(volumeName).FirstOrDefault();
               return !string.IsNullOrEmpty(dev) ? dev : string.Empty;
            }
            catch (Exception)
            {
            }
         }

         return string.Empty;
      }

      #endregion // GetDeviceForVolumeName

      #region GetDisplayNameForVolume

      /// <summary>Gets the shortest display name for the specified <paramref name="volumeName"/>.</summary>
      /// <param name="volumeName">The volume name.</param>
      /// <returns>The shortest display name for the specified volume found, or <see cref="string.Empty"/> if no display names were found.</returns>
      /// <remarks>This method basically returns the shortest string returned by <see cref="GetVolumePathNamesForVolume"/></remarks>
      [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
      [SecurityCritical]
      public static string GetDisplayNameForVolume(string volumeName)
      {
         string[] smallestMountPoint = { new string(Path.WildcardStarMatchAllChar, NativeMethods.MaxPath) };

         try
         {
            foreach (string m in GetVolumePathNamesForVolume(volumeName).Where(m => !string.IsNullOrEmpty(m) && m.Length < smallestMountPoint[0].Length))
               smallestMountPoint[0] = m;
         }
         catch (Exception)
         {
         }

         return smallestMountPoint[0][0] == Path.WildcardStarMatchAllChar ? string.Empty : smallestMountPoint[0];
      }

      #endregion // GetDisplayNameForVolume

      #region GetUniqueVolumeNameForPath

      /// <summary>Get the unique volume name for the given path.</summary>
      /// <param name="volumePathName">A pointer to the input path string. Both absolute and relative file and directory names,
      /// for example <see cref="Filesystem.Path.ParentDirectoryPrefix"/>, are acceptable in this path.
      /// If you specify a relative directory or file name without a volume qualifier, GetUniqueVolumeNameForPath returns the Drive letter of the current volume.
      /// </param>
      /// <param name="options">options[0] = true: Remove the trailing backslash.</param>
      /// <returns>
      /// The unique name of the Volume Mount Point, a volume GUID path: \\?\Volume{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}\
      /// If not available or if the function fails, the return value <see cref="string.Empty"/>.
      /// </returns>
      /// <exception cref="NativeError.ThrowException()"></exception>
      [SuppressMessage("Microsoft.Interoperability", "CA1404:CallGetLastErrorImmediatelyAfterPInvoke")]
      [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
      [SecurityCritical]
      public static string GetUniqueVolumeNameForPath(string volumePathName, params bool[] options)
      {
         if (string.IsNullOrEmpty(volumePathName))
            throw new ArgumentNullException("volumePathName");

         bool removeDirectorySeparator = options != null && (options.Any() && options[0]);

         volumePathName = Path.DirectorySeparatorAdd(volumePathName, false);
         StringBuilder volumeRootPath = new StringBuilder(NativeMethods.MaxPath);

         // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
         // Minimize method calls from here.
         using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
         {
            // Get the root path of the Volume.
            if (NativeMethods.GetVolumePathName(volumePathName, volumeRootPath, (uint)volumeRootPath.Capacity))
            {
               try
               {
                  string uniqueVolumeName = GetUniqueVolumeNameForVolumeMountPoint(volumeRootPath.ToString());
                  return removeDirectorySeparator ? Path.DirectorySeparatorRemove(uniqueVolumeName, false) : uniqueVolumeName;
               }
               catch (Exception)
               {
               }
            }
            else
            {
               int lastError = Marshal.GetLastWin32Error();

               switch ((uint)lastError)
               {
                  // Don't throw exeception on these errors.
                  case Win32Errors.ERROR_NO_MORE_FILES:
                  case Win32Errors.ERROR_INVALID_PARAMETER:
                  case Win32Errors.ERROR_INVALID_NAME:
                     break;

                  default:
                     NativeError.ThrowException(lastError, volumePathName);
                     break;
               }
            }
         }

         return string.Empty;
      }

      #endregion // GetUniqueVolumeNameForPath

      #region GetVolumeInformation

      /// <summary>Retrieves information about the file system and volume associated with the specified root directory or filestream.</summary>
      /// <param name="volumePath">A path that contains the root directory.</param>
      /// <returns>A <see cref="VolumeInfo"/> instance describing the volume associatied with the specified root directory.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static VolumeInfo GetVolumeInformation(string volumePath)
      {
         return GetVolumeInformationInternal(volumePath, null, true);
      }

      /// <summary>Retrieves information about the file system and volume associated with the specified root directory or filestream.</summary>
      /// <param name="volumeHandle">A pointer to a <see cref="FileStream"/> handle.</param>
      /// <returns>A <see cref="VolumeInfo"/> instance describing the volume associatied with the specified root directory.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static VolumeInfo GetVolumeInformation(FileStream volumeHandle)
      {
         return GetVolumeInformationInternal(null, volumeHandle, true);
      }

      #endregion // GetVolumeInformation

      #region GetVolumes

      /// <summary>Retrieves the name of a volume on a computer. FindFirstVolume is used to begin scanning the volumes of a computer.</summary>
      /// <returns>An IEnumerable string containing the volume names on the computer.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      [SecurityCritical]
      public static IEnumerable<string> GetVolumes()
      {
         StringBuilder sb = new StringBuilder(NativeMethods.MaxPath);

         // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
         // Minimize method calls from here.
         using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
         using (SafeFindVolumeHandle handle = NativeMethods.FindFirstVolume(sb, (uint)sb.Capacity))
         {
            while (NativeMethods.IsValidHandle(handle, false))
            {
               if (NativeMethods.FindNextVolume(handle, sb, (uint)sb.Capacity))
                  yield return sb.ToString();

               else
               {
                  int lastError = Marshal.GetLastWin32Error();
                  if (lastError == Win32Errors.ERROR_NO_MORE_FILES)
                     yield break;

                  NativeError.ThrowException(lastError);
               }
            }
         }
      }

      #endregion // GetVolumes

      #region GetVolumePathNamesForVolume

      /// <summary>Retrieves a list of Drive letters and mounted folder paths for the specified volume.</summary>
      /// <param name="volumeName">A volume GUID path: \\?\Volume{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}\</param>
      /// <returns>An IEnumerable string containing the path names for the specified volume.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static IEnumerable<string> GetVolumePathNamesForVolume(string volumeName)
      {
         if (string.IsNullOrEmpty(volumeName))
            throw new ArgumentNullException("volumeName");

         string volName = Path.DirectorySeparatorAdd(volumeName, false);

         uint requiredLength = NativeMethods.MaxPath;
         char[] buffer = new char[requiredLength];

         // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
         // Minimize method calls from here.
         using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
            while (!NativeMethods.GetVolumePathNamesForVolumeName(volName, buffer, (uint)buffer.Length, out requiredLength))
            {
               int lastError = Marshal.GetLastWin32Error();
               if (lastError == Win32Errors.ERROR_MORE_DATA || lastError == Win32Errors.ERROR_INSUFFICIENT_BUFFER)
                  buffer = new char[requiredLength];
               else
                  NativeError.ThrowException(volumeName);
            }

         StringBuilder sb = new StringBuilder(buffer.Length);
         foreach (char c in buffer)
         {
            if (c != Path.StringTerminatorChar)
               sb.Append(c);
            else
            {
               if (sb.Length > 0)
               {
                  yield return sb.ToString();
                  sb.Length = 0;
               }
            }
         }
      }

      #endregion // GetVolumePathNamesForVolume

      #region IsSame

      /// <summary>Determines whether the volume of two filesystem objects is the same.</summary>
      /// <param name="fsoPath1">The first filesystem ojbect with full path information.</param>
      /// <param name="fsoPath2">The second filesystem object with full path information.</param>
      /// <returns><c>true</c> if both filesytem objects reside on the same volume, <c>false</c> otherwise.</returns>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "fso")]
      public static bool IsSame(string fsoPath1, string fsoPath2)
      {
         if (string.IsNullOrEmpty(fsoPath1))
            throw new ArgumentNullException(fsoPath1);

         if (string.IsNullOrEmpty(fsoPath2))
            throw new ArgumentNullException(fsoPath2);

         VolumeInfo volInfo1 = GetVolumeInformationInternal(Path.GetPathRoot(fsoPath1), null, false);
         VolumeInfo volInfo2 = GetVolumeInformationInternal(Path.GetPathRoot(fsoPath2), null, false);

         return (volInfo1 != null && volInfo2 != null) && volInfo1.SerialNumber.Equals(volInfo2.SerialNumber, StringComparison.OrdinalIgnoreCase);
      }

      #endregion // IsSame

      #region IsVolume

      /// <summary>Determines whether the specified volume name is a defined volume on the current computer.</summary>
      /// <param name="volumeMountPoint">A string representing the path to a volume.
      ///  eg.  "C:\",  "D:",  "P:\Mountpoint\Backup",  "\\?\Volume{c0580d5e-2ad6-11dc-9924-806e6f6e6963}\"
      /// </param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
      [SecurityCritical]
      public static bool IsVolume(string volumeMountPoint)
      {
         try
         {
            return !string.IsNullOrEmpty(GetUniqueVolumeNameForVolumeMountPoint(volumeMountPoint));
         }
         catch (Exception)
         {
            return false;
         }
      }

      #endregion // IsVolume
      
      #region Volume Mount Point

      #region DeleteVolumeMountPoint

      /// <summary>Deletes a Drive letter or mounted folder.</summary>
      /// <param name="volumeMountPoint">The Drive letter or mounted folder to be deleted. For example, X:\ or Y:\MountX\.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      /// <remarks>Deleting a mounted folder does not cause the underlying directory to be deleted.</remarks>
      /// <remarks>It's not an error to attempt to unmount a volume from a volume mount point when there is no volume actually mounted at that volume mount point.</remarks>
      [SecurityCritical]
      public static bool DeleteVolumeMountPoint(string volumeMountPoint)
      {
         if (string.IsNullOrEmpty(volumeMountPoint))
            throw new ArgumentNullException("volumeMountPoint");

         volumeMountPoint = Path.DirectorySeparatorAdd(volumeMountPoint, false);

         // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
         // Minimize method calls from here.
         using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
            if (!NativeMethods.DeleteVolumeMountPoint(volumeMountPoint))
               NativeError.ThrowException(volumeMountPoint);

         return true;
      }

      #endregion // DeleteVolumeMountPoint

      #region GetVolumeMountPoints

      /// <summary>Retrieves the names of all mounted folders (volume mount points) on the specified volume.</summary>
      /// <param name="volumeGuid">A <see langref="String"/> containing the volume GUID.</param>
      /// <returns>The names of all volume mount points on the specified volume or <see cref="string.Empty"/> on error or if unavailable.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      /// <remarks>
      /// After Windows Vista, the API has changed. The short version of the issue you may encounter with FindFirstVolumeMountPoint is that unless you have a superuser account and permissions to the share, the mount point will not show up in the list.
      /// Your enumerated lists of mount points can be incomplete and you will not know when listed folders require permissions.</remarks>
      [SuppressMessage("Microsoft.Interoperability", "CA1404:CallGetLastErrorImmediatelyAfterPInvoke")]
      [SecurityCritical]
      public static IEnumerable<string> GetVolumeMountPoints(string volumeGuid)
      {
         if (string.IsNullOrEmpty(volumeGuid))
            throw new ArgumentNullException("volumeGuid");

         if (!volumeGuid.StartsWith(Path.VolumePrefix, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("volumeGuid");

         volumeGuid = Path.DirectorySeparatorAdd(volumeGuid, false);

         StringBuilder name = new StringBuilder(NativeMethods.MaxPathUnicode);

         // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
         // Minimize method calls from here.
         using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
         using (SafeFindVolumeMountPointHandle handle = NativeMethods.FindFirstVolumeMountPoint(volumeGuid, name, (uint)name.Capacity))
         {
            int lastError;

            if (!NativeMethods.IsValidHandle(handle, false))
            {
               lastError = Marshal.GetLastWin32Error();
               switch ((uint)lastError)
               {
                  case Win32Errors.ERROR_NO_MORE_FILES:
                  case Win32Errors.ERROR_PATH_NOT_FOUND: // Observed with USB stick, FAT32 formatted.
                     yield break;

                  default:
                     NativeError.ThrowException(lastError, volumeGuid);
                     break;
               }
            }

            yield return name.ToString();

            while (NativeMethods.FindNextVolumeMountPoint(handle, name, (uint)name.Capacity))
               yield return name.ToString();

            lastError = Marshal.GetLastWin32Error();
            if (lastError != Win32Errors.ERROR_NO_MORE_FILES)
               NativeError.ThrowException(lastError, volumeGuid);
         }
      }

      #endregion // GetVolumeMountPoints

      #region GetUniqueVolumeNameForVolumeMountPoint

      /// <summary>Retrieves the unique volume name for the specified volume mount point or root directory.</summary>
      /// <param name="volumeMountPoint">The path of a volume mount point or a Drive letter indicating a root directory (eg. "C:" or "D:\").</param>
      /// <returns>The unique volume name of the form: "\\?\Volume{GUID}\" where GUID is the GUID that identifies the volume.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static string GetUniqueVolumeNameForVolumeMountPoint(string volumeMountPoint)
      {
         if (string.IsNullOrEmpty(volumeMountPoint))
            throw new ArgumentNullException("volumeMountPoint");

         volumeMountPoint = Path.DirectorySeparatorAdd(volumeMountPoint, false);

         // Get the Volume name alias, this may be different from the unique Volume name in some rare cases.
         StringBuilder volumeName = new StringBuilder(NativeMethods.MaxPath);
         StringBuilder uniqueName = new StringBuilder(NativeMethods.MaxPath);

         // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
         // Minimize method calls from here.
         using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
         {
            if (!NativeMethods.GetVolumeNameForVolumeMountPoint(volumeMountPoint, volumeName, (uint)volumeName.Capacity) ||
               !NativeMethods.GetVolumeNameForVolumeMountPoint(volumeName.ToString(), uniqueName, (uint)volumeName.Capacity))
               NativeError.ThrowException(volumeMountPoint);

            return uniqueName.ToString();
         }
      }

      #endregion // GetUniqueVolumeNameForVolumeMountPoint

      #region SetVolumeMountPoint

      /// <summary>Associates a volume with a Drive letter or a directory on another volume.</summary>
      /// <param name="volumeMountPoint">
      /// The user-mode path to be associated with the volume. This may be a Drive letter (for example, "X:\")
      /// or a directory on another volume (for example, "Y:\MountX\").
      /// </param>
      /// <param name="volumeGuid">A <see langref="String"/> containing the volume GUID.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool SetVolumeMountPoint(string volumeMountPoint, string volumeGuid)
      {
         if (string.IsNullOrEmpty(volumeMountPoint))
            throw new ArgumentNullException("volumeMountPoint");

         if (string.IsNullOrEmpty(volumeGuid))
            throw new ArgumentNullException("volumeGuid");

         if (!volumeGuid.StartsWith(Path.VolumePrefix, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException(volumeGuid);

         volumeGuid = Path.DirectorySeparatorAdd(volumeGuid, false);
         volumeMountPoint = Path.DirectorySeparatorAdd(volumeMountPoint, false);

         // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
         // Minimize method calls from here.
         using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
            if (!NativeMethods.SetVolumeMountPoint(volumeMountPoint, volumeGuid))
            {
               int lastError = Marshal.GetLastWin32Error();

               // If the lpszVolumeMountPoint parameter contains a path to a mounted folder,
               // GetLastError returns ERROR_DIR_NOT_EMPTY, even if the directory is empty.

               if (lastError != Win32Errors.ERROR_DIR_NOT_EMPTY)
                  NativeError.ThrowException(lastError, volumeMountPoint, volumeGuid);
            }

         return true;
      }

      #endregion // SetVolumeMountPoint

      #endregion // Volume Mount Point

      #endregion // Volume


      #region Unified Internals

      /// <summary>Unified method GetVolumeInformationInternal() to retrieve information about the file system and volume associated with the specified root directory or filestream.</summary>
      /// <param name="volumePath">A path that contains the root directory.</param>
      /// <param name="volumeHandle">A pointer to a <see cref="FileStream"/> handle.</param>
      /// <param name="raiseException">If <c>true</c> raises Exceptions, when <c>false</c> no Exceptions are raised and the method returns <see langref="null"/>.</param>
      /// <returns>A <see cref="VolumeInfo"/> instance describing the volume associatied with the specified root directory. See <paramref name="raiseException"/></returns>
      /// <remarks>Either use <paramref name="volumePath"/> or <paramref name="volumeHandle"/>, not both.</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SuppressMessage("Microsoft.Interoperability", "CA1404:CallGetLastErrorImmediatelyAfterPInvoke")]
      [SecurityCritical]
      internal static VolumeInfo GetVolumeInformationInternal(string volumePath, FileStream volumeHandle, bool raiseException)
      {
         bool isHandle = false;

         if (!string.IsNullOrEmpty(volumePath))
            volumePath = Path.GetRegularPath(Path.DirectorySeparatorAdd(volumePath, false));

         else
         {
            if (volumeHandle == null || !NativeMethods.IsValidHandle(volumeHandle.SafeFileHandle, false))
               throw new ArgumentNullException("volumeHandle");

            isHandle = true;
         }

         uint serialNumber;
         uint maximumComponentLength;
         NativeMethods.VolumeInfoAttributes volumeInfoAttrs;
         StringBuilder volumeNameBuffer = new StringBuilder(NativeMethods.MaxPath);
         StringBuilder fileSystemNameBuffer = new StringBuilder(NativeMethods.MaxPath);

         // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
         // Minimize method calls from here.
         using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
            if (!(isHandle
                     ? NativeMethods.GetVolumeInformationByHandle(volumeHandle.SafeFileHandle, volumeNameBuffer, (uint) volumeNameBuffer.Capacity, out serialNumber, out maximumComponentLength, out volumeInfoAttrs, fileSystemNameBuffer, (uint) fileSystemNameBuffer.Capacity)
                     : NativeMethods.GetVolumeInformation(volumePath, volumeNameBuffer, (uint) volumeNameBuffer.Capacity, out serialNumber, out maximumComponentLength, out volumeInfoAttrs, fileSystemNameBuffer, (uint) fileSystemNameBuffer.Capacity)))
               if (raiseException)
                  NativeError.ThrowException(volumePath);
               else return null;

         return new VolumeInfo
            {
               FileSystemName = fileSystemNameBuffer.ToString(),
               FullPath = volumePath,
               MaximumComponentLength = maximumComponentLength,
               Name = volumeNameBuffer.ToString(),
               SerialNumber = serialNumber.ToString(CultureInfo.InvariantCulture),
               VolumeInfoAttributes = volumeInfoAttrs
            };
      }

      #endregion // Unified Internals
   }
}