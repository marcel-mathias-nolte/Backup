//-----------------------------------------------------------------------
// <copyright file="CreationDisposition.cs" company="RAN COMMUNITY SERVER">
//     (C)2010-2013 Marcel Nolte
// </copyright>
//-----------------------------------------------------------------------

namespace Nolte.IO
{
    /// <summary>
    /// PInvoke: Junction creation disposition
    /// </summary>
    public enum CreationDisposition : uint
    {
        /// <summary>
        /// PInvoke: Create new junction or open existing one
        /// </summary>
        New = 1,

        /// <summary>
        /// PInvoke: Always create new junction (delete existing)
        /// </summary>
        CreateAlways = 2,

        /// <summary>
        /// PInvoke: Open existing junction
        /// </summary>
        OpenExisting = 3,

        /// <summary>
        /// Always open existing junction
        /// </summary>
        OpenAlways = 4,

        /// <summary>
        /// PInvoke: truncate existing junction instead of deleting and re-creating the junction
        /// </summary>
        TruncateExisting = 5,
    }
}
