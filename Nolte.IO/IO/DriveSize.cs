//-----------------------------------------------------------------------
// <copyright file="DriveSize.cs" company="RAN COMMUNITY SERVER">
//     (C)2010-2013 Marcel Nolte
// </copyright>
//-----------------------------------------------------------------------

namespace Nolte.IO
{
    /// <summary>
    /// Represents the size of a Drive
    /// </summary>
    public class DriveSize
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DriveSize" /> class representing the volume properties.
        /// </summary>
        /// <param name="totalSpace">total free space</param>
        /// <param name="availableSpace">total available space</param>
        /// <param name="freeSpace">total bytes of this volume</param>
        public DriveSize(long totalSpace, long availableSpace, long freeSpace)
        {
            this.AvailableBytes = availableSpace;
            this.FreeBytes = freeSpace;
            this.TotalBytes = totalSpace;
            this.UsedBytes = totalSpace - freeSpace;
        }

        /// <summary>
        /// Gets the number of bytes available to the user
        /// </summary>
        public long AvailableBytes { get; private set; }

        /// <summary>
        /// Gets the total size of the drive
        /// </summary>
        public long TotalBytes { get; private set; }

        /// <summary>
        /// Gets the number of free bytes on the drive
        /// </summary>
        public long FreeBytes { get; private set; }

        /// <summary>
        /// Gets the number of used bytes on the drive
        /// </summary>
        public long UsedBytes { get; private set; }

        /// <summary>
        /// Gets the number of bytes available to the user
        /// </summary>
        public string AvailableBytesH
        {
            get
            {
                return Drive.HumanSize(this.AvailableBytes);
            }
        }

        /// <summary>
        /// Gets the total size of the drive
        /// </summary>
        public string TotalBytesH
        {
            get
            {
                return Drive.HumanSize(this.TotalBytes);
            }
        }

        /// <summary>
        /// Gets the number of free bytes on the drive
        /// </summary>
        public string FreeBytesH
        {
            get
            {
                return Drive.HumanSize(this.FreeBytes);
            }
        }

        /// <summary>
        /// Gets the number of used bytes on the drive
        /// </summary>
        public string UsedBytesH
        {
            get
            {
                return Drive.HumanSize(this.UsedBytes);
            }
        }

        /// <summary>
        /// Provides an addition of the file's size
        /// </summary>
        /// <param name="size1">The first Size</param>
        /// <param name="size2">The size to add</param>
        /// <returns>The resulting size</returns>
        public static DriveSize operator +(DriveSize size1, DriveSize size2)
        {
            return new DriveSize(size1.TotalBytes + size2.TotalBytes, size1.AvailableBytes + size2.AvailableBytes, size1.FreeBytes + size2.FreeBytes);
        }
    }
}
