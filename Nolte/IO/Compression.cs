//-----------------------------------------------------------------------
// <copyright file="Compression.cs" company="RAN COMMUNITY SERVER">
//     (C)2010-2013 Marcel Nolte
// </copyright>
//-----------------------------------------------------------------------

namespace Nolte.IO
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Provides information on compressed files on NTFS volumes
    /// </summary>
    public static class Compression
    {
        /// <summary>
        /// Returns the size, which a compressed file need on a NTFS volume
        /// </summary>
        /// <param name="filename">File to check</param>
        /// <returns>Size in Bytes</returns>
        public static ulong CompressedSize(string filename)
        {
            if (!System.IO.File.Exists(filename))
            {
                throw new System.IO.FileNotFoundException();
            }

            if (!IsCompressed(filename)) 
            {
                return (ulong)new System.IO.FileInfo(filename).Length;
            }

            uint high;
            var low = GetCompressedFileSizeAPI(filename, out high);
            var error = Marshal.GetLastWin32Error();
            if (high == 0 && low == 0xFFFFFFFF && error != 0)
            {
                throw new System.ComponentModel.Win32Exception(error);
            }
            
            return ((ulong)high << 32) + low;
        }

        /// <summary>
        /// Determines if a file is NTFS compressed
        /// </summary>
        /// <param name="filename">File to check</param>
        /// <returns>true if the file is compressed</returns>
        public static bool IsCompressed(string filename)
        {
            if (!System.IO.File.Exists(filename))
            {
                throw new System.IO.FileNotFoundException();
            }

            var fi = new System.IO.FileInfo(filename);
            return (fi.Attributes & System.IO.FileAttributes.Compressed) == System.IO.FileAttributes.Compressed;
        }

        /// <summary>
        /// Compresses the given file using NTFS compression
        /// </summary>
        /// <param name="filename">The File to compress</param>
        public static void Compress(string filename)
        {
            if (IsCompressed(filename))
            {
                return;
            }

            // verification of file existence on IsCompressed()
            try
            {
                short compressionFormatDefault = 1;
                const int FsctlSetCompression = 0x9C040;
                var bytesReturned = 0;
                var f = System.IO.File.Open(filename, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);
                DeviceIoControl(f.SafeFileHandle, FsctlSetCompression, ref compressionFormatDefault, 2, IntPtr.Zero, 0, ref bytesReturned, IntPtr.Zero);
                f.Close();
            }
            catch
            {
                throw new System.ComponentModel.Win32Exception("Failed to set compression on file.");
            }
        }

        /// <summary>
        /// Deflates the given file using NTFS compression
        /// </summary>
        /// <param name="filename">The File to deflate</param>
        public static void Uncompress(string filename)
        {
            if (!IsCompressed(filename))
            {
                return;
            }

            // verification of file existence on IsCompressed()
            try
            {
                short compressionFormatNone = 0;
                const int FsctlSetCompression = 0x9C040;
                var bytesReturned = 0;
                var f = System.IO.File.Open(filename, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);
                DeviceIoControl(f.SafeFileHandle, FsctlSetCompression, ref compressionFormatNone, 2, IntPtr.Zero, 0, ref bytesReturned, IntPtr.Zero);
                f.Close();
            }
            catch
            {
                throw new System.ComponentModel.Win32Exception("Failed to set compression on file.");
            }
        }

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "GetCompressedFileSize")]
        private static extern uint GetCompressedFileSizeAPI(string fileName, out uint fileSizeHigh);

        [DllImport("kernel32.dll")]
        private static extern int DeviceIoControl(Microsoft.Win32.SafeHandles.SafeFileHandle devicehandle, int iocontrolCode, ref short inBuffer, int inBufferSize, IntPtr outBuffer, int outBufferSize, ref int bytesReturned, IntPtr overlapped);
    }
}
