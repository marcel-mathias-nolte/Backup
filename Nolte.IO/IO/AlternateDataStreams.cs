//-----------------------------------------------------------------------
// <copyright file="AlternateDataStreams.cs" company="RAN COMMUNITY SERVER">
//     (C)2010-2013 Marcel Nolte
//     (C)2005 Sean Michael Murphy
// </copyright>
// <comment>
//     Further information at: http://support.microsoft.com/default.aspx?scid=kb;en-us;105763
// </comment>
//-----------------------------------------------------------------------

namespace Nolte.IO
{
    using System;
    
    /// <summary>
    /// A class with static methods for reading and writing to "hidden" streams in
    /// a normal file.  We have to drop down to interoperation here because MS managed code
    /// will not allow colons in the filename, which specify the stream within the
    /// file to access.  This only works on NTFS drives, not FAT or FAT32 drives.
    /// </summary>
    public class AlternateDataStreams
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="AlternateDataStreams" /> class from being created.
        /// </summary>
        private AlternateDataStreams()
        {
        }

        /// <summary>
        /// Returns true if the given file has an alternate data stream with the given name
        /// </summary>
        /// <param name="file">The file to look on</param>
        /// <param name="stream">The stream to search for</param>
        /// <returns>True, if there is an stream with the given name</returns>
        public static bool Exists(string file, string stream)
        {
            var handle = CreateFile(file + ":" + stream, AccessMode.GenericRead, FileShareMode.ShareRead, IntPtr.Zero, OpenMode.OpenExisting, 0, IntPtr.Zero);

            // if the handle returned is uint.MaxValue, the stream doesn't exist.
            if (handle != uint.MaxValue)
            {
                CloseHandle(handle);
                return true;
            }

            try
            {
                CloseHandle(handle);
            }
            catch (Exception)
            {
            }

            return false;
        }

        /// <summary>
        /// Method called when an alternate data stream must be read from.
        /// </summary>
        /// <param name="file">The fully qualified name of the file from which
        /// the ADS data will be read.</param>
        /// <param name="stream">The name of the stream within the "normal" file
        /// from which to read.</param>
        /// <returns>The contents of the file as a string.  It will always return
        /// at least a zero-length string, even if the file does not exist.</returns>
        public static string Read(string file, string stream)
        {
            uint handle = CreateFile(file + ":" + stream, AccessMode.GenericRead, FileShareMode.ShareRead, IntPtr.Zero, OpenMode.OpenExisting, 0, IntPtr.Zero);

            // if the handle returned is uint.MaxValue, the stream doesn't exist.
            if (handle != uint.MaxValue)
            {
                // A handle to the stream within the file was created successfully.
                var size = GetFileSize(handle, IntPtr.Zero);
                var buffer = new byte[size];
                var read = uint.MinValue;
                ReadFile(handle, buffer, size, ref read, IntPtr.Zero);

                CloseHandle(handle);

                // Convert the bytes read into an ASCII string and return it to the caller.
                return System.Text.Encoding.ASCII.GetString(buffer);
            }

            throw new StreamNotFoundException(file, stream);
        }

        /// <summary>
        /// Delete an alternate data stream
        /// </summary>
        /// <param name="file">The file, which the stream is attached to</param>
        /// <param name="stream">The stream's name</param>
        /// <returns>True, if the deletion succeeded</returns>
        public static bool Delete(string file, string stream)
        {
            return DeleteFile(file + ":" + stream);
        }

        /// <summary>
        /// The static method to call when data must be written to a stream.
        /// </summary>
        /// <param name="data">The string data to embed in the stream in the file</param>
        /// <param name="file">The fully qualified name of the file with the stream into which the data will be written.</param>
        /// <param name="stream">The name of the stream within the normal file to write the data.</param>
        /// <returns>An unsigned integer of how many bytes were actually written.</returns>
        public static uint Write(string data, string file, string stream)
        {
            // Convert the string data to be written to an array of ascii characters.
            byte[] barData = System.Text.Encoding.ASCII.GetBytes(data);
            uint nreturn = 0;

            uint handle = CreateFile(file + ":" + stream, AccessMode.GenericWrite, FileShareMode.ShareWrite, IntPtr.Zero, OpenMode.OpenAlways, 0, IntPtr.Zero);

            bool bok = WriteFile(handle, barData, (uint)barData.Length, ref nreturn, IntPtr.Zero);

            CloseHandle(handle);

            // Throw an exception if the data wasn't written successfully.
            if (!bok)
            {
                throw new System.ComponentModel.Win32Exception(
                    System.Runtime.InteropServices.Marshal.GetLastWin32Error());
            }

            return nreturn;
        }

        #region Win32 API Defines

        [System.Runtime.InteropServices.DllImport("kernel32", SetLastError = true)]
        private static extern uint GetFileSize(uint handle, IntPtr size);

        [System.Runtime.InteropServices.DllImport("kernel32", CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool DeleteFile(string name);

        [System.Runtime.InteropServices.DllImport("kernel32", SetLastError = true)]
        private static extern uint ReadFile(uint handle, byte[] buffer, uint byteToRead, ref uint bytesRead, IntPtr overlapped);

        [System.Runtime.InteropServices.DllImport("kernel32", SetLastError = true)]
        private static extern uint CreateFile(string filename, AccessMode desiredAccess, FileShareMode shareMode, IntPtr attributes, OpenMode creationDisposition, uint flagsAndAttributes, IntPtr templateFile);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteFile(uint fileHandle, byte[] buffer, uint numberOfBytesToWrite, ref uint numberOfBytesWritten, IntPtr overlapped);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(uint fileHandle);

        #endregion
    }
}