//-----------------------------------------------------------------------
// <copyright file="WebClientEx.cs" company="RAN COMMUNITY SERVER">
//     (C)2010-2013 Marcel Nolte
// </copyright>
//-----------------------------------------------------------------------

namespace Nolte.Net
{
    using System;
    using System.Net;

    /// <summary>
    /// A web client with increased and changeable timeout
    /// </summary>
    public class WebClientEx : WebClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebClientEx" /> class.
        /// </summary>
        public WebClientEx()
        {
            this.Timeout = 30000;
        }

        /// <summary>
        /// Gets or sets the timeout in milliseconds
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Generates a new WebRequest object
        /// </summary>
        /// <param name="address">The uri to use</param>
        /// <returns>The generated object or null</returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            var result = base.GetWebRequest(address);
            if (result != null)
            {
                result.Timeout = this.Timeout;
            }

            return result;
        }
    }
}
