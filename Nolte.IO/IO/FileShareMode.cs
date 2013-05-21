//-----------------------------------------------------------------------
// <copyright file="FileShareMode.cs" company="RAN COMMUNITY SERVER">
//     (C)2010-2013 Marcel Nolte
// </copyright>
// <comment>
//     Further information at: http://support.microsoft.com/default.aspx?scid=kb;en-us;105763
// </comment>
//-----------------------------------------------------------------------

namespace Nolte.IO
{
    using System;

    /// <summary>
    /// PInvoke: File share mode for API calls
    /// </summary>
    [Flags]
    public enum FileShareMode : uint
    {
        /// <summary>
        /// PInvoke: No shared access
        /// </summary>
        None = 0x00000000,

        /// <summary>
        /// PInvoke: Shared read access
        /// </summary>
        ShareRead = 0x00000001,

        /// <summary>
        /// PInvoke: Shared write access
        /// </summary>
        ShareWrite = 0x00000002,

        /// <summary>
        /// PInvoke: Shared delete access
        /// </summary>
        Delete = 0x00000004,
    }
}
