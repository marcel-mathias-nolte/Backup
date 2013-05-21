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
using System.IO;
using System.Runtime.InteropServices;

namespace Alphaleonis.Win32.Filesystem
{
   #region ByHandleFileInfo

   /// <summary>BY_HANDLE_FILE_INFORMATION - Represents information retrieved by File.GetFileInformationByHandle()</summary>
   [Serializable]
   [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
   public sealed class ByHandleFileInfo
   {
      #region Constructor

      internal ByHandleFileInfo()
      {
      }

      #endregion // Constructor

      #region Fields

      private FileAttributes _fileAttributes;
      private FileTime _creationTime;
      private FileTime _lastAccessTime;
      private FileTime _lastWriteTime;
      private uint _volumeSerialNumber;
      private uint _fileSizeHigh;
      private uint _fileSizeLow;
      private uint _numberOfLinks;
      private uint _fileIndexHigh;
      private uint _fileIndexLow;

      #endregion // Fields

      #region Properties

      #region Attributes

      /// <summary>Gets the file attributes.</summary>
      /// <value>The file attributes.</value>
      public FileAttributes Attributes
      {
         get { return _fileAttributes; }
      }

      #endregion // Attributes

      #region CreationTime

      /// <summary>Gets a <see cref="DateTime"/> structure that specified when a file or directory was created.</summary>
      /// <value>A <see cref="DateTime"/> structure that specified when a file or directory was created.</value>
      public DateTime CreationTime
      {
         get { return DateTime.FromFileTimeUtc(_creationTime).ToLocalTime(); }
      }

      #endregion // CreationTime

      #region LastAccessTime

      /// <summary>Gets a <see cref="DateTime"/> structure. 
      /// For a file, the structure specifies the last time that a file is read from or written to. 
      /// For a directory, the structure specifies when the directory is created. 
      /// For both files and directories, the specified date is correct, but the time of day is always set to midnight. 
      /// If the underlying file system does not support the last access time, this member is zero (0).
      /// </summary>
      /// <value>A <see cref="DateTime"/> structure that specified when a file was last written to or the directory created.</value>
      public DateTime LastAccessTime
      {
         get { return DateTime.FromFileTimeUtc(_lastAccessTime).ToLocalTime(); }
      }

      #endregion // LastAccessTime

      #region LastWriteTime

      /// <summary>Gets a <see cref="DateTime"/> structure. 
      /// For a file, the structure specifies the last time that a file is written to. 
      /// For a directory, the structure specifies when the directory is created. 
      /// If the underlying file system does not support the last access time, this member is zero (0).
      /// </summary>
      /// <value>A <see cref="DateTime"/> structure that specified when a file was last written to or the directory created.</value>
      public DateTime LastWriteTime
      {
         get { return DateTime.FromFileTimeUtc(_lastWriteTime).ToLocalTime(); }
      }

      #endregion // LastWriteTime

      #region VolumeSerialNumber

      /// <summary>Gets the the serial number of the volume that contains a file.</summary>
      /// <value>The serial number of the volume that contains a file.</value>
      public uint VolumeSerialNumber
      {
         get { return _volumeSerialNumber; }
      }

      #endregion // VolumeSerialNumber

      #region FileSize

      /// <summary>Gets the size of the file.</summary>
      /// <value>The size of the file.</value>
      public long FileSize
      {
         get { return NativeMethods.ToLong(_fileSizeHigh, _fileSizeLow); }
      }

      #endregion // FileSize

      #region NumberOfLinks

      /// <summary>Gets the number of links to this file. For the FAT file system this member is always 1. For the NTFS file system, it can be more than 1.</summary>
      /// <value>The number of links to this file. </value>
      public uint NumberOfLinks
      {
         get { return _numberOfLinks; }
      }

      #endregion // NumberOfLinks

      #region FileIndex

      /// <summary>
      /// Gets the unique identifier associated with the file. The identifier and the volume serial number uniquely identify a 
      /// file on a single computer. To determine whether two open handles represent the same file, combine the identifier 
      /// and the volume serial number for each file and compare them.
      /// </summary>
      /// <value>The unique identifier of the file.</value>
      public long FileIndex
      {
         get { return NativeMethods.ToLong(_fileIndexHigh, _fileIndexLow); }
      }

      #endregion // FileIndex

      #endregion // Properties
   }

   #endregion // ByHandleFileInfo
}