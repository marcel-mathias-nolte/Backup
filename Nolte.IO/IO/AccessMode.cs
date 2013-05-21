//-----------------------------------------------------------------------
// <copyright file="AccessMode.cs" company="RAN COMMUNITY SERVER">
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
    /// PInvoke: File access mode for API calls
    /// </summary>
    [Flags]
    public enum AccessMode : uint
    {
        /// <summary>
        /// PInvoke: Write access
        /// </summary>
        GenericWrite = 0x40000000,

        /// <summary>
        /// PInvoke: Read access
        /// </summary>
        GenericRead = 0x80000000,

        /// <summary>
        /// PInvoke: Execution access
        /// </summary>
        GenericExecute = 0x20000000,

        /// <summary>
        /// PInvoke: Any access (all together)
        /// </summary>
        GenericAll = 0x10000000
    }
}
