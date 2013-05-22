//-----------------------------------------------------------------------
// <copyright file="PostArguments.cs" company="RAN COMMUNITY SERVER">
//     (C)2010-2013 Marcel Nolte
// </copyright>
//-----------------------------------------------------------------------

namespace Nolte.Net
{
    using System.Collections.Generic;
    using System.Text;
    using System.Web;

    /// <summary>
    /// Represents a collection of uri parameters to submit on a post
    /// </summary>
    public class PostArguments
    {
        /// <summary>
        /// A collection of the data to submit
        /// </summary>
        private readonly Dictionary<string, string> postData = new Dictionary<string, string>();

        /// <summary>
        /// Gets the length of the encoded content
        /// </summary>
        public int ContentLength
        {
            get
            {
                return this.Content.Length;
            }
        }

        /// <summary>
        /// Gets the encoded content 
        /// </summary>
        public byte[] Content
        {
            get
            {
                string content = string.Empty;
                foreach (var data in this.postData)
                {
                    content += (string.IsNullOrWhiteSpace(content) ? string.Empty : "&") + HttpUtility.UrlEncode(data.Key) + "=" + HttpUtility.UrlEncode(data.Value);
                }

                return Encoding.ASCII.GetBytes(content);
            }
        }

        /// <summary>
        /// Gets the property's value identified by it's name
        /// </summary>
        /// <param name="name">The name of the property to search for</param>
        /// <returns>The property's value</returns>
        public string this[string name]
        {
            get
            {
                return this.postData.ContainsKey(name) ? this.postData[name] : string.Empty;
            }
        }

        /// <summary>
        /// Add a key/value pair to the data collection
        /// </summary>
        /// <param name="name">The property's name</param>
        /// <param name="value">The property's value</param>
        public void Add(string name, string value)
        {
            if (this.postData.ContainsKey(name))
            {
                this.postData[name] = value;
            }
            else
            {
                this.postData.Add(name, value);
            }
        }

        /// <summary>
        /// Removes the given property
        /// </summary>
        /// <param name="name">The name of the property to remove</param>
        public void Remove(string name)
        {
            if (this.postData.ContainsKey(name))
            {
                this.postData.Remove(name);
            }
        }

        /// <summary>
        /// Checks if the given property is set
        /// </summary>
        /// <param name="name">The name of the property to search for</param>
        /// <returns>True, if the property exists</returns>
        public bool Contains(string name)
        {
            return this.postData.ContainsKey(name);
        }
    }
}
