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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using Alphaleonis.Win32.Security;
using Microsoft.Win32.SafeHandles;
using SecurityNativeMethods = Alphaleonis.Win32.Security.NativeMethods;

namespace Alphaleonis.Win32.Filesystem
{
   internal static class NativeMethods
   {
      #region Internal Utility

      internal static uint GetLowOrderDword(long lowPart)
      {
         return (uint)(lowPart & 0xFFFFFFFF);
      }

      internal static uint GetHighOrderDword(long highPart)
      {
         return (uint)((highPart >> 32) & 0xFFFFFFFF);
      }

      internal static long ToLong(uint highPart, uint lowPart)
      {
         return (((long)highPart) << 32) | (((long)lowPart) & 0xFFFFFFFF);
      }

      internal static ulong LuidToLong(SecurityNativeMethods.Luid luid)
      {
         ulong high = (((ulong)luid.HighPart) << 32);
         ulong low = (((ulong)luid.LowPart) & 0x00000000FFFFFFFF);
         return high | low;
      }

      internal static SecurityNativeMethods.Luid LongToLuid(ulong lluid)
      {
         return new SecurityNativeMethods.Luid { HighPart = (uint)(lluid >> 32), LowPart = (uint)(lluid & 0xFFFFFFFF) };
      }

      internal static bool HasFileAttribute(FileAttributes attributes, FileAttributes hasAttribute)
      {
         return (attributes & hasAttribute) == hasAttribute;
      }

      internal static bool HasVolumeInfoAttribute(VolumeInfoAttributes attributes, VolumeInfoAttributes hasAttribute)
      {
         return (attributes & hasAttribute) == hasAttribute;
      }

      /// <summary>Check is the current handle is not null, not closed and not invalid.</summary>
      /// <param name="handle">The current handle to check.</param>
      /// <param name="throwException"><c>true</c> will throw an <exception cref="Resources.HandleInvalid"/>, <c>false</c> will not raise this exception..</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      internal static bool IsValidHandle(SafeHandle handle, bool throwException = true)
      {
         if (handle == null || handle.IsClosed || handle.IsInvalid)
         {
            if (throwException)
               throw new ArgumentException(Resources.HandleInvalid);

            return false;
         }

         return true;
      }

      /// <summary>Check is the current stream is not null, not closed and not invalid.</summary>
      /// <param name="stream">The current stream to check.</param>
      /// <param name="throwException"><c>true</c> will throw an <exception cref="Resources.HandleInvalid"/>, <c>false</c> will not raise this exception.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      internal static bool IsValidStream(FileStream stream, bool throwException = true)
      {
         if (stream == null || stream.SafeFileHandle == null || stream.SafeFileHandle.IsClosed || stream.SafeFileHandle.IsInvalid)
         {
            if (throwException)
               throw new ArgumentException(Resources.StreamInvalid);

            return false;
         }

         return true;
      }

      #region UnitSizeToText
      
      /// <summary>Convert a number of type T to string with UnitSize or Percentage suffixed.</summary>
      internal static string UnitSizeToText<T>(T numberOfBytes, params bool[] options)
      {
         // Suffixes
         // bool[0]  = false = "MB", True = "MiB"
         // bool[1]  = true = %
         bool useMebi = options != null && options.Any() && options[0];
         bool usePercent = options != null && options.Count() == 2 && options[1];

         string template = "{0:0.00}{1}";
         string sfx = useMebi ? "Bi" : "bytes";

         double bytes = Convert.ToDouble(numberOfBytes, CultureInfo.InvariantCulture);

         if (bytes >= 1125899906842624) { sfx = useMebi ? "PiB" : "PB"; bytes /= 1125899906842624; }
         else if (bytes >= 1099511627776) { sfx = useMebi ? "TiB" : "TB"; bytes /= 1099511627776; }
         else if (bytes >= 1073741824) { sfx = useMebi ? "GiB" : "GB"; bytes /= 1073741824; }
         else if (bytes >= 1048576) { sfx = useMebi ? "MiB" : "MB"; bytes /= 1048576; }
         else if (bytes >= 1024) { sfx = useMebi ? "KiB" : "KB"; bytes /= 1024; }

         else if (!usePercent)
            // Will return "512 bytes" instead of "512,00 bytes".
            template = "{0:0}{1}";

         return string.Format(CultureInfo.CurrentCulture, template, bytes, usePercent ? "%" : " " + sfx);
      }

      /// <summary>Calculates a percentage value.</summary>
      /// <param name="currentValue"></param>
      /// <param name="minimumValue"></param>
      /// <param name="maximumValue"></param>
      internal static double PercentCalculate(double currentValue, double minimumValue, double maximumValue)
      {
         return (currentValue < 0 || maximumValue <= 0) ? 0 : currentValue * 100 / (maximumValue - minimumValue);
      }

      #endregion // UnitSizeToText

      #region SetErrorMode

      /// <summary>Enum for struct ChangeErrorMode.</summary>
      [Flags]
      internal enum NativeErrorMode
      {
         SystemDefault = 0x0,
         FailCriticalErrors = 0x0001,
         NoGpfaultErrorbox = 0x0002,
         NoAlignmentFaultExcept = 0x0004,
         NoOpenFileErrorbox = 0x8000
      }

      /// <summary>Controls whether the system will handle the specified types of serious errors or whether the process will handle them.</summary>
      /// <remarks>Not yet implemented: Windows 7, callers should favor SetThreadErrorMode over SetErrorMode since it is less disruptive to the normal behavior of the system.</remarks>
      /// <remarks>Minimum supported client: Windows 2000 Professional</remarks>
      /// <remarks>Minimum supported server: Windows 2000 Server</remarks>
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      internal struct ChangeErrorMode : IDisposable
      {
         private readonly NativeErrorMode _oldMode;

         internal ChangeErrorMode(NativeErrorMode mode)
         {
            _oldMode = SetErrorMode(mode);
         }

         void IDisposable.Dispose()
         {
            SetErrorMode(_oldMode);
         }
      }

      /// <summary>Controls whether the system will handle the specified types of serious errors or whether the process will handle them.</summary>
      /// <returns>The return value is the previous state of the error-mode bit attributes.</returns>
      /// <remarks>
      /// Because the error mode is set for the entire process, you must ensure that multi-threaded applications
      /// do not set different error-mode attributes. Doing so can lead to inconsistent error handling.
      /// </remarks>
      /// <remarks>SetLastError is set to false.</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = false, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.U4)]
      private static extern NativeErrorMode SetErrorMode(NativeErrorMode nativeErrorMode);

      #endregion // SetErrorMode

      #endregion // Internal Utility

      #region Constants

      #region Standard Values

      /// <summary>MaxPath = 260
      /// The specified path, file name, or both exceed the system-defined maximum length.
      /// For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters. 
      /// </summary>
      internal const int MaxPath = 260;

      /// <summary>MaxPathUnicode = 32767</summary>
      internal const int MaxPathUnicode = 32767;

      /// <summary>DefaultFileEncoding = Encoding.UTF8; Default type of Encoding used for reading and writing files.</summary>
      internal static readonly Encoding DefaultFileEncoding = Encoding.UTF8;

      /// <summary>DefaultFileBufferSize = 4096; Default type buffer size used for reading and writing files.</summary>
      internal const int DefaultFileBufferSize = 4096;

      /// <summary>Combination of <see cref="CopyOptions.FailIfExists"/> and <see cref="CopyOptions.NoBuffering"/></summary>
      internal const CopyOptions CopyOptsFail = CopyOptions.FailIfExists | CopyOptions.NoBuffering;

      /// <summary>Combination of <see cref="CopyOptions.None"/> and <see cref="CopyOptions.NoBuffering"/></summary>
      internal const CopyOptions CopyOptsNone = CopyOptions.None;

      /// <summary>Combination of <see cref="MoveOptions.ReplaceExisting"/> and <see cref="MoveOptions.CopyAllowed"/></summary>
      internal const MoveOptions MoveOptsReplace = MoveOptions.ReplaceExisting | MoveOptions.CopyAllowed;
      
      #endregion // Standard Values

      #region LargeCache (Used by FindFirstFileEx / FindNextFile)
      /// <summary>Uses a larger buffer for directory queries, which can increase performance of the find operation.</summary>
      /// <remarks>This value is not supported until Windows Server 2008 R2 and Windows 7.</remarks>
      internal static bool LargeCache = OperatingSystemInfo.IsAtLeast(OSVersionName.Windows7);

      #endregion // LargeCache (Used by FindFirstFileEx / FindNextFile)

      #region Dos Device Flags (Used by DefineDosDevice)

      // 2012-01-31: Yomodo; Moved to: Filesystem\Enumerations\DosDeviceAttributes.cs as DosDeviceAttributes enum
      // Used by DefineDosDevice 
      //internal const uint DDD_RAW_TARGET_PATH = 0x00000001;
      //internal const uint DDD_REMOVE_DEFINITION = 0x00000002;
      //internal const uint DDD_EXACT_MATCH_ON_REMOVE = 0x00000004;
      //internal const uint DDD_NO_BROADCAST_SYSTEM = 0x00000008;

      #endregion // Dos Device Flags (Used by DefineDosDevice)

      #region File System Flags (Used by GetVolumeInformation)

      // 2012-02-14: Yomodo; Moved to Filesystem\Enumerations\VolumeInfoAttributes.cs as VolumeInfoAttributes enum
      // Used by GetVolumeInformation
      //internal const uint FILE_CASE_SENSITIVE_SEARCH = 0x00000001;
      //internal const uint FILE_CASE_PRESERVED_NAMES = 0x00000002;
      //internal const uint FILE_UNICODE_ON_DISK = 0x00000004;
      //internal const uint FILE_PERSISTENT_ACLS = 0x00000008;
      //internal const uint FILE_FILE_COMPRESSION = 0x00000010;
      //internal const uint FILE_VOLUME_QUOTAS = 0x00000020;
      //internal const uint FILE_SUPPORTS_SPARSE_FILES = 0x00000040;
      //internal const uint FILE_SUPPORTS_REPARSE_POINTS = 0x00000080;
      //internal const uint FILE_SUPPORTS_REMOTE_STORAGE = 0x00000100;
      //internal const uint FILE_VOLUME_IS_COMPRESSED = 0x00008000;
      //internal const uint FILE_SUPPORTS_OBJECT_IDS = 0x00010000;
      //internal const uint FILE_SUPPORTS_ENCRYPTION = 0x00020000;
      //internal const uint FILE_NAMED_STREAMS = 0x00040000;
      //internal const uint FILE_READ_ONLY_VOLUME = 0x00080000;

      #endregion // File System Flags (Used by GetVolumeInformation)

      #region Drive Types (Used by GetDriveType)

      // 2012-02-14: Yomodo; Obsolete, .NET DriveType enum is used instead.
      //internal const uint DRIVE_UNKNOWN = 0;
      //internal const uint DRIVE_NO_ROOT_DIR = 1;
      //internal const uint DRIVE_REMOVABLE = 2;
      //internal const uint DRIVE_FIXED = 3;
      //internal const uint DRIVE_REMOTE = 4;
      //internal const uint DRIVE_CDROM = 5;
      //internal const uint DRIVE_RAMDISK = 6;

      #endregion // Drive Types (Used by GetDriveType)

      #region File Access and Rights

      // 2012-01-31: Yomodo; Moved to: Filesystem\Enumerations\FileSystemRights.cs as FileSystemRights enum
      // 2012-10-10: Yomodo; .NET already provides this enum.
      //internal const uint DRIVE_UNKNOWN = 0;
      //public const uint ACCESS_SYSTEM_SECURITY = 0x01000000;
      //public const uint DELETE = 0x00010000;
      //public const uint READ_CONTROL = 0x00020000;
      //public const uint WRITE_DAC = 0x00040000;
      //public const uint WRITE_OWNER = 0x00080000;
      //public const uint SYNCHRONIZE = 0x00100000;
      //public const uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
      //public const uint STANDARD_RIGHTS_READ = READ_CONTROL;
      //public const uint STANDARD_RIGHTS_WRITE = READ_CONTROL;
      //public const uint STANDARD_RIGHTS_EXECUTE = READ_CONTROL;
      //public const uint STANDARD_RIGHTS_ALL = 0x001F0000;
      //public const uint SPECIFIC_RIGHTS_ALL = 0x0000FFFF;

