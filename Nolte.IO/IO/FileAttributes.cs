//-----------------------------------------------------------------------
// <copyright file="FileAttributes.cs" company="RAN COMMUNITY SERVER">
//     (C)2010-2013 Marcel Nolte
// </copyright>
//-----------------------------------------------------------------------

namespace Nolte.IO
{
    using System;

    /// <summary>
    /// PInvoke: File attributes
    /// </summary>
    [Flags]
    public enum FileAttributes : uint
    {
        /// <summary>
        /// PInvoke: Read only / write protected
        /// </summary>
        Readonly = 0x00000001,

        /// <summary>
        /// PInvoke: Hidden file
        /// </summary>
        Hidden = 0x00000002,

        /// <summary>
        /// PInvoke: System file
        /// </summary>
        System = 0x00000004,

        /// <summary>
        /// PInvoke: Is a directory
        /// </summary>
        Directory = 0x00000010,

        /// <summary>
        /// PInvoke: Is archived
        /// </summary>
        Archive = 0x00000020,

        /// <summary>
        /// PInvoke: Is a device
        /// </summary>
        Device = 0x00000040,

        /// <summary>
        /// PInvoke: Is a usual file
        /// </summary>
        Normal = 0x00000080,

        /// <summary>
        /// PInvoke: Is a temporary file
        /// </summary>
        Temporary = 0x00000100,

        /// <summary>
        /// PInvoke: Is a sparse file (consumes even unused space)
        /// </summary>
        SparseFile = 0x00000200,

        /// <summary>
        /// PInvoke: Is a reparse point (junction)
        /// </summary>
        ReparsePoint = 0x00000400,

        /// <summary>
        /// PInvoke: Is NTFS compressed
        /// </summary>
        Compressed = 0x00000800,

        /// <summary>
        /// PInvoke: Is offline
        /// </summary>
        Offline = 0x00001000,

        /// <summary>
        /// PInvoke: is not indexed by windows search
        /// </summary>
        NotContentIndexed = 0x00002000,

        /// <summary>
        /// PInvoke: Is EFS encrypted
        /// </summary>
        Encrypted = 0x00004000,

        /// <summary>
        /// PInvoke: Is a write-through file (without buffer)
        /// </summary>
        WriteThrough = 0x80000000,

        /// <summary>
        /// PInvoke: Is an overlapped file
        /// </summary>
        Overlapped = 0x40000000,

        /// <summary>
        /// PInvoke: Is an un-buffered file
        /// </summary>
        NoBuffering = 0x20000000,

        /// <summary>
        /// Is an random access optimized file
        /// </summary>
        RandomAccess = 0x10000000,

        /// <summary>
        /// PInvoke: Is an sequential access optimized file
        /// </summary>
        SequentialScan = 0x08000000,

        /// <summary>
        /// PInvoke: Delete on close
        /// </summary>
        DeleteOnClose = 0x04000000,

        /// <summary>
        /// PInvoke: backup semantics
        /// </summary>
        BackupSemantics = 0x02000000,

        /// <summary>
        /// PInvoke: File uses posix semantics
        /// </summary>
        PosixSemantics = 0x01000000,

        /// <summary>
        /// PInvoke: Open reparse point
        /// </summary>
        OpenReparsePoint = 0x00200000,

        /// <summary>
        /// PInvoke: Open no recall
        /// </summary>
        OpenNoRecall = 0x00100000,

        /// <summary>
        /// PInvoke: Is a pipe
        /// </summary>
        FirstPipeInstance = 0x00080000,

        /// <summary>
        /// The file is a virtual file.
        /// </summary>
        Virtual = 0x10000,
    }
}
