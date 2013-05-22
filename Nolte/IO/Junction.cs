//-----------------------------------------------------------------------
// <copyright file="Junction.cs" company="RAN COMMUNITY SERVER">
//     (C)2010-2013 Marcel Nolte
// </copyright>
//-----------------------------------------------------------------------

namespace Nolte.IO
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using Microsoft.Win32.SafeHandles;
    
    /// <summary>
    /// Provides access to NTFS junction points in .Net.
    /// </summary>
    public static class Junction
    {
        /// <summary>
        /// The file or directory is not a reparse point.
        /// </summary>
        private const int ErrorNotAReparsePoint = 4390;

        /// <summary>
        /// Command to set the reparse point data block.
        /// </summary>
        private const int FsctlSetReparsePoint = 0x000900A4;

        /// <summary>
        /// Command to get the reparse point data block.
        /// </summary>
        private const int FsctlGetReparsePoint = 0x000900A8;

        /// <summary>
        /// Command to delete the reparse point data base.
        /// </summary>
        private const int FsctlDeleteReparsePoint = 0x000900AC;

        /// <summary>
        /// Reparse point tag used to identify mount points and junction points.
        /// </summary>
        private const uint IoReparseTagMountPoint = 0xA0000003;

        /// <summary>
        /// This prefix indicates to NTFS that the path is to be treated as a non-interpreted
        /// path in the virtual file system.
        /// </summary>
        private const string NonInterpretedPathPrefix = @"\??\";

        /// <summary>
        /// Creates a junction point from the specified directory to the specified target directory.
        /// </summary>
        /// <remarks>
        /// Only works on NTFS.
        /// </remarks>
        /// <param name="junctionPoint">The junction point path</param>
        /// <param name="targetDir">The target directory</param>
        /// <param name="overwrite">If true overwrites an existing reparse point or empty directory</param>
        /// <exception cref="IOException">Thrown when the junction point could not be created or when
        /// an existing directory was found and <paramref name="overwrite" /> if false</exception>
        public static void Create(string junctionPoint, string targetDir, bool overwrite)
        {
            targetDir = Path.GetFullPath(targetDir);

            if (!Directory.Exists(targetDir))
            {
                throw new IOException("Target path does not exist or is not a directory.");
            }

            if (Directory.Exists(junctionPoint))
            {
                if (!overwrite)
                {
                    throw new IOException("Directory already exists and overwrite parameter is false.");
                }
            }
            else
            {
                Directory.CreateDirectory(junctionPoint);
            }

            using (SafeFileHandle handle = OpenReparsePoint(junctionPoint, AccessMode.GenericWrite))
            {
                byte[] targetDirBytes = Encoding.Unicode.GetBytes(NonInterpretedPathPrefix + Path.GetFullPath(targetDir));

                var reparseDataBuffer = new ReparseDataBuffer
                {
                    ReparseTag = IoReparseTagMountPoint, 
                    ReparseDataLength = (ushort)(targetDirBytes.Length + 12), 
                    SubstituteNameOffset = 0, 
                    SubstituteNameLength = (ushort)targetDirBytes.Length, 
                    PrintNameOffset = (ushort)(targetDirBytes.Length + 2), 
                    PrintNameLength = 0, 
                    PathBuffer = new byte[0x3ff0]
                };

                Array.Copy(targetDirBytes, reparseDataBuffer.PathBuffer, targetDirBytes.Length);

                var inBufferSize = Marshal.SizeOf(reparseDataBuffer);
                var inBuffer = Marshal.AllocHGlobal(inBufferSize);

                try
                {
                    Marshal.StructureToPtr(reparseDataBuffer, inBuffer, false);

                    int bytesReturned;
                    var result = DeviceIoControl(handle.DangerousGetHandle(), FsctlSetReparsePoint, inBuffer, targetDirBytes.Length + 20, IntPtr.Zero, 0, out bytesReturned, IntPtr.Zero);

                    if (!result)
                    {
                        ThrowLastWin32Error("Unable to create junction point.");
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(inBuffer);
                }
            }
        }

        /// <summary>
        /// Deletes a junction point at the specified source directory along with the directory itself.
        /// Does nothing if the junction point does not exist.
        /// </summary>
        /// <remarks>
        /// Only works on NTFS.
        /// </remarks>
        /// <param name="junctionPoint">The junction point path</param>
        public static void Delete(string junctionPoint)
        {
            if (!Directory.Exists(junctionPoint))
            {
                if (File.Exists(junctionPoint))
                {
                    throw new IOException("Path is not a junction point.");
                }

                return;
            }

            using (var handle = OpenReparsePoint(junctionPoint, AccessMode.GenericWrite))
            {
                var reparseDataBuffer = new ReparseDataBuffer
                {
                    ReparseTag = IoReparseTagMountPoint, 
                    ReparseDataLength = 0, 
                    PathBuffer = new byte[0x3ff0]
                };

                var inBufferSize = Marshal.SizeOf(reparseDataBuffer);
                var inBuffer = Marshal.AllocHGlobal(inBufferSize);
                try
                {
                    Marshal.StructureToPtr(reparseDataBuffer, inBuffer, false);

                    int bytesReturned;
                    var result = DeviceIoControl(handle.DangerousGetHandle(), FsctlDeleteReparsePoint, inBuffer, 8, IntPtr.Zero, 0, out bytesReturned, IntPtr.Zero);

                    if (!result)
                    {
                        ThrowLastWin32Error("Unable to delete junction point.");
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(inBuffer);
                }

                try
                {
                    Directory.Delete(junctionPoint);
                }
                catch (IOException ex)
                {
                    throw new IOException("Unable to delete junction point.", ex);
                }
            }
        }

        /// <summary>
        /// Determines whether the specified path exists and refers to a junction point.
        /// </summary>
        /// <param name="path">The junction point path</param>
        /// <returns>True if the specified path represents a junction point</returns>
        /// <exception cref="IOException">Thrown if the specified path is invalid
        /// or some other error occurs</exception>
        public static bool Exists(string path)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }

            using (var handle = OpenReparsePoint(path, AccessMode.GenericRead))
            {
                return InternalGetTarget(handle) != null;
            }
        }

        /// <summary>
        /// Gets the target of the specified junction point.
        /// </summary>
        /// <remarks>
        /// Only works on NTFS.
        /// </remarks>
        /// <param name="junctionPoint">The junction point path</param>
        /// <returns>The target of the junction point</returns>
        /// <exception cref="IOException">Thrown when the specified path does not
        /// exist, is invalid, is not a junction point, or some other error occurs</exception>
        public static string GetTarget(string junctionPoint)
        {
            using (var handle = OpenReparsePoint(junctionPoint, AccessMode.GenericRead))
            {
                var target = InternalGetTarget(handle);
                if (target == null)
                {
                    throw new IOException("Path is not a junction point.");
                }

                return target;
            }
        }

        /// <summary>
        /// Gets the target of the specified junction point.
        /// </summary>
        /// <param name="handle">The file handle for which the target should be searched</param>
        /// <returns>The target path as string</returns>
        private static string InternalGetTarget(SafeFileHandle handle)
        {
            var outBufferSize = Marshal.SizeOf(typeof(ReparseDataBuffer));
            var outBuffer = Marshal.AllocHGlobal(outBufferSize);

            try
            {
                int bytesReturned;
                var result = DeviceIoControl(handle.DangerousGetHandle(), FsctlGetReparsePoint, IntPtr.Zero, 0, outBuffer, outBufferSize, out bytesReturned, IntPtr.Zero);

                if (!result)
                {
                    if (Marshal.GetLastWin32Error() == ErrorNotAReparsePoint)
                    {
                        return null;
                    }

                    ThrowLastWin32Error("Unable to get information about junction point.");
                }

                var reparseDataBuffer = (ReparseDataBuffer)Marshal.PtrToStructure(outBuffer, typeof(ReparseDataBuffer));

                if (reparseDataBuffer.ReparseTag != IoReparseTagMountPoint)
                {
                    return null;
                }

                var targetDir = Encoding.Unicode.GetString(reparseDataBuffer.PathBuffer, reparseDataBuffer.SubstituteNameOffset, reparseDataBuffer.SubstituteNameLength);

                if (targetDir.StartsWith(NonInterpretedPathPrefix))
                {
                    targetDir = targetDir.Substring(NonInterpretedPathPrefix.Length);
                }

                return targetDir;
            }
            finally
            {
                Marshal.FreeHGlobal(outBuffer);
            }
        }

        /// <summary>
        /// Gets an safe file handle to a reparse point
        /// </summary>
        /// <param name="reparsePoint">The file path of the reparse point</param>
        /// <param name="accessMode">The access mode to use</param>
        /// <returns>The generated file handle or null if failed</returns>
        private static SafeFileHandle OpenReparsePoint(string reparsePoint, AccessMode accessMode)
        {
            var reparsePointHandle = new SafeFileHandle(CreateFile(reparsePoint, accessMode, FileShareMode.ShareRead | FileShareMode.ShareWrite | FileShareMode.Delete, IntPtr.Zero, CreationDisposition.OpenExisting, FileAttributes.BackupSemantics | FileAttributes.OpenReparsePoint, IntPtr.Zero), true);

            if (Marshal.GetLastWin32Error() != 0)
            {
                ThrowLastWin32Error("Unable to open reparse point.");
            }

            return reparsePointHandle;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool DeviceIoControl(IntPtr deviceHandle, uint iocontrolCode, IntPtr inbuffer, int inbufferSize, IntPtr outBuffer, int outBufferSize, out int bytesReturned, IntPtr overlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFile(string fileName, AccessMode desiredAccess, FileShareMode shareMode, IntPtr securityAttributes, CreationDisposition creationDisposition, FileAttributes flagsAndAttributes, IntPtr templateFile);

        /// <summary>
        /// Throws a new error based on an PInvoke error
        /// </summary>
        /// <param name="message">Error message to throw</param>
        private static void ThrowLastWin32Error(string message)
        {
            throw new IOException(message, Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
        }
    }
}
