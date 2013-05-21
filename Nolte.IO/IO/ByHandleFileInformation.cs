//-----------------------------------------------------------------------
// <copyright file="ByHandleFileInformation.cs" company="RAN COMMUNITY SERVER">
//     (C)2010-2013 Marcel Nolte
// </copyright>
//-----------------------------------------------------------------------

namespace Nolte.IO
{
    /// <summary>
    /// File information
    /// </summary>
    internal struct ByHandleFileInformation
    {
        /// <summary>
        /// File attributes
        /// </summary>
        public int DwFileAttributes;

        /// <summary>
        /// File creation time
        /// </summary>
        public System.Runtime.InteropServices.ComTypes.FILETIME FtCreationTime;

        /// <summary>
        /// File last access time
        /// </summary>
        public System.Runtime.InteropServices.ComTypes.FILETIME FtLastAccessTime;

        /// <summary>
        /// File modification time
        /// </summary>
        public System.Runtime.InteropServices.ComTypes.FILETIME FtLastWriteTime;

        /// <summary>
        /// Volume serial number
        /// </summary>
        public int DwVolumeSerialNumber;

        /// <summary>
        /// File size (higher byte)
        /// </summary>
        public int NFileSizeHigh;

        /// <summary>
        /// File size (lower byte)
        /// </summary>
        public int NFileSizeLow;

        /// <summary>
        /// Number of hard links to the same file
        /// </summary>
        public int NNumberOfLinks;

        /// <summary>
        /// File index number (higher byte)
        /// </summary>
        public int NFileIndexHigh;

        /// <summary>
        /// File index number (lower byte)
        /// </summary>
        public int NFileIndexLow;
    }
}
