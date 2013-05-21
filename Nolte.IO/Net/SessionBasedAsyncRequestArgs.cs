//-----------------------------------------------------------------------
// <copyright file="SessionBasedAsyncRequestArgs.cs" company="RAN COMMUNITY SERVER">
//     (C)2010-2013 Marcel Nolte
// </copyright>
//-----------------------------------------------------------------------

namespace Nolte.Net
{
    /// <summary>
    /// Delegate for throwing events, to signalize new finished post or get requests
    /// </summary>
    /// <param name="e">The result set</param>
    public delegate void SessionBasedAsyncRequestHandler(SessionBasedAsyncRequestArgs e);

    /// <summary>
    /// A result set of a post or get query
    /// </summary>
    public class SessionBasedAsyncRequestArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionBasedAsyncRequestArgs" /> class.
        /// </summary>
        /// <param name="result">The HTML content returned by the post or get query</param>
        /// <param name="tag">The user-defined tag</param>
        public SessionBasedAsyncRequestArgs(string result, object tag = null)
        {
            this.Result = result;
            this.Tag = tag;
        }

        /// <summary>
        /// Gets or sets the HTML content returned by the post or get query
        /// </summary>
        public string Result { get; protected set; }

        /// <summary>
        /// Gets or sets the user-defined tag
        /// </summary>
        public object Tag { get; protected set; }
    }
}
