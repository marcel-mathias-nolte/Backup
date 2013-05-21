//-----------------------------------------------------------------------
// <copyright file="OpenMode.cs" company="RAN COMMUNITY SERVER">
//     (C)2010-2013 Marcel Nolte
// </copyright>
// <comment>
//     Further information at: http://support.microsoft.com/default.aspx?scid=kb;en-us;105763
// </comment>
//-----------------------------------------------------------------------

namespace Nolte.IO
{
    /// <summary>
    /// PInvoke: File open mode for API calls
    /// </summary>
    public enum OpenMode : uint
    {
        /// <summary>
        /// PInvoke: Try to create a new file
        /// </summary>
        CreateNew = 1,

        /// <summary>
        /// PInvoke: Always create a new file
        /// </summary>
        CreateAlways = 2,

        /// <summary>
        /// PInvoke: Open existing file or create a new one
        /// </summary>
        OpenExisting = 3,

        /// <summary>
        /// PInvoke: Always open an existing file
        /// </summary>
        OpenAlways = 4
    }
}