      //public const uint FILE_READ_DATA = 0x0001;
      //public const uint FILE_LIST_DIRECTORY = 0x0001;
      //public const uint FILE_WRITE_DATA = 0x0002;
      //public const uint FILE_ADD_FILE = 0x0002;
      //public const uint FILE_APPEND_DATA = 0x0004;
      //public const uint FILE_ADD_SUBDIRECTORY = 0x0004;
      //public const uint FILE_CREATE_PIPE_INSTANCE = 0x0004;
      //public const uint FILE_READ_EA = 0x0008;
      //public const uint FILE_WRITE_EA = 0x0010;
      //public const uint FILE_EXECUTE = 0x0020;
      //public const uint FILE_TRAVERSE = 0x0020;
      //public const uint FILE_DELETE_CHILD = 0x0040;
      //public const uint FILE_READ_ATTRIBUTES = 0x0080;
      //public const uint FILE_WRITE_ATTRIBUTES = 0x0100;
      //public const uint FILE_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0x1FF;
      //public const uint FILE_GENERIC_READ = STANDARD_RIGHTS_READ | FILE_READ_DATA | FILE_READ_ATTRIBUTES | FILE_READ_EA | SYNCHRONIZE;
      //public const uint FILE_GENERIC_WRITE = STANDARD_RIGHTS_WRITE | FILE_WRITE_DATA | FILE_WRITE_ATTRIBUTES | FILE_WRITE_EA | FILE_APPEND_DATA | SYNCHRONIZE;
      //public const uint FILE_GENERIC_EXECUTE = STANDARD_RIGHTS_EXECUTE | FILE_READ_ATTRIBUTES | FILE_EXECUTE | SYNCHRONIZE;
      //public const uint FILE_SHARE_READ = 0x00000001;
      //public const uint FILE_SHARE_WRITE = 0x00000002;
      //public const uint FILE_SHARE_DELETE = 0x00000004;
      //public const uint FILE_ATTRIBUTE_READONLY = 0x00000001;
      //public const uint FILE_ATTRIBUTE_HIDDEN = 0x00000002;
      //public const uint FILE_ATTRIBUTE_SYSTEM = 0x00000004;
      //public const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
      //public const uint FILE_ATTRIBUTE_ARCHIVE = 0x00000020;
      //public const uint FILE_ATTRIBUTE_DEVICE = 0x00000040;
      //public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
      //public const uint FILE_ATTRIBUTE_TEMPORARY = 0x00000100;
      //public const uint FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200;
      //public const uint FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400;
      //public const uint FILE_ATTRIBUTE_COMPRESSED = 0x00000800;
      //public const uint FILE_ATTRIBUTE_OFFLINE = 0x00001000;
      //public const uint FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000;
      //public const uint FILE_ATTRIBUTE_ENCRYPTED = 0x00004000;
      //public const uint FILE_ATTRIBUTE_VIRTUAL = 0x00010000;
      //public const uint FILE_NOTIFY_CHANGE_FILE_NAME = 0x00000001;
      //public const uint FILE_NOTIFY_CHANGE_DIR_NAME = 0x00000002;
      //public const uint FILE_NOTIFY_CHANGE_ATTRIBUTES = 0x00000004;
      //public const uint FILE_NOTIFY_CHANGE_SIZE = 0x00000008;
      //public const uint FILE_NOTIFY_CHANGE_LAST_WRITE = 0x00000010;
      //public const uint FILE_NOTIFY_CHANGE_LAST_ACCESS = 0x00000020;
      //public const uint FILE_NOTIFY_CHANGE_CREATION = 0x00000040;
      //public const uint FILE_NOTIFY_CHANGE_SECURITY = 0x00000100;
      //public const uint FILE_ACTION_ADDED = 0x00000001;
      //public const uint FILE_ACTION_REMOVED = 0x00000002;
      //public const uint FILE_ACTION_MODIFIED = 0x00000003;
      //public const uint FILE_ACTION_RENAMED_OLD_NAME = 0x00000004;
      //public const uint FILE_ACTION_RENAMED_NEW_NAME = 0x00000005;
      //public const uint FILE_SEQUENTIAL_WRITE_ONCE = 0x00100000;
      //public const uint FILE_SUPPORTS_TRANSACTIONS = 0x00200000;

      //public const uint REPLACEFILE_WRITE_THROUGH = 0x01;
      //public const uint REPLACEFILE_IGNORE_MERGE_ERRORS = 0x02;
      //public const uint REPLACEFILE_IGNORE_ACL_ERRORS = 0x04;

      #endregion // File Access and Rights

      #region Security

      // 2012-01-31: Yomodo; Moved to: Filesystem\Enumerations\SecurityInformation.cs as SecurityInformation enum
      //public const int OWNER_SECURITY_INFORMATION = 0x00000001;
      //public const int GROUP_SECURITY_INFORMATION = 0x00000002;
      //public const int DACL_SECURITY_INFORMATION = 0x00000004;
      //public const int SACL_SECURITY_INFORMATION = 0x00000008;
      /* Not needed?
      public const uint LABEL_SECURITY_INFORMATION = 0x00000010;
      public const uint PROTECTED_DACL_SECURITY_INFORMATION = 0x80000000;
      public const uint PROTECTED_SACL_SECURITY_INFORMATION = 0x40000000;
      public const uint UNPROTECTED_DACL_SECURITY_INFORMATION = 0x20000000;
      public const uint UNPROTECTED_SACL_SECURITY_INFORMATION = 0x10000000;
      */
      #endregion

      #region Backup

      // 2012-01-31: Yomodo; Moved to: Filesystem\Enumerations\BackupStream.cs as BackupStreamAttributes enum
      // (Only moved these values, enum already existed)
      //public const uint STREAM_NORMAL_ATTRIBUTE = 0x00000000;
      //public const uint STREAM_MODIFIED_WHEN_READ = 0x00000001;
      //public const uint STREAM_CONTAINS_SECURITY = 0x00000002;
      //public const uint STREAM_CONTAINS_PROPERTIES = 0x00000004;
      //public const uint STREAM_SPARSE_ATTRIBUTE = 0x00000008;

      #endregion

      #endregion // Constants

      #region Volume Management

      #region DosDevice

      /// <summary>Defines, redefines, or deletes MS-DOS device names.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// To get extended error information call Win32Exception()
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "DefineDosDeviceW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool DefineDosDevice(DosDeviceAttributes dwAttributes, [MarshalAs(UnmanagedType.LPWStr)] string lpDeviceName, [MarshalAs(UnmanagedType.LPWStr)] string lpTargetPath);

      /// <summary>Retrieves information about MS-DOS device names.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// To get extended error information call Win32Exception()
      /// If the buffer is too small, the function fails and the last error code is ERROR_INSUFFICIENT_BUFFER.
      /// </returns>
      /// <remarks>
      /// Windows Server 2003 and Windows XP: QueryDosDevice first searches the Local MS-DOS Device namespace for the specified device name.
      /// If the device name is not found, the function will then search the Global MS-DOS Device namespace.
      /// </remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "QueryDosDeviceW")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint QueryDosDevice([MarshalAs(UnmanagedType.LPWStr)] string lpDeviceName, char[] lpTargetPath, [MarshalAs(UnmanagedType.U4)] uint ucchMax);

      #endregion // DosDevice

      #region Volume

