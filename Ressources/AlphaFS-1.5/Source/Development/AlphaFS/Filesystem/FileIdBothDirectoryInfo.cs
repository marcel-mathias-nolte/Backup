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

namespace Alphaleonis.Win32.Filesystem
{
   /// <summary>Contains information about files in the specified directory.</summary>
   /// <remarks>See also: Directory.GetFileIdBothDirectoryInfo()</remarks>
   public class FileIdBothDirectoryInfo
   {
      #region Constructor

      #region FileIdBothDirectoryInfo

      internal FileIdBothDirectoryInfo(NativeMethods.FileIdBothDirInfo fibdi, string fileName)
      {
         CreationTime = DateTime.FromFileTimeUtc(fibdi.CreationTime).ToLocalTime();
         LastAccessTime = DateTime.FromFileTimeUtc(fibdi.LastAccessTime).ToLocalTime();
         LastWriteTime = DateTime.FromFileTimeUtc(fibdi.LastWriteTime).ToLocalTime();
         ChangeTime = DateTime.FromFileTimeUtc(fibdi.ChangeTime).ToLocalTime();

         EndOfFile = fibdi.EndOfFile;
         AllocationSize = fibdi.AllocationSize;
         FileAttributes = fibdi.FileAttributes;
         ExtendedAttributesSize = fibdi.EaSize;

         // ShortNameLength is the number of bytes in the short name; since we have a unicode string we must divide that by 2.
         ShortName = new string(fibdi.ShortName, 0, fibdi.ShortNameLength/2);

         FileIndex = fibdi.FileIndex;
         FileId = fibdi.FileId;
         FileName = fileName;
      }

      #endregion // FileIdBothDirectoryInfo

      #endregion // Constructor

      #region Properties

      #region AllocationSize

      /// <summary>The number of bytes that are allocated for the file. This value is usually a multiple of the sector or cluster size of the underlying physical device.</summary>
      public long AllocationSize { get; set; }

      #endregion // AllocationSize

      #region ChangeTime

      /// <summary>The time that the file was last changed.</summary>
      public DateTime ChangeTime { get; set; }

      #endregion // ChangeTime

      #region EaSize

      /// <summary>The size of the extended attributes for the file.</summary>
      public uint ExtendedAttributesSize { get; set; }

      #endregion // EaSize

      #region EndOfFile

      /// <summary>The absolute new end-of-file position as a byte offset from the start of the file to the end of the file. 
      /// Because this value is zero-based, it actually refers to the first free byte in the file. In other words, <b>EndOfFile</b> is the offset to 
      /// the byte that immediately follows the last valid byte in the file.
      /// </summary>
      public long EndOfFile { get; set; }

      #endregion // EndOfFile

      #region FileAttributes

      /// <summary>The file attributes.</summary>
      public FileAttributes FileAttributes { get; set; }

      #endregion FileAttributes

      #region FileId

      /// <summary>The file ID.</summary>
      public long FileId { get; set; }

      #endregion // FileId

      #region FileIndex

      /// <summary>The byte offset of the file within the parent directory. This member is undefined for file systems, such as NTFS,
      /// in which the position of a file within the parent directory is not fixed and can be changed at any time to maintain sort order.
      /// </summary>
      public uint FileIndex { get; set; }

      #endregion // FileIndex

      #region FileName

      /// <summary>The name of the file.</summary>
      public string FileName { get; set; }

      #endregion // FileName

      #region CreationTime

      /// <summary>The time that the file was created.</summary>
      public DateTime CreationTime { get; set; }

      #endregion // CreationTime

      #region LastAccessTime

      /// <summary>The time that the file was last accessed.</summary>
      public DateTime LastAccessTime { get; set; }

      #endregion // LastAccessTime

      #region LastWriteTime

      /// <summary>The time that the file was last written to.</summary>
      public DateTime LastWriteTime { get; set; }

      #endregion // LastWriteTime

      #region ShortName

      /// <summary>The short 8.3 file naming convention (for example, FILENAME.TXT) name of the file.</summary>
      public string ShortName { get; set; }

      #endregion // ShortName

      #endregion // Properties
   }
}