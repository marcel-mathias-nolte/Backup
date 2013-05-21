//-----------------------------------------------------------------------
// <copyright file="Hardlink.cs" company="RAN COMMUNITY SERVER">
//     (C)2010-2013 Marcel Nolte
// </copyright>
//-----------------------------------------------------------------------

namespace Nolte.IO
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Hard link management
    /// </summary>
    public static class Hardlink
    {
        /// <summary>
        /// Creates a new hard link
        /// </summary>
        /// <param name="link">new filename</param>
        /// <param name="target">existing file</param>
        public static void Create(string link, string target)
        {
            Create(link, target, false);
        }

        /// <summary>
        /// Creates a new hard link
        /// </summary>
        /// <param name="link">new filename</param>
        /// <param name="target">existing file</param>
        /// <param name="overwrite">True to overwrite existing files</param>
        public static void Create(string link, string target, bool overwrite)
        {
            target = Path.GetFullPath(target);
            link = Path.GetFullPath(link);

            if (!File.Exists(target))
            {
                throw new IOException("Target path does not exist or is not a file.");
            }

            if (File.Exists(link))
            {
                if (!overwrite)
                {
                    throw new IOException("File already exists and overwrite parameter is false.");
                }

                (new FileInfo(link)).Delete();
            }

            var directoryInfo = (new FileInfo(link)).Directory;
            if (directoryInfo != null && (new DriveInfo(directoryInfo.FullName[0].ToString(CultureInfo.InvariantCulture))).DriveFormat == "NTFS")
            {
                CreateHardLink(link, target, IntPtr.Zero);
            }
        }

        /// <summary>
        /// Creates a new hard link
        /// </summary>
        /// <param name="link">new filename</param>
        /// <param name="target">existing file</param>
        public static void Create(FileInfo link, FileInfo target)
        {
            Create(link.FullName, target.FullName, false);
        }

        /// <summary>
        /// Creates a new hard link
        /// </summary>
        /// <param name="link">new filename</param>
        /// <param name="target">existing file</param>
        /// <param name="overwrite">True to overwrite existing files</param>
        public static void Create(FileInfo link, FileInfo target, bool overwrite)
        {
            Create(link.FullName, target.FullName, overwrite);
        }

        /// <summary>
        /// Returns the number of links to the given file
        /// </summary>
        /// <param name="filename">The file to check</param>
        /// <returns>The number of links to the file</returns>
        public static int GetNumberOfLinks(string filename)
        {
            using (var s = new FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite))
            {
                Microsoft.Win32.SafeHandles.SafeFileHandle handle = s.SafeFileHandle;
                ByHandleFileInformation info;
                if (GetFileInformationByHandle(handle, out info))
                {
                    return info.NNumberOfLinks;
                }
            }

            return 0;
        }

        /// <summary>
        /// Returns if the given file is a hard link
        /// </summary>
        /// <param name="filename">The file to check</param>
        /// <returns>True if there is more than one link to the same file</returns>
        public static bool IsLink(string filename)
        {
            return GetNumberOfLinks(filename) > 1;
        }

        /// <summary>
        /// Returns all file names which are linked to one physical file
        /// </summary>
        /// <param name="filepath">Name of one file pointing to the data block</param>
        /// <returns>A collection of all file names, including the given, pointing to the same data structure as the given file name</returns>
        public static string[] GetFileSiblingHardLinks(string filepath)
        {
            var result = new List<string>();
            uint stringLength = 256;
            var sb = new StringBuilder(256);
            GetVolumePathName(filepath, sb, stringLength);
            var volume = sb.ToString();
            sb.Length = 0; 
            stringLength = 256;
            var findHandle = FindFirstFileNameW(filepath, 0, ref stringLength, sb);
            if (findHandle.ToInt32() != -1)
            {
                do
                {
                    var pathSb = new StringBuilder(volume, 256);
                    PathAppend(pathSb, sb.ToString());
                    result.Add(pathSb.ToString());
                    sb.Length = 0; 
                    stringLength = 256;
                } 
                while (FindNextFileNameW(findHandle, ref stringLength, sb));

                FindClose(findHandle);
                return result.ToArray();
            }

            return null;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetFileInformationByHandle(Microsoft.Win32.SafeHandles.SafeFileHandle file, out ByHandleFileInformation fileInformation);

        [DllImport("kernel32.dll", EntryPoint = "CreateHardLinkA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern long CreateHardLink(string fileName, string existingFileName, IntPtr securityAttributes);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr FindFirstFileNameW(string fileName, uint dwflags, ref uint stringLength, StringBuilder fileNameW);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool FindNextFileNameW(IntPtr findStreamHandle, ref uint stringLength, StringBuilder fileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FindClose(IntPtr findHandle);

        [DllImport("kernel32.dll")]
        private static extern bool GetVolumePathName(string lpszFileName, [Out] StringBuilder lpszVolumePathName, uint cchBufferLength);

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        private static extern bool PathAppend([In, Out] StringBuilder pszPath, string pszMore);
    }
}