      /// <summary>Volume Attributes used by the GetVolumeInformation() function.</summary>
      [SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
      [Flags]
      internal enum VolumeInfoAttributes
      {
         /// <summary>Default</summary>
         None = 0,

         /// <summary>FILE_CASE_SENSITIVE_SEARCH The specified volume supports case-sensitive file names.</summary>
         CaseSensitiveSearch = 1,

         /// <summary> FILE_CASE_PRESERVED_NAMES The specified volume supports preserved case of file names when it places a name on disk.</summary>
         CasePreservedNames = 2,

         /// <summary> FILE_UNICODE_ON_DISK The specified volume supports Unicode in file names as they appear on disk.</summary>
         UnicodeOnDisk = 4,

         /// <summary> FILE_PERSISTENT_ACLS The specified volume preserves and enforces access control lists (ACL).
         /// For example, the NTFS file system preserves and enforces ACLs, and the FAT file system does not.
         /// </summary>
         [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Acls")]
         PersistentAcls = 8,

         /// <summary> FILE_FILE_COMPRESSION The specified volume supports file-based compression.</summary>
         Compression = 16,

         /// <summary> FILE_VOLUME_QUOTAS The specified volume supports disk quotas.</summary>
         VolumeQuotas = 32,

         /// <summary> FILE_SUPPORTS_SPARSE_FILES The specified volume supports sparse files.</summary>
         SupportsSparseFiles = 64,

         /// <summary> FILE_SUPPORTS_REPARSE_POINTS The specified volume supports re-parse points.</summary>
         SupportsReparsePoints = 128,

         /// <summary>(doesn't appear on MSDN)</summary>
         SupportsRemoteStorage = 256,

         /// <summary>FILE_VOLUME_IS_COMPRESSED The specified volume is a compressed volume, for example, a DoubleSpace volume.</summary>
         VolumeIsCompressed = 32768,

         /// <summary>FILE_SUPPORTS_OBJECT_IDS The specified volume supports object identifiers.</summary>
         SupportsObjectIds = 65536,

         /// <summary>FILE_SUPPORTS_ENCRYPTION The specified volume supports the Encrypted File System (EFS). For more information, see File Encryption.</summary>
         SupportsEncryption = 131072,

         /// <summary>FILE_NAMED_STREAMS The specified volume supports named streams.</summary>
         NamedStreams = 262144,

         /// <summary>FILE_READ_ONLY_VOLUME The specified volume is read-only.</summary>
         ReadOnlyVolume = 524288,

         /// <summary>FILE_SEQUENTIAL_WRITE_ONCE The specified volume is read-only.</summary>
         SequentialWriteOnce = 1048576,

         /// <summary>FILE_SUPPORTS_TRANSACTIONS The specified volume supports transactions.For more information, see About KTM.</summary>
         SupportsTransactions = 2097152,

         /// <summary>FILE_SUPPORTS_HARD_LINKS The specified volume supports hard links. For more information, see Hard Links and Junctions.</summary>
         /// <remarks>
         /// Windows Server 2008, Windows Vista, Windows Server 2003, and Windows XP:
         /// This value is not supported until Windows Server 2008 R2 and Windows 7.
         /// </remarks>
         SupportsHardLinks = 4194304,

         /// <summary>FILE_SUPPORTS_EXTENDED_ATTRIBUTES The specified volume supports extended attributes.
         /// An extended attribute is a piece of application-specific metadata that
         /// an application can associate with a file and is not part of the file's data.
         /// </summary>
         /// <remarks>
         /// Windows Server 2008, Windows Vista, Windows Server 2003, and Windows XP:
         /// This value is not supported until Windows Server 2008 R2 and Windows 7.
         /// </remarks>
         SupportsExtendedAttributes = 8388608,

         /// <summary>FILE_SUPPORTS_OPEN_BY_FILE_ID The file system supports open by FileID. For more information, see FILE_ID_BOTH_DIR_INFO.</summary>
         /// <remarks>
         /// Windows Server 2008, Windows Vista, Windows Server 2003, and Windows XP:
         /// This value is not supported until Windows Server 2008 R2 and Windows 7.
         /// </remarks>
         SupportsOpenByFileId = 16777216,

         /// <summary>FILE_SUPPORTS_USN_JOURNAL The specified volume supports update sequence number (USN) journals. For more information, see Change Journal Records.</summary>
         /// <remarks>
         /// Windows Server 2008, Windows Vista, Windows Server 2003, and Windows XP:
         /// This value is not supported until Windows Server 2008 R2 and Windows 7.
         /// </remarks>
         [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Usn")]
         SupportsUsnJournal = 33554432,
      }

      /// <summary>Retrieves the name of a volume on a computer. FindFirstVolume is used to begin scanning the volumes of a computer.</summary>
      /// <returns>
      /// If the function succeeds, the return value is a search handle used in a subsequent call to the FindNextVolume and FindVolumeClose functions.
      /// If the function fails to find any volumes, the return value is the INVALID_HANDLE_VALUE error code. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "FindFirstVolumeW")]
      internal extern static SafeFindVolumeHandle FindFirstVolume(StringBuilder lpszVolumeName, [MarshalAs(UnmanagedType.U4)] uint cchBufferLength);

      /// <summary>Retrieves the name of a mounted folder on the specified volume. FindFirstVolumeMountPoint is used to begin scanning the mounted folders on a volume.</summary>
      /// <returns>
      /// If the function succeeds, the return value is a search handle used in a subsequent call to the FindNextVolumeMountPoint and FindVolumeMountPointClose functions.
      /// If the function fails to find a mounted folder on the volume, the return value is the INVALID_HANDLE_VALUE error code.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      /// <remarks>"lpszRootPathName" must end with a trailing backslash.</remarks>
      /// <remarks>
      /// Might not enumerate all: http://blogs.msdn.com/b/seealso/archive/2011/07/27/use-this-not-this-apis-to-avoid-all-together.aspx
      /// After Windows Vista, the API has changed. The short version of the issue you may encounter with FindFirstVolumeMountPoint is that unless you have a superuser account and permissions to the share, the mount point will not show up in the list.
      /// Your enumerated lists of mount points can be incomplete and you will not know when listed folders require permissions.</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "FindFirstVolumeMountPointW")]
      internal extern static SafeFindVolumeMountPointHandle FindFirstVolumeMountPoint([MarshalAs(UnmanagedType.LPWStr)] string lpszRootPathName, StringBuilder lpszVolumeMountPoint, [MarshalAs(UnmanagedType.U4)] uint cchBufferLength);

      /// <summary>Continues a volume search started by a call to the FindFirstVolume function. FindNextVolume finds one volume per call.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError. If no matching files can be found, the GetLastError function
      /// returns the ERROR_NO_MORE_FILES error code. In that case, close the search with the FindVolumeClose function.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "FindNextVolumeW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal extern static bool FindNextVolume(SafeHandle hFindVolume, StringBuilder lpszVolumeName, [MarshalAs(UnmanagedType.U4)] uint cchBufferLength);

      /// <summary>Continues a mounted folder search started by a call to the FindFirstVolumeMountPoint function. FindNextVolumeMountPoint finds one mounted folder per call.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError. If no more mounted folders can be found, the GetLastError function returns the ERROR_NO_MORE_FILES error code.
      /// In that case, close the search with the FindVolumeMountPointClose function.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "FindNextVolumeMountPointW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal extern static bool FindNextVolumeMountPoint(SafeHandle hFindVolume, StringBuilder lpszVolumeName, [MarshalAs(UnmanagedType.U4)] uint cchBufferLength);

      /// <summary>Closes the specified volume search handle.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal extern static bool FindVolumeClose(IntPtr hFindVolume);

      /// <summary>Closes the specified mounted folder search handle.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal extern static bool FindVolumeMountPointClose(IntPtr hFindVolume);

      /// <summary>Retrieves a volume GUID path for the volume that is associated with the specified volume mount point (drive letter, volume GUID path, or mounted folder).</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// To get extended error information call Win32Exception()
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      /// <remarks>"lpszVolumeMountPoint" must end with a trailing backslash.</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetVolumeNameForVolumeMountPointW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool GetVolumeNameForVolumeMountPoint([MarshalAs(UnmanagedType.LPWStr)] string lpszVolumeMountPoint, StringBuilder lpszVolumeName, [MarshalAs(UnmanagedType.U4)] uint cchBufferLength);

      /// <summary>Retrieves the volume mount point where the specified path is mounted.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// To get extended error information call Win32Exception()
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetVolumePathNameW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool GetVolumePathName([MarshalAs(UnmanagedType.LPWStr)] string lpszFileName, StringBuilder lpszVolumePathName, [MarshalAs(UnmanagedType.U4)] uint cchBufferLength);

      /// <summary>Retrieves a list of drive letters and mounted folder paths for the specified volume.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetVolumePathNamesForVolumeNameW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool GetVolumePathNamesForVolumeName([MarshalAs(UnmanagedType.LPWStr)] string lpszVolumeName, char[] lpszVolumePathNames, [MarshalAs(UnmanagedType.U4)] uint cchBuferLength, [MarshalAs(UnmanagedType.U4)] out uint lpcchReturnLength);

      /// <summary>Retrieves information about the file system and volume associated with the specified root directory.</summary>
      /// <returns>
      /// If all the requested information is retrieved, the return value is nonzero.
      /// If not all the requested information is retrieved, the return value is zero.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      /// <remarks>"lpRootPathName" must end with a trailing backslash.</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetVolumeInformationW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal extern static bool GetVolumeInformation([MarshalAs(UnmanagedType.LPWStr)] string lpRootPathName, StringBuilder lpVolumeNameBuffer, [MarshalAs(UnmanagedType.U4)] uint nVolumeNameSize, [MarshalAs(UnmanagedType.U4)] out uint lpVolumeSerialNumber, [MarshalAs(UnmanagedType.U4)] out uint lpMaximumComponentLength, [MarshalAs(UnmanagedType.U4)] out VolumeInfoAttributes lpFileSystemAttributes, StringBuilder lpFileSystemNameBuffer, [MarshalAs(UnmanagedType.U4)] uint nFileSystemNameSize);

      /// <summary>Retrieves information about the file system and volume associated with the specified file.</summary>
      /// <returns>
      /// If all the requested information is retrieved, the return value is nonzero.
      /// If not all the requested information is retrieved, the return value is zero.
      /// To get extended error information call Win32Exception()
      /// </returns>
      /// <remarks>Minimum supported client: Windows Vista</remarks>
      /// <remarks>Minimum supported server: Windows Server 2008</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetVolumeInformationByHandleW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal extern static bool GetVolumeInformationByHandle(SafeHandle hFile, StringBuilder lpVolumeNameBuffer, [MarshalAs(UnmanagedType.U4)] uint nVolumeNameSize, [MarshalAs(UnmanagedType.U4)] out uint lpVolumeSerialNumber, [MarshalAs(UnmanagedType.U4)] out uint lpMaximumComponentLength, out VolumeInfoAttributes lpFileSystemAttributes, StringBuilder lpFileSystemNameBuffer, [MarshalAs(UnmanagedType.U4)] uint nFileSystemNameSize);

      /// <summary>Sets the label of a file system volume.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      /// <remarks>"lpRootPathName" must end with a trailing backslash.</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "SetVolumeLabelW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal extern static bool SetVolumeLabel([MarshalAs(UnmanagedType.LPWStr)] string lpRootPathName, [MarshalAs(UnmanagedType.LPWStr)] string lpVolumeName);

      /// <summary>Associates a volume with a drive letter or a directory on another volume.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      /// <remarks>"lpszVolumeMountPoint" must end with a trailing backslash.</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "SetVolumeMountPointW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal extern static bool SetVolumeMountPoint([MarshalAs(UnmanagedType.LPWStr)] string lpszVolumeMountPoint, [MarshalAs(UnmanagedType.LPWStr)] string lpszVolumeName);

      /// <summary>Deletes a drive letter or mounted folder.</summary>
      /// If all the requested information is retrieved, the return value is nonzero.
      /// If not all the requested information is retrieved, the return value is zero.
      /// To get extended error information call Win32Exception()
      /// <remarks>Deleting a mounted folder does not cause the underlying directory to be deleted.
      /// It's not an error to attempt to unmount a volume from a volume mount point when there is no volume actually mounted at that volume mount point.
      /// </remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      /// <remarks>"lpszVolumeMountPoint" must end with a trailing backslash.</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "DeleteVolumeMountPointW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal extern static bool DeleteVolumeMountPoint([MarshalAs(UnmanagedType.LPWStr)] string lpszVolumeMountPoint);

      #endregion // Volume

      #endregion // Volume Management

      #region Disk Management

      /// <summary>Determines whether a disk drive is a removable, fixed, CD-ROM, RAM disk, or network drive.</summary>
      /// <returns>The return value specifies the type of drive, which can be one of the following <see cref="DriveType"/> values.</returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetDriveTypeW")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal extern static DriveType GetDriveType([MarshalAs(UnmanagedType.LPWStr)] string lpRootPathName);

      /// <summary>Retrieves information about the specified disk, including the amount of free space on the disk.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>
      /// Symbolic link behavior; if the path points to a symbolic link, the operation is performed on the target.
      /// </remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetDiskFreeSpaceW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool GetDiskFreeSpace([MarshalAs(UnmanagedType.LPWStr)] string lpRootPathName, out uint lpSectorsPerCluster, out uint lpBytesPerSector, out uint lpNumberOfFreeClusters, out uint lpTotalNumberOfClusters);

      /// <summary>Retrieves information about the amount of space that is available on a disk volume, which is the total amount of space,
      /// the total amount of free space, and the total amount of free space available to the user that is associated with the calling thread.
      /// </summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>
      /// Symbolic link behavior; if the path points to a symbolic link, the operation is performed on the target.
      /// </remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetDiskFreeSpaceExW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool GetDiskFreeSpaceEx([MarshalAs(UnmanagedType.LPWStr)] string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);

      /// <summary>Retrieves a bitmask representing the currently available disk drives.</summary>
      /// <returns>
      /// If the function succeeds, the return value is a bitmask representing the currently available disk drives.
      /// Bit position 0 (the least-significant bit) is drive A, bit position 1 is drive B, bit position 2 is drive C, and so on.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint GetLogicalDrives();

      #endregion // Disk Management

      #region Path

      #region GetFullPathName

      /// <summary>Retrieves the full path and file name of the specified file or directory.</summary>
      /// <remarks>The GetFullPathName function is not recommended for multithreaded applications or shared library code.</remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetFullPathNameW")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint GetFullPathName([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, [MarshalAs(UnmanagedType.U4)] uint nBufferLength, StringBuilder lpBuffer, IntPtr lpFilePart);

      /// <summary>Retrieves the full path and file name of the specified file or directory as a transacted operation.</summary>
      /// <remarks>Minimum supported client: Windows Vista</remarks>
      /// <remarks>Minimum supported server: Windows Server 2008</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetFullPathNameTransactedW")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint GetFullPathNameTransacted([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, [MarshalAs(UnmanagedType.U4)] uint nBufferLength, StringBuilder lpBuffer, IntPtr lpFilePart, SafeHandle hTransaction);

      #endregion // GetFullPathName

      #region GetLongPathName

      /// <summary>Converts the specified path to its long form.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetLongPathNameW")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint GetLongPathName([MarshalAs(UnmanagedType.LPWStr)] string lpszShortPath, StringBuilder lpszLongPath, [MarshalAs(UnmanagedType.U4)] uint cchBuffer);

      #endregion // GetLongPathName

      #region GetShortPathName

      /// <summary>Retrieves the short path form of the specified path.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetShortPathNameW")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint GetShortPathName([MarshalAs(UnmanagedType.LPWStr)] string lpszLongPath, StringBuilder lpszShortPath, [MarshalAs(UnmanagedType.U4)] uint cchBuffer);

      #endregion // GetShortPathName

      #region Path/Url Conversion

      /// <summary>Converts a file URL to a Microsoft MS-DOS path.</summary>
      /// <returns>Type: HRESULT
      /// If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
      /// </returns>
      /// <remarks>Minimum supported client: Windows 2000 Professional</remarks>
      /// <remarks>Minimum supported server: Windows 2000 Server</remarks>
      /// <remarks>This function returns a standard COM error value, so set "PreserveSig" to <see langref="false"/> to automatically convert HRESULT or retval values to exceptions.</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("shlwapi.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "PathCreateFromUrlW", PreserveSig = false)]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint PathCreateFromUrl([MarshalAs(UnmanagedType.LPWStr)] string pszUrl, StringBuilder pszPath, ref uint pcchPath, uint dwFlags);

      /// <summary>Creates a path from a file URL.</summary>
      /// <returns>Type: HRESULT
      /// If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
      /// </returns>
      /// <remarks>Minimum supported client: Windows Vista</remarks>
      /// <remarks>Minimum supported server: Windows Server 2008</remarks>
      /// <remarks>This function returns a standard COM error value, so set "PreserveSig" to <see langref="false"/> to automatically convert HRESULT or retval values to exceptions.</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("shlwapi.dll", SetLastError = true, CharSet = CharSet.Unicode, PreserveSig = false)]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint PathCreateFromUrlAlloc([MarshalAs(UnmanagedType.LPWStr)] string pszIn, ref StringBuilder pszPath, uint dwFlags);

      /// <summary>Converts a Microsoft MS-DOS path to a canonicalized URL.</summary>
      /// <returns>Type: HRESULT
      /// Returns S_FALSE if pszPath is already in URL format. In this case, pszPath will simply be copied to pszUrl.
      /// Otherwise, it returns S_OK if successful or a standard COM error value if not.
      /// </returns>
      /// <remarks>
      /// UrlCreateFromPath does not support extended paths. These are paths that include the extended-length path prefix "\\?\".
      /// </remarks>
      /// <remarks>Minimum supported client: Windows 2000 Professional</remarks>
      /// <remarks>Minimum supported server: Windows 2000 Server</remarks>
      /// <remarks>This function returns a standard COM error value, so set "PreserveSig" to <see langref="false"/> to automatically convert HRESULT or retval values to exceptions.</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("shlwapi.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "UrlCreateFromPathW", PreserveSig = false)]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint UrlCreateFromPath([MarshalAs(UnmanagedType.LPWStr)] string pszPath, StringBuilder pszUrl, ref uint pcchUrl, uint dwFlags);

      /// <summary>Tests whether a URL is a specified type.</summary>
      /// <returns>
      /// Type: BOOL
      /// For all but one of the URL types, UrlIs returns true if the URL is the specified type, or false if not.
      /// If UrlIs is set to <see cref="Shell32.UrlTypes.IsAppliable"/>, UrlIs will attempt to determine the URL scheme.
      /// If the function is able to determine a scheme, it returns true, or false otherwise.
      /// </returns>
      /// <remarks>Minimum supported client: Windows 2000 Professional</remarks>
      /// <remarks>Minimum supported server: Windows 2000 Server</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("shlwapi.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "UrlIsW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool UrlIs([MarshalAs(UnmanagedType.LPWStr)] string pszUrl, Shell32.UrlTypes urlIs);

      #endregion // Path/Url Conversion

      #endregion // Path

      #region Directory Management

      /// <summary>Creates a new directory. 
      /// If the underlying file system supports security on files and directories, the function applies a specified security descriptor to the new directory.
      /// </summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "CreateDirectoryW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal extern static bool CreateDirectory([MarshalAs(UnmanagedType.LPWStr)] string lpPathName, [MarshalAs(UnmanagedType.LPStruct)] SecurityAttributes lpSecurityAttributes);

      /// <summary>Creates a new directory with the attributes of a specified template directory.
      /// If the underlying file system supports security on files and directories, the function applies a specified security descriptor to the new directory.
      /// The new directory retains the other attributes of the specified template directory.
      /// </summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "CreateDirectoryExW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal extern static bool CreateDirectoryEx([MarshalAs(UnmanagedType.LPWStr)] string lpTemplateDirectory, [MarshalAs(UnmanagedType.LPWStr)] string lpPathName, [MarshalAs(UnmanagedType.LPStruct)] SecurityAttributes lpSecurityAttributes);

      /// <summary>Creates a new directory as a transacted operation, with the attributes of a specified template directory.
      /// If the underlying file system supports security on files and directories, the function applies a specified security descriptor to the new directory.
      /// The new directory retains the other attributes of the specified template directory.
      /// </summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows Vista</remarks>
      /// <remarks>Minimum supported server: Windows Server 2008</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "CreateDirectoryTransactedW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool CreateDirectoryTransacted([MarshalAs(UnmanagedType.LPWStr)] string lpTemplateDirectory, [MarshalAs(UnmanagedType.LPWStr)] string lpNewDirectory, [MarshalAs(UnmanagedType.LPStruct)] SecurityAttributes lpSecurityAttributes, SafeHandle hTransaction);

      /// <summary>Deletes an existing empty directory.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>
      /// RemoveDirectory removes a directory junction, even if the contents of the target are not empty; the function removes directory
      /// junctions regardless of the state of the target object. For more information on junctions, see Hard Links and Junctions.
      /// </remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "RemoveDirectoryW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal extern static bool RemoveDirectory([MarshalAs(UnmanagedType.LPWStr)] string lpPathName);

      /// <summary>Deletes an existing empty directory as a transacted operation.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>
      /// RemoveDirectory removes a directory junction, even if the contents of the target are not empty; the function removes directory
      /// junctions regardless of the state of the target object. For more information on junctions, see Hard Links and Junctions.
      /// </remarks>
      /// <remarks>Minimum supported client:Windows Vista</remarks>
      /// <remarks>Minimum supported server: Windows Server 2008</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "RemoveDirectoryTransactedW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool RemoveDirectoryTransacted([MarshalAs(UnmanagedType.LPWStr)] string lpPathName, SafeHandle hTransaction);

      #endregion // Directory Management

      #region File Management
      
      #region Mapping (View, not Network share)

      /// <summary>Creates or opens a named or unnamed file mapping object for a specified file.</summary>
      /// <returns>
      /// If the function succeeds, the return value is a handle to the newly created file mapping object.
      /// If the function fails, the return value is <see langword="null"/>.
      /// </returns>
      /// <remarks>SetLastError is set to false.</remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = false, CharSet = CharSet.Unicode, EntryPoint = "CreateFileMappingW")]
      internal static extern SafeFileHandle CreateFileMapping(SafeFileHandle hFile, SecurityAttributes lpSecurityAttributes, uint flProtect, [MarshalAs(UnmanagedType.U4)] uint dwMaximumSizeHigh, [MarshalAs(UnmanagedType.U4)] uint dwMaximumSizeLow, [MarshalAs(UnmanagedType.LPWStr)] string lpName);

      /// <summary>Checks whether the specified address is within a memory-mapped file in the address space of the specified process.
      /// If so, the function returns the name of the memory-mapped file.
      /// </summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>SetLastError is set to false.</remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("psapi.dll", SetLastError = false, CharSet = CharSet.Unicode, EntryPoint = "GetMappedFileNameW")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint GetMappedFileName(IntPtr hProcess, SafeLocalMemoryBufferHandle lpv, StringBuilder lpFilename, [MarshalAs(UnmanagedType.U4)] uint nSize);

      /// <summary>Maps a view of a file mapping into the address space of a calling process.</summary>
      /// <returns>
      /// If the function succeeds, the return value is the starting address of the mapped view.
      /// If the function fails, the return value is <see langword="null"/>.
      /// </returns>
      /// <remarks>SetLastError is set to false.</remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = false, CharSet = CharSet.Unicode)]
      internal static extern SafeLocalMemoryBufferHandle MapViewOfFile(SafeFileHandle hFileMappingObject, uint dwDesiredAccess, [MarshalAs(UnmanagedType.U4)] uint dwFileOffsetHigh, [MarshalAs(UnmanagedType.U4)] uint dwFileOffsetLow, UIntPtr dwNumberOfBytesToMap);

      /// <summary>Unmaps a mapped view of a file from the calling process's address space.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>SetLastError is set to false.</remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = false, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool UnmapViewOfFile(SafeLocalMemoryBufferHandle lpBaseAddress);

      /// <summary>Retrieves the final path for the specified file.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows Vista</remarks>
      /// <remarks>Minimum supported server: Windows Server 2008</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetFinalPathNameByHandleW")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint GetFinalPathNameByHandle(SafeFileHandle hFile, StringBuilder lpszFilePath, [MarshalAs(UnmanagedType.U4)] uint cchFilePath, FinalPathFormats dwFlags);

      #endregion // Mapping (View, not Network share)

      #region Type

      /// <summary>Retrieves the file type of the specified file.</summary>
      /// <returns>
      /// You can distinguish between a "valid" return of FILE_TYPE_UNKNOWN and its return due to a calling error
      /// (for example, passing an invalid handle to GetFileType) by calling Win32Exception().
      /// If the function worked properly and FILE_TYPE_UNKNOWN was returned, a call to GetLastError will return NO_ERROR.
      /// If the function returned FILE_TYPE_UNKNOWN due to an error in calling GetFileType, Win32Exception() will return the error code. 
      /// </returns>
      /// <remarks>
      /// "Don't let more than one process try to read from stdin at the same time."
      /// http://blogs.msdn.com/b/oldnewthing/archive/2011/12/02/10243553.aspx
      /// </remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern FileTypes GetFileType(SafeFileHandle hFile);

      /// <summary>Retrieves information about an object in the file system, such as a file, folder, directory, or drive root.</summary>
      /// <remarks>You should call this function from a background thread. Failure to do so could cause the UI to stop responding.</remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows 2000 Server</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "SHGetFileInfoW")]
      internal static extern UIntPtr SHGetFileInfo([MarshalAs(UnmanagedType.LPWStr)] string pszPath, FileAttributes dwFileAttributes, out Shell32.FileInfo psfi, [MarshalAs(UnmanagedType.U4)] uint cbFileInfo, Shell32.FileInfoAttributes uFileIconSize);

      /// <summary>Retrieves the name of and handle to the executable (.exe) file associated with a specific document file.
      /// This is the application that is launched when the document file is directly double-clicked or when Open is chosen from the file's shortcut menu.
      /// </summary>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows 2000 Server</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "FindExecutableW")]
      internal static extern SafeFindFileHandle FindExecutable([MarshalAs(UnmanagedType.LPWStr)] string lpFile, [MarshalAs(UnmanagedType.LPWStr)] string lpDirectory, StringBuilder lpResult);

      /// <summary>Searches for and retrieves a file or protocol association-related string from the registry.</summary>
      /// <returns>Return value Type: HRESULT. Returns a standard COM error value, including the following: S_OK, E_POINTER and S_FALSE.</returns>
      /// <remarks>Minimum supported client: Windows 2000 Professional</remarks>
      /// <remarks>Minimum supported server: Windows 2000 Server</remarks>
      /// <remarks>This function returns a standard COM error value, so set "PreserveSig" to <see langref="false"/> to automatically convert HRESULT or retval values to exceptions.</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("shlwapi.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "AssocQueryStringW", PreserveSig = false)]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint AssocQueryString(Shell32.AssociationAttributes flags, Shell32.AssociationString str, [MarshalAs(UnmanagedType.LPWStr)] string pszAssoc, [MarshalAs(UnmanagedType.LPWStr)] string pszExtra, StringBuilder pszOut, [MarshalAs(UnmanagedType.U4)] out uint pcchOut);

      #endregion // Type

      #region Copy/Move

      internal delegate CopyMoveProgressResult NativeCopyProgressRoutine(long totalFileSize, long totalBytesTransferred, long streamSize, long streamBytesTransferred, uint dwStreamNumber, CopyProgressCallbackReason dwCallbackReason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData);

      /// <summary>Copies an existing file to a new file, notifying the application of its progress through a callback function.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "CopyFileExW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool CopyFileEx([MarshalAs(UnmanagedType.LPWStr)] string lpExistingFileName, [MarshalAs(UnmanagedType.LPWStr)] string lpNewFileName, NativeCopyProgressRoutine lpProgressRoutine, IntPtr lpData, out int pbCancel, CopyOptions dwCopyFlags);

      /// <summary>Copies an existing file to a new file as a transacted operation, notifying the application of its progress through a callback function.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows Vista</remarks>
      /// <remarks>Minimum supported server: Windows Server 2008</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "CopyFileTransactedW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool CopyFileTransacted([MarshalAs(UnmanagedType.LPWStr)] string lpExistingFileName, [MarshalAs(UnmanagedType.LPWStr)] string lpNewFileName, NativeCopyProgressRoutine lpProgressRoutine, IntPtr lpData, out int pbCancel, CopyOptions dwCopyFlags, SafeHandle hTransaction);

      /// <summary>Moves a file or directory, including its children. You can provide a callback function that receives progress notifications.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "MoveFileWithProgressW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool MoveFileWithProgress([MarshalAs(UnmanagedType.LPWStr)] string existingFileName, [MarshalAs(UnmanagedType.LPWStr)] string newFileName, NativeCopyProgressRoutine lpProgressRoutine, IntPtr lpData, [MarshalAs(UnmanagedType.U4)] MoveOptions dwFlags);

      /// <summary>Moves an existing file or a directory, including its children, as a transacted operation.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows Vista</remarks>
      /// <remarks>Minimum supported server: Windows Server 2008</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "MoveFileTransactedW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool MoveFileTransacted([MarshalAs(UnmanagedType.LPWStr)] string lpExistingFileName, [MarshalAs(UnmanagedType.LPWStr)] string lpNewFileName, NativeCopyProgressRoutine lpProgressRoutine, IntPtr lpData, [MarshalAs(UnmanagedType.U4)] MoveOptions dwCopyFlags, SafeHandle hTransaction);

      #endregion // Copy/Move

      #region Create

      /// <summary>Creates or opens a file or I/O device. The most commonly used I/O devices are as follows: file, file stream, directory, physical disk, volume, console buffer, tape drive, communications resource, mailslot, and pipe.</summary>
      /// <returns>
      /// If the function succeeds, the return value is an open handle to the specified file, device, named pipe, or mail slot.
      /// If the function fails, the return value is <see cref="Win32Errors.ERROR_INVALID_HANDLE"/>. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "CreateFileW")]
      internal static extern SafeFileHandle CreateFile([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, [MarshalAs(UnmanagedType.U4)] FileSystemRights dwDesiredAccess, [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode, SecurityAttributes lpSecurityAttributes, [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition, [MarshalAs(UnmanagedType.U4)] EFileAttributes dwFlagsAndAttributes, SafeHandle hTemplateFile);

      /// <summary>Creates or opens a file or I/O device. The most commonly used I/O devices are as follows: file, file stream, directory, physical disk, volume, console buffer, tape drive, communications resource, mailslot, and pipe.</summary>
      /// <returns>
      /// If the function succeeds, the return value is an open handle to the specified file, device, named pipe, or mail slot.
      /// If the function fails, the return value is <see cref="Win32Errors.ERROR_INVALID_HANDLE"/>. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "CreateFileTransactedW")]
      internal static extern SafeFileHandle CreateFileTransacted([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, [MarshalAs(UnmanagedType.U4)] FileSystemRights dwDesiredAccess, [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode, SecurityAttributes lpSecurityAttributes, [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition, [MarshalAs(UnmanagedType.U4)] EFileAttributes dwFlagsAndAttributes, SafeHandle hTemplateFile, SafeHandle hTransaction, IntPtr pusMiniVersion, IntPtr pExtendedParameter);

      #endregion // Create

      #region Delete

      /// <summary>Deletes an existing file.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero (0). To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "DeleteFileW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool DeleteFile([MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

      /// <summary>Deletes an existing file as a transacted operation.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero (0). To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows Vista</remarks>
      /// <remarks>Minimum supported server: Windows Server 2008</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "DeleteFileTransactedW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool DeleteFileTransacted([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, SafeHandle hTransaction);

      #endregion // Delete

      #region Links

      /// <summary>Establishes a hard link between an existing file and a new file. 
      /// This function is only supported on the NTFS file system, and only for files, not directories.
      /// </summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero (0). To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "CreateHardLinkW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool CreateHardLink([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, [MarshalAs(UnmanagedType.LPWStr)] string lpExistingFileName, IntPtr lpSecurityAttributes);

      /// <summary>Establishes a hard link between an existing file and a new file as a transacted operation.
      /// This function is only supported on the NTFS file system, and only for files, not directories.
      /// </summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero (0). To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "CreateHardLinkTransactedW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool CreateHardLinkTransacted([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, [MarshalAs(UnmanagedType.LPWStr)] string lpExistingFileName, IntPtr lpSecurityAttributes, SafeHandle hTransaction);

      /// <summary>Creates a symbolic link.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero (0). To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows Vista</remarks>
      /// <remarks>Minimum supported server: Windows Server 2008</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "CreateSymbolicLinkW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool CreateSymbolicLink([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, [MarshalAs(UnmanagedType.LPWStr)] string lpExistingFileName, [MarshalAs(UnmanagedType.U4)] SymbolicLinkTarget dwFlags);

      /// <summary>Creates a symbolic link as a transacted operation.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero (0). To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows Vista</remarks>
      /// <remarks>Minimum supported server: Windows Server 2008</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "CreateSymbolicLinkTransactedW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool CreateSymbolicLinkTransacted([MarshalAs(UnmanagedType.LPWStr)] string lpSymlinkFileName, [MarshalAs(UnmanagedType.LPWStr)] string lpTargetFileName, [MarshalAs(UnmanagedType.U4)] SymbolicLinkTarget dwFlags, SafeHandle hTransaction);

      #endregion // Links

      #region Find

      /// <summary>Searches a directory for a file or subdirectory with a name and attributes that match those specified.</summary>
      /// <returns>
      /// If the function succeeds, the return value is a search handle used in a subsequent call to FindNextFile or FindClose, and the lpFindFileData parameter contains information about the first file or directory found.
      /// If the function fails or fails to locate files from the search string in the lpFileName parameter, the return value is INVALID_HANDLE_VALUE and the contents of lpFindFileData are indeterminate. To get extended error information, call the GetLastError function.
      /// </returns>
      /// <remarks>A trailing backslash is not allowed and will be removed.</remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "FindFirstFileExW")]
      internal static extern SafeFindFileHandle FindFirstFileEx([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, FindExInfoLevels fInfoLevelId, ref Win32FindData lpFindFileData, FindExSearchOps fSearchOp, IntPtr lpSearchFilter, FindExAdditionalFlags dwAdditionalFlags);

      /// <summary>Searches a directory for a file or subdirectory with a name that matches a specific name as a transacted operation.</summary>
      /// <returns>
      /// If the function succeeds, the return value is a search handle used in a subsequent call to FindNextFile or FindClose, and the lpFindFileData parameter contains information about the first file or directory found.
      /// If the function fails or fails to locate files from the search string in the lpFileName parameter, the return value is INVALID_HANDLE_VALUE and the contents of lpFindFileData are indeterminate. To get extended error information, call the GetLastError function.
      /// </returns>
      /// <remarks>A trailing backslash is not allowed and will be removed.</remarks>
      /// <remarks>Minimum supported client: Windows Vista</remarks>
      /// <remarks>Minimum supported server: Windows Server 2008</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "FindFirstFileTransactedW")]
      internal static extern SafeFindFileHandle FindFirstFileTransacted([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, FindExInfoLevels fInfoLevelId, ref Win32FindData lpFindFileData, FindExSearchOps fSearchOp, IntPtr lpSearchFilter, FindExAdditionalFlags dwAdditionalFlags, SafeHandle hTransaction);

      /// <summary>Continues a file search from a previous call to the FindFirstFile, FindFirstFileEx, or FindFirstFileTransacted functions.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero and the lpFindFileData parameter contains information about the next file or directory found.
      /// If the function fails, the return value is zero and the contents of lpFindFileData are indeterminate. To get extended error information, call the GetLastError function.
      /// If the function fails because no more matching files can be found, the GetLastError function returns ERROR_NO_MORE_FILES.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "FindNextFileW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool FindNextFile(SafeFindFileHandle hFindFile, ref Win32FindData lpFindFileData);

      /// <summary>Closes a file search handle opened by the FindFirstFile, FindFirstFileEx, FindFirstFileNameW, FindFirstFileNameTransactedW, FindFirstFileTransacted, FindFirstStreamTransactedW, or FindFirstStreamW functions.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>SetLastError is set to false.</remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = false, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool FindClose(IntPtr hFindFile);

      /// <summary>Creates an enumeration of all the hard links to the specified file. 
      /// The FindFirstFileNameW function returns a handle to the enumeration that can be used on subsequent calls to the FindNextFileNameW function.
      /// </summary>
      /// <returns>
      /// If the function succeeds, the return value is a search handle that can be used with the FindNextFileNameW function or closed with the FindClose function.
      /// If the function fails, the return value is INVALID_HANDLE_VALUE (0xffffffff). To get extended error information, call the GetLastError function.
      /// </returns>
      /// <remarks>Minimum supported client: Windows Vista</remarks>
      /// <remarks>Minimum supported server: Windows Server 2008</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "FindFirstFileNameW")]
      internal static extern SafeFindFileHandle FindFirstFileName([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, uint dwFlags, [In, Out] ref uint stringLength, StringBuilder linkName);

      /// <summary>Creates an enumeration of all the hard links to the specified file as a transacted operation. The function returns a handle to the enumeration that can be used on subsequent calls to the FindNextFileNameW function.</summary>
      /// <returns>
      /// If the function succeeds, the return value is a search handle that can be used with the FindNextFileNameW function or closed with the FindClose function.
      /// If the function fails, the return value is INVALID_HANDLE_VALUE (0xffffffff). To get extended error information, call the GetLastError function.
      /// </returns>
      /// <remarks>Minimum supported client: Windows Vista</remarks>
      /// <remarks>Minimum supported server: Windows Server 2008</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "FindFirstFileNameTransactedW")]
      internal static extern SafeFindFileHandle FindFirstFileNameTransacted([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, int dwFlags, [In, Out] ref uint stringLength, StringBuilder linkName, SafeHandle hTransaction);

      /// <summary>Continues enumerating the hard links to a file using the handle returned by a successful call to the FindFirstFileNameW function.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero (0). To get extended error information, call GetLastError.
      /// If no matching files can be found, the GetLastError function returns ERROR_HANDLE_EOF.
      /// </returns>
      /// <remarks>Minimum supported client: Windows Vista</remarks>
      /// <remarks>Minimum supported server: Windows Server 2008</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "FindNextFileNameW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool FindNextFileName(SafeHandle hFindStream, [In, Out] ref uint stringLength, [In, Out] StringBuilder linkName);

      /// <summary>Determines whether a path to a file system object such as a file or folder is valid.</summary>
      /// <returns>true if the file exists; otherwise, false. Call GetLastError for extended error information.</returns>
      /// <remarks>
      /// This function tests the validity of the path.
      /// A path specified by Universal Naming Convention (UNC) is limited to a file only; that is, \\server\share\file is permitted.
      /// A network share path to a server or server share is not permitted; that is, \\server or \\server\share.
      /// This function returns FALSE if a mounted remote drive is out of service.
      /// </remarks>
      /// <remarks>Minimum supported client: Windows 2000 Professional</remarks>
      /// <remarks>Minimum supported server: Windows 2000 Server</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "PathFileExistsW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool PathFileExists([MarshalAs(UnmanagedType.LPWStr)] string pszPath);

      #endregion // Find

      /// <summary>Retrieves attributes for a specified file or directory.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>
      /// The GetFileAttributes function retrieves file system attribute information. GetFileAttributesEx can obtain other sets of file or directory attribute information.
      /// Currently, GetFileAttributesEx retrieves a set of standard attributes that is a superset of the file system attribute information.
      /// When the GetFileAttributesEx function is called on a directory that is a mounted folder, it returns the attributes of the directory, not those of
      /// the root directory in the volume that the mounted folder associates with the directory. To obtain the attributes of the associated volume,
      /// call GetVolumeNameForVolumeMountPoint to obtain the name of the associated volume. Then use the resulting name in a call to GetFileAttributesEx.
      /// The results are the attributes of the root directory on the associated volume.
      /// Symbolic link behavior: If the path points to a symbolic link, the function returns attributes for the symbolic link.
      /// </remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetFileAttributesExW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool GetFileAttributesEx([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, [MarshalAs(UnmanagedType.U4)] uint fInfoLevelId, ref Win32FileAttributeData lpFileInformation);

      /// <summary>Retrieves file system attributes for a specified file or directory as a transacted operation.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>
      /// Transacted Operations
      /// If a file is open for modification in a transaction, no other thread can open the file for modification until the transaction is committed.
      /// Conversely, if a file is open for modification outside of a transaction, no transacted thread can open the file for modification until the
      /// non-transacted handle is closed. If a non-transacted thread has a handle opened to modify a file, a call to GetFileAttributesTransacted for
      /// that file will fail with an ERROR_TRANSACTIONAL_CONFLICT error.
      /// </remarks>
      /// <remarks>Minimum supported client: Windows Vista</remarks>
      /// <remarks>Minimum supported server: Windows Server 2008</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetFileAttributesTransactedW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool GetFileAttributesTransacted([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, [MarshalAs(UnmanagedType.U4)] uint fInfoLevelId, ref Win32FileAttributeData lpFileInformation, SafeHandle hTransaction);

      /// <summary>Replaces one file with another file, with the option of creating a backup copy of the original file. The replacement file assumes the name of the replaced file and its identity.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "ReplaceFileW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool ReplaceFile([MarshalAs(UnmanagedType.LPWStr)] string lpReplacedFileName, [MarshalAs(UnmanagedType.LPWStr)] string lpReplacementFileName, [MarshalAs(UnmanagedType.LPWStr)] string lpBackupFileName, FileSystemRights dwReplaceFlags, IntPtr lpExclude, IntPtr lpReserved);

      /// <summary>Sets the attributes for a file or directory.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [SuppressMessage("Microsoft.Usage", "CA2205:UseManagedEquivalentsOfWin32Api", Justification = "2012-09-26: Yomodo; Managed version can't handle LongPaths.")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "SetFileAttributesW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool SetFileAttributes([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, [MarshalAs(UnmanagedType.U4)] FileAttributes dwFileAttributes);

      /// <summary>Sets the attributes for a file or directory as a transacted operation.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows Vista</remarks>
      /// <remarks>Minimum supported server: Windows Server 2008</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "SetFileAttributesTransactedW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool SetFileAttributesTransacted([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, [MarshalAs(UnmanagedType.U4)] FileAttributes dwFileAttributes, SafeHandle hTransaction);

      /// <summary>Sets the date and time that the specified file or directory was created, last accessed, or last modified.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool SetFileTime(SafeFileHandle hFile, SafeHandle lpCreationTime, SafeHandle lpLastAccessTime, SafeHandle lpLastWriteTime);

      /// <summary>Retrieves the size of the specified file.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool GetFileSizeEx(SafeHandle hFile, out long lpFileSize);

      /// <summary>Retrieves the actual number of bytes of disk storage used to store a specified file.</summary>
      /// <returns>
      /// If the function succeeds, the return value is the low-order DWORD of the actual number of bytes of disk storage used to store the specified file, and if lpFileSizeHigh is non-NULL, the function puts the high-order DWORD of that actual value into the DWORD pointed to by that parameter. This is the compressed file size for compressed files, the actual file size for noncompressed files.
      /// If the function fails, and lpFileSizeHigh is NULL, the return value is INVALID_FILE_SIZE. To get extended error information, call GetLastError.
      /// If the return value is INVALID_FILE_SIZE and lpFileSizeHigh is non-NULL, an application must call GetLastError to determine whether the function has succeeded (value is NO_ERROR) or failed (value is other than NO_ERROR).
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetCompressedFileSizeW")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint GetCompressedFileSize([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, [MarshalAs(UnmanagedType.U4)] out uint lpFileSizeHigh);

      /// <summary>Retrieves the actual number of bytes of disk storage used to store a specified file as a transacted operation.</summary>
      /// <returns>
      /// If the function succeeds, the return value is the low-order DWORD of the actual number of bytes of disk storage used to store the specified file, and if lpFileSizeHigh is non-NULL, the function puts the high-order DWORD of that actual value into the DWORD pointed to by that parameter. This is the compressed file size for compressed files, the actual file size for noncompressed files.
      /// If the function fails, and lpFileSizeHigh is NULL, the return value is INVALID_FILE_SIZE. To get extended error information, call GetLastError.
      /// If the return value is INVALID_FILE_SIZE and lpFileSizeHigh is non-NULL, an application must call GetLastError to determine whether the function has succeeded (value is NO_ERROR) or failed (value is other than NO_ERROR).
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetCompressedFileSizeTransactedW")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint GetCompressedFileSizeTransacted([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, [MarshalAs(UnmanagedType.U4)] out uint lpFileSizeHigh, SafeHandle hTransaction);

      /// <summary>Flushes the buffers of a specified file and causes all buffered data to be written to a file.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool FlushFileBuffers(SafeFileHandle hFile);

      /// <summary>Locks the specified file for exclusive access by the calling process.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero (TRUE).
      /// If the function fails, the return value is zero (FALSE). To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool LockFile(SafeFileHandle hFile, [MarshalAs(UnmanagedType.U4)] uint dwFileOffsetLow, [MarshalAs(UnmanagedType.U4)] uint dwFileOffsetHigh, [MarshalAs(UnmanagedType.U4)] uint nNumberOfBytesToLockLow, [MarshalAs(UnmanagedType.U4)] uint nNumberOfBytesToLockHigh);

      /// <summary>Unlocks a region in an open file. Unlocking a region enables other processes to access the region.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool UnlockFile(SafeFileHandle hFile, [MarshalAs(UnmanagedType.U4)] uint dwFileOffsetLow, [MarshalAs(UnmanagedType.U4)] uint dwFileOffsetHigh, [MarshalAs(UnmanagedType.U4)] uint nNumberOfBytesToUnlockLow, [MarshalAs(UnmanagedType.U4)] uint nNumberOfBytesToUnlockHigh);

      /// <summary>Retrieves file information for the specified file.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero and file information data is contained in the buffer pointed to by the lpFileInformation parameter.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>
      /// Depending on the underlying network features of the operating system and the type of server connected to,
      /// the GetFileInformationByHandle function may fail, return partial information, or full information for the given file.
      /// </remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool GetFileInformationByHandle(SafeFileHandle hFile, ByHandleFileInfo lpFileInformation);

      /// <summary>Retrieves file information for the specified file.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero and file information data is contained in the buffer pointed to by the lpFileInformation parameter.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows Vista</remarks>
      /// <remarks>Minimum supported server: Windows Server 2008</remarks>
      /// <remarks>Redistributable: Windows SDK on Windows Server 2003 and Windows XP.</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool GetFileInformationByHandleEx(SafeFileHandle hFile, [MarshalAs(UnmanagedType.U4)] uint fileInformationClass, SafeHandle lpFileInformation, [MarshalAs(UnmanagedType.U4)] uint dwBufferSize);

      #endregion // File Management

      # region Compress/Decompress

      /// <summary>Sends a control code directly to a specified device driver, causing the corresponding device to perform the corresponding operation.</summary>
      /// <returns>
      /// If the operation completes successfully, the return value is nonzero.
      /// If the operation fails or is pending, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool DeviceIoControl(SafeFileHandle hDevice, IoControlCode dwIoControlCode, ref short compressionFormatDefault, [MarshalAs(UnmanagedType.U4)] uint nInBufferSize, IntPtr lpOutBuffer, [MarshalAs(UnmanagedType.U4)] uint nOutBufferSize, [MarshalAs(UnmanagedType.U4)] out uint lpBytesReturned, IntPtr lpOverlapped);

      #endregion // Compress/Decompress

      #region Encrypt/Decryption

      /// <summary>Encrypts a file or directory. All data streams in a file are encrypted. All new files created in an encrypted directory are encrypted.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>
      /// The EncryptFile function requires exclusive access to the file being encrypted, and will fail if another process is using the file.
      /// If the file is already encrypted, EncryptFile simply returns a nonzero value, which indicates success. If the file is compressed,
      /// EncryptFile will decompress the file before encrypting it. If lpFileName specifies a read-only file, the function fails and GetLastError
      /// returns ERROR_FILE_READ_ONLY. If lpFileName specifies a directory that contains a read-only file, the functions succeeds but the directory is not encrypted.
      /// </remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "EncryptFileW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool EncryptFile([MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

      /// <summary>Decrypts an encrypted file or directory.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>
      /// The DecryptFile function requires exclusive access to the file being decrypted, and will fail if another process is using the file.
      /// If the file is not encrypted, DecryptFile simply returns a nonzero value, which indicates success.
      /// If lpFileName specifies a read-only file, the function fails and GetLastError returns ERROR_FILE_READ_ONLY.
      /// If lpFileName specifies a directory that contains a read-only file, the functions succeeds but the directory is not decrypted.
      /// </remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "DecryptFileW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool DecryptFile([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, uint dwReserved);

      /// <summary>Disables or enables encryption of the specified directory and the files in it.
      /// It does not affect encryption of subdirectories below the indicated directory. 
      /// </summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>
      /// EncryptionDisable disables encryption of directories and files.
      /// It does not affect the visibility of files with the FILE_ATTRIBUTE_SYSTEM attribute set.
      /// This method will create/change the file "Desktop.ini" and wil set Encryption value: "Disable=0|1"
      /// </remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool EncryptionDisable([MarshalAs(UnmanagedType.LPWStr)] string dirPath, [MarshalAs(UnmanagedType.Bool)] bool disable);

      /// <summary>Retrieves the encryption status of the specified file.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "FileEncryptionStatusW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool FileEncryptionStatus([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, out FileEncryptionStatus lpStatus);

      #endregion // Encrypt/Decryption

      #region Security

      #region Get/SetFileSecurity

      // 2012-10-15: Yomodo; Moved to Security\NativeMethods.cs since more Security-related stuff is in there.

      ///// <summary>
      ///// The GetFileSecurity function obtains specified information about the security of a file or directory.
      ///// The information obtained is constrained by the caller's access rights and privileges.
      ///// </summary>
      ///// <returns>
      ///// If the function succeeds, the return value is nonzero.
      ///// If the function fails, it returns zero. To get extended error information, call GetLastError.
      ///// </returns>
      ///// <remarks>Minimum supported client: Windows XP</remarks>
      ///// <remarks>Minimum supported server: Windows Server 2003</remarks>
      //[DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetFileSecurityW")]
      //[return: MarshalAs(UnmanagedType.Bool)]
      //internal static extern bool GetFileSecurity([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, uint requestedInformation, SafeHandle pSecurityDescriptor, uint nLength, out uint lpnLengthNeeded);

      ///// <summary>
      ///// The SetFileSecurity function sets the security of a file or directory object.
      ///// </summary>
      ///// <returns>
      ///// If the function succeeds, the return value is nonzero.
      ///// If the function fails, it returns zero. To get extended error information, call GetLastError.
      ///// </returns>
      ///// <remarks>Minimum supported client: Windows XP</remarks>
      ///// <remarks>Minimum supported server: Windows Server 2003</remarks>
      ///// <remarks>This function is obsolete. Use the SetNamedSecurityInfo function instead.</remarks>
      //[DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "SetFileSecurityW")]
      //[return: MarshalAs(UnmanagedType.Bool)]
      //internal static extern bool SetFileSecurity([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, Security.NativeMethods.SECURITY_INFORMATION securityInformation, SafeHandle pSecurityDescriptor);

      #endregion // Get/SetFileSecurity

      #region Get/SetNamedSecurityInfo

      ///// <summary>
      ///// The GetNamedSecurityInfo function retrieves a copy of the security descriptor for an object specified by name.
      ///// </summary>
      ///// <returns>
      ///// If the function succeeds, the return value is ERROR_SUCCESS.
      ///// If the function fails, the return value is a nonzero error code defined in WinError.h.
      ///// </returns>
      ///// <remarks>Minimum supported client: Windows XP</remarks>
      ///// <remarks>Minimum supported server: Windows Server 2003</remarks>
      //[DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetNamedSecurityInfoW")]
      //[return: MarshalAs(UnmanagedType.Bool)]
      //internal static extern bool GetNamedSecurityInfo([MarshalAs(UnmanagedType.LPWStr)] string pObjectName, Security.NativeMethods.SE_OBJECT_TYPE objectType, Security.NativeMethods.SECURITY_INFORMATION securityInfo, out IntPtr ppsidOwner, out IntPtr ppsidGroup, out IntPtr ppDacl, out IntPtr ppSacl, out SafeLocalMemoryBufferHandle ppSecurityDescriptor);

      ///// <summary>
      ///// The SetNamedSecurityInfo function sets specified security information in the security descriptor of a specified object. The caller identifies the object by name.
      ///// </summary>
      ///// <returns>
      ///// If the function succeeds, the function returns ERROR_SUCCESS.
      ///// If the function fails, it returns a nonzero error code defined in WinError.h.
      ///// </returns>
      ///// <remarks>Minimum supported client: Windows XP</remarks>
      ///// <remarks>Minimum supported server: Windows Server 2003</remarks>
      //[DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "SetNamedSecurityInfoW")]
      //[return: MarshalAs(UnmanagedType.Bool)]
      //internal static extern bool SetNamedSecurityInfo([MarshalAs(UnmanagedType.LPWStr)] string pObjectName, Security.NativeMethods.SE_OBJECT_TYPE objectType, Security.NativeMethods.SECURITY_INFORMATION securityInfo, out IntPtr psidOwner, out IntPtr psidGroup, out IntPtr pDacl, out IntPtr pSacl);

      #endregion Get/SetNamedSecurityInfo

      #endregion // Security

      #region Backup

      /// <summary>The BackupRead function can be used to back up a file or directory, including the security information.
      /// The function reads data associated with a specified file or directory into a buffer, which can then be written to the backup medium using the WriteFile function.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero, indicating that an I/O error occurred. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>This function is not intended for use in backing up files encrypted under the Encrypted File System. Use ReadEncryptedFileRaw for that purpose.</remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool BackupRead(SafeFileHandle hFile, SafeGlobalMemoryBufferHandle lpBuffer, [MarshalAs(UnmanagedType.U4)] uint nNumberOfBytesToRead, [MarshalAs(UnmanagedType.U4)] out uint lpNumberOfBytesRead, [MarshalAs(UnmanagedType.Bool)] bool bAbort, [MarshalAs(UnmanagedType.Bool)] bool bProcessSecurity, out IntPtr lpContext);

      /// <summary>The BackupWrite function can be used to restore a file or directory that was backed up using BackupRead.
      /// Use the ReadFile function to get a stream of data from the backup medium, then use BackupWrite to write the data to the specified file or directory.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero, indicating that an I/O error occurred. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>This function is not intended for use in restoring files encrypted under the Encrypted File System. Use WriteEncryptedFileRaw for that purpose.</remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool BackupWrite(SafeFileHandle hFile, SafeGlobalMemoryBufferHandle lpBuffer, [MarshalAs(UnmanagedType.U4)] uint nNumberOfBytesToWrite, [MarshalAs(UnmanagedType.U4)] out uint lpNumberOfBytesWritten, [MarshalAs(UnmanagedType.Bool)] bool bAbort, [MarshalAs(UnmanagedType.Bool)] bool bProcessSecurity, out IntPtr lpContext);

      /// <summary>The BackupSeek function seeks forward in a data stream initially accessed by using the BackupRead or BackupWrite function.</summary>
      /// <returns>
      /// If the function could seek the requested amount, the function returns a nonzero value.
      /// If the function could not seek the requested amount, the function returns zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>
      /// Applications use the BackupSeek function to skip portions of a data stream that cause errors.
      /// This function does not seek across stream headers. For example, this function cannot be used to skip the stream name.
      /// If an application attempts to seek past the end of a substream, the function fails, the lpdwLowByteSeeked and lpdwHighByteSeeked parameters
      /// indicate the actual number of bytes the function seeks, and the file position is placed at the start of the next stream header.
      /// </remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool BackupSeek(SafeFileHandle hFile, [MarshalAs(UnmanagedType.U4)] uint dwLowBytesToSeek, [MarshalAs(UnmanagedType.U4)] uint dwHighBytesToSeek, [MarshalAs(UnmanagedType.U4)] out uint lpdwLowBytesSeeked, [MarshalAs(UnmanagedType.U4)] out uint lpdwHighBytesSeeked, out IntPtr lpContext);

      #endregion // Backup

      #region Kernel Transaction Manager

      #region CommitTransaction

      /// <summary>Requests that the specified transaction be committed.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is 0 (zero). To get extended error information, call the GetLastError function.
      /// </returns>
      /// <remarks>Minimum supported client: Windows Vista</remarks>
      /// <remarks>Minimum supported server: Windows Server 2008</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("ktmw32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool CommitTransaction(SafeHandle hTrans);

      #endregion // CommitTransaction

      #region CreateTransaction

      /// <summary>Creates a new transaction object.</summary>
      /// <returns>
      /// If the function succeeds, the return value is a handle to the transaction. 
      /// If the function fails, the return value is INVALID_HANDLE_VALUE. To get extended error information, call the GetLastError function.
      /// </returns>
      /// <remarks>Minimum supported client: Windows Vista</remarks>
      /// <remarks>Minimum supported server: Windows Server 2008</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("ktmw32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      internal static extern SafeKernelTransactionHandle CreateTransaction(SecurityAttributes lpTransactionAttributes, IntPtr uow, uint createOptions, uint isolationLevel, uint isolationFlags, uint timeout, string description);

      #endregion // CreateTransaction

      #region RollbackTransaction

      /// <summary>Requests that the specified transaction be rolled back. This function is synchronous.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call the GetLastError function.
      /// </returns>
      /// <remarks>Minimum supported client: Windows Vista</remarks>
      /// <remarks>Minimum supported server: Windows Server 2008</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("ktmw32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool RollbackTransaction(SafeHandle hTrans);

      #endregion // RollbackTransaction

      #endregion // Kernel Transaction Manager

      #region Handles and Objects

      #region CloseHandle

      /// <summary>Closes an open object handle.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows 2000 Professional</remarks>
      /// <remarks>Minimum supported server: Windows 2000 Server</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool CloseHandle(IntPtr hObject);

      #endregion // CloseHandle

      #endregion // Handles and Objects

      #region Types

      #region ReparsePointTags

      /// <summary>Enumeration specifying the different reparse point tags.</summary>
      internal enum ReparsePointTags : uint
      {
         /// <summary>The entry is not a reparse point.</summary>
         None = 0,

         ///// <summary>IO_REPARSE_TAG_DFS</summary>
         //Dfs = 2147483658,

         ///// <summary>IO_REPARSE_TAG_DFSR</summary>
         //Dfsr = 2147483666,

         /// <summary>IO_REPARSE_TAG_MOUNT_POINT - Used for mount point support.</summary>
         MountPoint = 2684354563,

         /// <summary>IO_REPARSE_TAG_SYMLINK - Used for symbolic link support.</summary>
         SymLink = 2684354572
      }

      #endregion // ReparsePointTags

      #region FindEx Constants

      /// <summary>Defines values that are used with the FindFirstFileEx function to specify the information level of the returned data.</summary>
      internal enum FindExInfoLevels
      {
         /// <summary>The FindFirstFileEx function retrieves a standard set of attribute information.</summary>
         /// <remarks>The data is returned in a <see cref="Win32FindData"/> structure.</remarks>
         InfoStandard,

         /// <summary>The FindFirstFileEx function does not query the short file name, improving overall enumeration speed.</summary>
         /// <remarks>The data is returned in a <see cref="Win32FindData"/> structure, and cAlternateFileName member is always a NULL string.</remarks>
         /// <remarks>This value is not supported until Windows Server 2008 R2 and Windows 7.</remarks>
         InfoBasic,

         /// <summary>This value is used for validation. Supported values are less than this value.</summary>
         InfoMaxLevel
      }

      /// <summary>Defines values that are used with the FindFirstFileEx function to specify the type of filtering to perform.</summary>
      internal enum FindExSearchOps
      {
         /// <summary>The search for a file that matches a specified file name.</summary>
         SearchNameMatch,

         /// <summary>This is an advisory flag. If the file system supports directory filtering, the function searches for a file that matches
         /// the specified name and is also a directory. If the file system does not support directory filtering, this flag is silently ignored. 
         /// </summary>
         /// <remarks>
         /// The lpSearchFilter parameter of the FindFirstFileEx function must be NULL when this search value is used.
         /// If directory filtering is desired, this flag can be used on all file systems, but because it is an advisory flag and only affects file systems that support it,
         /// the application must examine the file attribute data stored in the lpFindFileData parameter of the FindFirstFileEx function to determine whether the function has returned a handle to a directory.
         /// </remarks>
         SearchLimitToDirectories,

         /// <summary>This filtering type is not available.</summary>
         SearchLimitToDevices
      }

      /// <summary>Additional flags that control the search.</summary>
      [Flags]
      internal enum FindExAdditionalFlags
      {
         /// <summary>No additional flags used.</summary>
         None = 0,

         /// <summary>Searches are case-sensitive.</summary>
         CaseSensitive = 1,

         /// <summary>Uses a larger buffer for directory queries, which can increase performance of the find operation.</summary>
         /// <remarks>This value is not supported until Windows Server 2008 R2 and Windows 7.</remarks>
         LargeFetch = 2
      }

      #endregion // FindEx Constants

      #region FileIdBothDirInfo

      /// <summary>Contains information about files in the specified directory. Used for directory handles. Use only when calling GetFileInformationByHandleEx.</summary>
      /// <remarks>
      /// The number of files that are returned for each call to GetFileInformationByHandleEx depends on the size of the buffer that is passed to the function.
      /// Any subsequent calls to GetFileInformationByHandleEx on the same handle will resume the enumeration operation after the last file is returned.
      /// </remarks>
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      internal struct FileIdBothDirInfo
      {
         /// <summary>The offset for the next FILE_ID_BOTH_DIR_INFO structure that is returned. Contains zero (0) if no other entries follow this one.</summary>
         public readonly uint NextEntryOffset;

         /// <summary>The byte offset of the file within the parent directory. This member is undefined for file systems, such as NTFS,
         /// in which the position of a file within the parent directory is not fixed and can be changed at any time to maintain sort order.
         /// </summary>
         public readonly uint FileIndex;

         /// <summary>The time that the file was created.</summary>
         public FileTime CreationTime;

         /// <summary>The time that the file was last accessed.</summary>
         public FileTime LastAccessTime;

         /// <summary>The time that the file was last written to.</summary>
         public FileTime LastWriteTime;

         /// <summary>The time that the file was last changed.</summary>
         public FileTime ChangeTime;

         /// <summary>The absolute new end-of-file position as a byte offset from the start of the file to the end of the file.
         /// Because this value is zero-based, it actually refers to the first free byte in the file.
         /// In other words, EndOfFile is the offset to the byte that immediately follows the last valid byte in the file.
         /// </summary>
         public readonly long EndOfFile;

         /// <summary>The number of bytes that are allocated for the file. This value is usually a multiple of the sector or cluster size of the underlying physical device.</summary>
         public readonly long AllocationSize;

         /// <summary>The file attributes.</summary>
         public readonly FileAttributes FileAttributes;

         /// <summary>The length of the file name.</summary>
         public readonly uint FileNameLength;

         /// <summary>The size of the extended attributes for the file.</summary>
         public readonly uint EaSize;

         /// <summary>The length of ShortName.</summary>
         public readonly byte ShortNameLength;

         /// <summary>The short 8.3 file naming convention (for example, "FILENAME.TXT") name of the file.</summary>
         [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12, ArraySubType = UnmanagedType.U2)]
         public readonly char[] ShortName;

         /// <summary>The file ID.</summary>
         public readonly long FileId;

         /// <summary>The first character of the file name string. This is followed in memory by the remainder of the string.</summary>
         public IntPtr FileName;
      }

      #endregion // FileIdBothDirInfo

      #region SecurityAttributes

      /// <summary>Class used to represent the SECURITY_ATTRIBUES native win32 structure. It provides initialization function from an <see cref="ObjectSecurity"/> object.</summary>
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      internal sealed class SecurityAttributes
      {
         /// <summary>Initializes the SecurityAttributes structure from an instance of <see cref="ObjectSecurity"/>.</summary>
         /// <param name="memoryHandle">A handle that will refer to the memory allocated by this object for storage of the 
         /// security descriptor. As long as this object is used, the memory handle should be kept alive, and afterwards it
         /// should be disposed as soon as possible.</param>
         /// <param name="securityDescriptor">The <see cref="ObjectSecurity"/> security descriptor to assign to this object. This parameter may be <see langword="null"/>.</param>
         [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
         public void Initialize(out SafeGlobalMemoryBufferHandle memoryHandle, ObjectSecurity securityDescriptor)
         {
            //nLength = (uint) Marshal.SizeOf(this);

            if (securityDescriptor == null)
               memoryHandle = new SafeGlobalMemoryBufferHandle();

            else
            {
               byte[] src = securityDescriptor.GetSecurityDescriptorBinaryForm();
               memoryHandle = new SafeGlobalMemoryBufferHandle(src.Length);
               memoryHandle.CopyFrom(src, 0, src.Length);
            }
         }

         //private uint nLength;
      }

      #endregion // SecurityAttributes

      #region Win32

      #region Win32FileAttributeData

      /// <summary>WIN32_FILE_ATTRIBUTE_DATA structure contains attribute information for a file or directory. The GetFileAttributesEx function uses this structure.</summary>
      /// <remarks>
      /// Not all file systems can record creation and last access time, and not all file systems record them in the same manner.
      /// For example, on the FAT file system, create time has a resolution of 10 milliseconds, write time has a resolution of 2 seconds,
      /// and access time has a resolution of 1 day. On the NTFS file system, access time has a resolution of 1 hour. 
      /// For more information, see File Times.
      /// </remarks>
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      internal struct Win32FileAttributeData
      {
         #region FileAttributes

         /// <summary>The file attributes of a file.</summary>
         internal FileAttributes FileAttributes;

         #endregion // FileAttributes

         #region CreationTime

         /// <summary>A <see cref="FileTime"/> structure that specifies when a file or directory was created.
         /// If the underlying file system does not support creation time, this member is zero.</summary>
         internal FileTime CreationTime;

         #endregion // CreationTime

         #region LastAccessTime

         /// <summary>A <see cref="FileTime"/> structure.
         /// For a file, the structure specifies when the file was last read from, written to, or for executable files, run.
         /// For a directory, the structure specifies when the directory is created. If the underlying file system does not support last access time, this member is zero.
         /// On the FAT file system, the specified date for both files and directories is correct, but the time of day is always set to midnight.
         /// </summary>
         internal FileTime LastAccessTime;

         #endregion // LastAccessTime

         #region LastWriteTime

         /// <summary>A <see cref="FileTime"/> structure.
         /// For a file, the structure specifies when the file was last written to, truncated, or overwritten, for example, when WriteFile or SetEndOfFile are used.
         /// The date and time are not updated when file attributes or security descriptors are changed.
         /// For a directory, the structure specifies when the directory is created. If the underlying file system does not support last write time, this member is zero.
         /// </summary>
         internal FileTime LastWriteTime;

         #endregion // LastWriteTime

         #region FileSizeHigh

         /// <summary>The high-order DWORD of the file size. This member does not have a meaning for directories.
         /// This value is zero unless the file size is greater than MAXDWORD.
         /// The size of the file is equal to (nFileSizeHigh * (MAXDWORD+1)) + nFileSizeLow.
         /// </summary>
         internal uint FileSizeHigh;

         #endregion // FileSizeHigh

         #region FileSizeLow

         /// <summary>The low-order DWORD of the file size. This member does not have a meaning for directories.</summary>
         internal uint FileSizeLow;

         #endregion // FileSizeLow
      }

      #endregion // Win32FileAttributeData

      #region Win32FindData

      /// <summary>WIN32_FIND_DATA structure - Contains information about the file that is found by the FindFirstFile, FindFirstFileEx, or FindNextFile function.</summary>
      /// <remarks>
      /// If a file has a long file name, the complete name appears in the cFileName member, and the 8.3 format truncated version of the name appears
      /// in the cAlternateFileName member. Otherwise, cAlternateFileName is empty. If the FindFirstFileEx function was called with a value of FindExInfoBasic
      /// in the fInfoLevelId parameter, the cAlternateFileName member will always contain a NULL string value. This remains true for all subsequent calls to the
      /// FindNextFile function. As an alternative method of retrieving the 8.3 format version of a file name, you can use the GetShortPathName function.
      /// For more information about file names, see File Names, Paths, and Namespaces.
      /// </remarks>
      /// <remarks>
      /// Not all file systems can record creation and last access times, and not all file systems record them in the same manner.
      /// For example, on the FAT file system, create time has a resolution of 10 milliseconds, write time has a resolution of 2 seconds,
      /// and access time has a resolution of 1 day. The NTFS file system delays updates to the last access time for a file by up to 1 hour
      /// after the last access. For more information, see File Times.
      /// </remarks>
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      internal struct Win32FindData
      {
         #region FileAttributes

         /// <summary>The file attributes of a file.</summary>
         internal FileAttributes FileAttributes;

         #endregion // FileAttributes

         #region CreationTime

         /// <summary>A <see cref="FileTime"/> structure that specifies when a file or directory was created.
         /// If the underlying file system does not support creation time, this member is zero.</summary>
         internal FileTime CreationTime;

         #endregion // CreationTime

         #region LastAccessTime

         /// <summary>A <see cref="FileTime"/> structure.
         /// For a file, the structure specifies when the file was last read from, written to, or for executable files, run.
         /// For a directory, the structure specifies when the directory is created. If the underlying file system does not support last access time, this member is zero.
         /// On the FAT file system, the specified date for both files and directories is correct, but the time of day is always set to midnight.
         /// </summary>
         internal FileTime LastAccessTime;

         #endregion // LastAccessTime

         #region LastWriteTime

         /// <summary>A <see cref="FileTime"/> structure.
         /// For a file, the structure specifies when the file was last written to, truncated, or overwritten, for example, when WriteFile or SetEndOfFile are used.
         /// The date and time are not updated when file attributes or security descriptors are changed.
         /// For a directory, the structure specifies when the directory is created. If the underlying file system does not support last write time, this member is zero.
         /// </summary>
         internal FileTime LastWriteTime;

         #endregion // LastWriteTime

         #region FileSizeHigh

         /// <summary>The high-order DWORD of the file size. This member does not have a meaning for directories.
         /// This value is zero unless the file size is greater than MAXDWORD.
         /// The size of the file is equal to (nFileSizeHigh * (MAXDWORD+1)) + nFileSizeLow.
         /// </summary>
         internal uint FileSizeHigh;

         #endregion // FileSizeHigh

         #region FileSizeLow

         /// <summary>The low-order DWORD of the file size. This member does not have a meaning for directories.</summary>
         internal uint FileSizeLow;

         #endregion // FileSizeLow

         #region Reserved0

         /// <summary>If the dwFileAttributes member includes the FILE_ATTRIBUTE_REPARSE_POINT attribute, this member specifies the reparse point tag.
         /// Otherwise, this value is undefined and should not be used.
         /// </summary>
         internal ReparsePointTags Reserved0;

         #endregion // Reserved0

         #region Reserved1

         /// <summary>Reserved for future use.</summary>
         internal uint Reserved1;

         #endregion // Reserved1

         #region FileName

         /// <summary>The name of the file.</summary>
         [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxPath)]
         internal string FileName;

         #endregion // FileName

         #region AlternateFileName

         /// <summary>An alternative name for the file. This name is in the classic 8.3 file name format.</summary>
         [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
         internal string AlternateFileName;

         #endregion AlternateFileName
      }

      #endregion // Win32FindData

      #region Win32StreamId

      /// <summary>The Win32StreamId structure contains stream data.</summary>
      [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
      internal struct Win32StreamId
      {
         /// <summary>Type of data.
         /// This member can be one of the following values:
         ///   BACKUP_DATA            0x00000001  Standard data. This corresponds to the NTFS $DATA stream type on the default (unnamed) data stream.
         ///   BACKUP_EA_DATA         0x00000002  Extended attribute data. This corresponds to the NTFS $EA stream type.
         ///   BACKUP_SECURITY_DATA   0x00000003  Security descriptor data.
         ///   BACKUP_ALTERNATE_DATA  0x00000004  Alternative data streams. This corresponds to the NTFS $DATA stream type on a named data stream.
         ///   BACKUP_LINK            0x00000005  Hard link information. This corresponds to the NTFS $FILE_NAME stream type.
         ///   BACKUP_PROPERTY_DATA   0x00000006  Property data.
         ///   BACKUP_OBJECT_ID       0x00000007  Objects identifiers. This corresponds to the NTFS $OBJECT_ID stream type.
         ///   BACKUP_REPARSE_DATA    0x00000008  Reparse points. This corresponds to the NTFS $REPARSE_POINT stream type.
         ///   BACKUP_SPARSE_BLOCK    0x00000009  Sparse file. This corresponds to the NTFS $DATA stream type for a sparse file.
         ///   BACKUP_TXFS_DATA       0x0000000A  Transactional NTFS (TxF) data stream. This corresponds to the NTFS $TXF_DATA stream type.
         ///                                      Windows Server 2003 and Windows XP:  This value is not supported.
         /// </summary>
         public readonly uint StreamId;

         /// <summary>Attributes of data to facilitate cross-operating system transfer.
         /// This member can be one or more of the following values:
         ///   STREAM_MODIFIED_WHEN_READ  0x00000001  Attribute set if the stream contains data that is modified when read. Allows the backup application to know that verification of data will fail.
         ///   STREAM_CONTAINS_SECURITY   0x00000002  Stream contains security data (general attributes). Allows the stream to be ignored on cross-operations restore.
         /// </summary>
         public readonly BackupStreamAttributes StreamAttributes;

         /// <summary>Size of data, in bytes.</summary>
         public readonly ulong Size;

         /// <summary>Length of the name of the alternative data stream, in bytes.</summary>
         public readonly uint StreamNameSize;
      }

      #endregion // Win32StreamId

      #endregion // Win32

      #endregion // Types

      #region Device Management

      /// <summary>Sends a control code directly to a specified device driver, causing the corresponding device to perform the corresponding operation.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "6")]
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool DeviceIoControl(SafeHandle hDevice, IoControlCode dwIoControlCode, IntPtr lpInBuffer, [MarshalAs(UnmanagedType.U4)] uint nInBufferSize, SafeHandle lpOutBuffer, [MarshalAs(UnmanagedType.U4)] uint nOutBufferSize, [MarshalAs(UnmanagedType.U4)] out uint lpBytesReturned, IntPtr lpOverlapped);

      //// 2013-03-15: Yomodo; Disabled for Now, not used.
      ///// <summary>The SetupDiDestroyDeviceInfoList function deletes a device information set and frees all associated memory.</summary>
      ///// <returns>
      ///// The function returns true if it is successful.
      ///// Otherwise, it returns false and the logged error can be retrieved with a call to GetLastError.
      ///// </returns>
      ///// <remarks>Available in Microsoft Windows 2000 and later versions of Windows.</remarks>
      //[SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      //[SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
      //[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      //[return: MarshalAs(UnmanagedType.Bool)]
      //internal static extern bool SetupDiDestroyDeviceInfoList(IntPtr hDevice);

      #endregion // Device Management
   }
}
