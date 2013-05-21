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
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Alphaleonis.Win32.Filesystem
{
   internal static class DeviceIo
   {
      #region CompressionEnable

      /// <summary>Sets the compression state of a file or directory on a volume whose file system supports per-file and per-directory compression.</summary>
      /// <param name="path">A path that describes a folder or file to compress or decompress.</param>
      /// <param name="compress"><c>true</c> = compress, <c>false</c> = decompress</param>
      /// <returns>If the function succeeds, <c>true</c>, otherwise <c>false</c>.</returns>
      [SecurityCritical]
      public static bool CompressionEnable(string path, bool compress)
      {
         return CompressionEnable(null, path, compress);
      }

      #region Transacted

      /// <summary>Sets the compression state of a file or directory on a volume whose file system supports per-file and per-directory compression.</summary>
      /// <param name="transaction"><para>The transaction.</para></param>
      /// <param name="path">A path that describes a folder or file to compress or decompress.</param>
      /// <param name="compress"><c>true</c> = compress, <c>false</c> = decompress</param>
      /// <returns>If the function succeeds, <c>true</c>, otherwise <c>false</c>.</returns>
      [SecurityCritical]
      public static bool CompressionEnable(KernelTransaction transaction, string path, bool compress)
      {
         using (SafeFileHandle handle = File.CreateFileInternal(transaction, path, EFileAttributes.BackupSemantics, null, FileMode.Open, FileSystemRights.FullControl, FileShare.None))
         {
            uint lpBytesReturned;

            // 0 = Decompress, 1 = Compress.
            short lpInBuffer = (short) ((compress) ? 1 : 0);

            if (!NativeMethods.DeviceIoControl(handle, IoControlCode.FsctlSetCompression, ref lpInBuffer, sizeof (short), IntPtr.Zero, 0, out lpBytesReturned, IntPtr.Zero))
               NativeError.ThrowException(path);

            return true;
         }
      }

      #endregion // Transacted

      #endregion // CompressionEnable

      #region GetLinkTargetInfo

      [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Runtime.InteropServices.SafeHandle.DangerousGetHandle")]
      [SecurityCritical]
      public static LinkTargetInfo GetLinkTargetInfo(SafeHandle device)
      {
         // Start with a large buffer to prevent a 2nd call.
         uint bytesReturned = NativeMethods.MaxPathUnicode;
         SafeGlobalMemoryBufferHandle safeBuffer = null;

         try
         {
            safeBuffer = new SafeGlobalMemoryBufferHandle((int)bytesReturned);

            do
            {
               // Possible PInvoke signature bug: safeBuffer.Capacity  and  bytesReturned are always the same.
               // Since we use a large buffer, we're not affected.

               if (!NativeMethods.DeviceIoControl(device, IoControlCode.FsctlGetReparsePoint, IntPtr.Zero, 0, safeBuffer, (uint)safeBuffer.Capacity, out bytesReturned, IntPtr.Zero))
               {
                  int lastError = Marshal.GetLastWin32Error();
                  switch ((uint)lastError)
                  {
                     case Win32Errors.ERROR_MORE_DATA:
                     case Win32Errors.ERROR_INSUFFICIENT_BUFFER:
                        if (safeBuffer.Capacity < bytesReturned)
                        {
                           safeBuffer.Dispose();
                           break;
                        }
                        NativeError.ThrowException(lastError);
                        break;
                  }
               }
               else
                  break;
            } while (true);

            
            IntPtr bufPtr = safeBuffer.DangerousGetHandle();
            Type toMountPointReparseBuffer = typeof(MountPointReparseBuffer);
            Type toReparseDataBufferHeader = typeof(ReparseDataBufferHeader);
            Type toSymbolicLinkReparseBuffer = typeof(SymbolicLinkReparseBuffer);
            IntPtr marshalReparseBuffer = Marshal.OffsetOf(toReparseDataBufferHeader, "data");
            ReparseDataBufferHeader header = (ReparseDataBufferHeader)Marshal.PtrToStructure(bufPtr, toReparseDataBufferHeader);
            IntPtr dataPos;
            byte[] dataBuffer;

            switch (header.ReparseTag)
            {
               case NativeMethods.ReparsePointTags.MountPoint:
                  MountPointReparseBuffer mprb = (MountPointReparseBuffer)Marshal.PtrToStructure(new IntPtr(bufPtr.ToInt64() + marshalReparseBuffer.ToInt64()), toMountPointReparseBuffer);

                  dataPos = new IntPtr(marshalReparseBuffer.ToInt64() + Marshal.OffsetOf(toMountPointReparseBuffer, "data").ToInt64());
                  dataBuffer = new byte[bytesReturned - dataPos.ToInt64()];

                  Marshal.Copy(new IntPtr(bufPtr.ToInt64() + dataPos.ToInt64()), dataBuffer, 0, dataBuffer.Length);

                  return new LinkTargetInfo(
                     Encoding.Unicode.GetString(dataBuffer, mprb.SubstituteNameOffset, mprb.SubstituteNameLength),
                     Encoding.Unicode.GetString(dataBuffer, mprb.PrintNameOffset, mprb.PrintNameLength));

               case NativeMethods.ReparsePointTags.SymLink:
                  SymbolicLinkReparseBuffer slrb = (SymbolicLinkReparseBuffer)Marshal.PtrToStructure(new IntPtr(bufPtr.ToInt64() +marshalReparseBuffer.ToInt64()), toSymbolicLinkReparseBuffer);

                  dataPos = new IntPtr(marshalReparseBuffer.ToInt64() + Marshal.OffsetOf(toSymbolicLinkReparseBuffer, "data").ToInt64());
                  dataBuffer = new byte[bytesReturned - dataPos.ToInt64()];

                  Marshal.Copy(new IntPtr(bufPtr.ToInt64() + dataPos.ToInt64()), dataBuffer, 0, dataBuffer.Length);

                  return new SymbolicLinkTargetInfo(
                     Encoding.Unicode.GetString(dataBuffer, slrb.SubstituteNameOffset, slrb.SubstituteNameLength),
                     Encoding.Unicode.GetString(dataBuffer, slrb.PrintNameOffset, slrb.PrintNameLength),
                     (SymbolicLinkType)slrb.Flags);

               default:
                  throw new UnrecognizedReparsePointException();
            }
         }
         finally
         {
            if (safeBuffer != null)
               safeBuffer.Dispose();
         }
      }

      #endregion // GetLinkTargetInfo

      #region Structures

      [StructLayout(LayoutKind.Sequential)]
      private struct ReparseDataBufferHeader
      {
         [MarshalAs(UnmanagedType.U4)] public readonly NativeMethods.ReparsePointTags ReparseTag;
         public readonly ushort ReparseDataLength;
         public readonly ushort Reserved;
         [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)] public readonly byte[] data;
      }

      [StructLayout(LayoutKind.Sequential)]
      private struct SymbolicLinkReparseBuffer
      {
         public readonly ushort SubstituteNameOffset;
         public readonly ushort SubstituteNameLength;
         public readonly ushort PrintNameOffset;
         public readonly ushort PrintNameLength;
         public readonly uint Flags;
         [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)] public readonly byte[] data;
      }

      [StructLayout(LayoutKind.Sequential)]
      private struct MountPointReparseBuffer
      {
         public readonly ushort SubstituteNameOffset;
         public readonly ushort SubstituteNameLength;
         public readonly ushort PrintNameOffset;
         public readonly ushort PrintNameLength;
         [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)] public readonly byte[] data;
      }

      #endregion // Structures
   }
}