//-----------------------------------------------------------------------
// <copyright file="Drive.cs" company="RAN COMMUNITY SERVER">
//     (C)2010-2013 Marcel Nolte
// </copyright>
//-----------------------------------------------------------------------

namespace Nolte.IO
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Management;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Provides access to drive specific information
    /// </summary>
    public class Drive
    {
        /// <summary>
        /// Gets a list of all local hard disk volumes
        /// </summary>
        public static List<string> GeLocalHarddiskVolumes
        {
            get
            {
                var drives = new List<string>();
                foreach (var di in DriveInfo.GetDrives())
                {
                    if (di.DriveType == System.IO.DriveType.Fixed && di.IsReady)
                    {
                        drives.Add(di.RootDirectory.FullName);
                    }
                }

                return drives;
            }
        }

        /// <summary>
        /// Get the total and available space on a drive
        /// </summary>
        /// <param name="path">The drive to check (can be a path)</param>
        /// <returns>The size information</returns>
        public static DriveSize GetSpace(string path)
        {
            long available, total, free;
            GetDiskFreeSpaceEx(path, out available, out total, out free);
            return new DriveSize(total, available, free);
        }

        /// <summary>
        /// Returns a human readable file size
        /// </summary>
        /// <param name="size">The size to convert</param>
        /// <returns>The size with appropriate suffix</returns>
        public static string HumanSize(long size)
        {
            var suffixes = new[] { string.Empty, "k", "M", "G", "T", "E" };
            var pos = 0;
            while (size > 20480)
            {
                size /= 1024;
                pos++;
            }

            return size.ToString(CultureInfo.InvariantCulture) + suffixes[pos];
        }

        /// <summary>
        /// Get the serial number of a logical volume
        /// </summary>
        /// <param name="drive">The drive or path to check</param>
        /// <returns>The volume's serial number</returns>
        public static string GetDriveSerialNumber(string drive)
        {
            var driveSerialnumber = string.Empty;
            var pathRoot = Path.GetPathRoot(drive);
            if (pathRoot != null)
            {
                var driveFixed = pathRoot.Replace(@"\", string.Empty);
                var wmiQuery = "SELECT VolumeSerialNumber FROM Win32_LogicalDisk Where Name = '" + driveFixed + "'";
                using (var driveSearcher = new ManagementObjectSearcher(wmiQuery))
                {
                    using (var driveCollection = driveSearcher.Get())
                    {
                        foreach (ManagementObject managementObjectItem in driveCollection)
                        {
                            driveSerialnumber = Convert.ToString(managementObjectItem["VolumeSerialNumber"]);
                        }
                    }
                }
            }

            return driveSerialnumber;
        }

        /// <summary>
        /// Resolve the real volume of an directory (because it can be different due to junction points)
        /// </summary>
        /// <param name="path">The drive or path to check</param>
        /// <returns>The resulting path</returns>
        public static string GetRealPath(string path)
        {
            var di = new DirectoryInfo(path);
            if (!di.Exists)
            {
                return path;
            }

            while (di != null && di.Parent != null)
            {
                if (Junction.Exists(di.FullName))
                {
                    di = new DirectoryInfo(Junction.GetTarget(di.FullName));
                }

                di = di.Parent;
            }

            return di == null ? string.Empty : di.FullName;
        }

        [DllImport("kernel32.dll", EntryPoint = "GetDiskFreeSpaceExA")]
        private static extern long GetDiskFreeSpaceEx(string directoryName, out long freeBytesAvailableToCaller, out long totalNumberOfBytes, out long totalNumberOfFreeBytes);
    }
}