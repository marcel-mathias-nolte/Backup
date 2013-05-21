//-----------------------------------------------------------------------
// <copyright file="IniValue.cs" company="RAN COMMUNITY SERVER">
//     (C)2010-2013 Marcel Nolte
// </copyright>
//-----------------------------------------------------------------------

namespace Nolte.IO
{
    using System;

    /// <summary>
    /// Represents a single initialization file's key/value pair
    /// </summary>
    public class IniValue
    {
        /// <summary>
        /// The key/value pair's key
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// The key/value pair's value
        /// </summary>
        private string value = string.Empty;

        /// <summary>
        /// The key/value pair's inline comment
        /// </summary>
        private string inlineComment = string.Empty;

        /// <summary>
        /// The key/value pair's full-line comments
        /// </summary>
        private string prelineComment = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="IniValue" /> class.
        /// </summary>
        /// <param name="parent">The key/value pair's parent section</param>
        /// <param name="name">The key/value pair's key</param>
        /// <param name="value">The key/value pair's value</param>
        public IniValue(IniSection parent, string name, string value)
        {
            this.Parent = parent;
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniValue" /> class.
        /// </summary>
        /// <param name="parent">The key/value pair's parent section</param>
        /// <param name="name">The key/value pair's key</param>
        public IniValue(IniSection parent, string name)
        {
            this.Parent = parent;
            this.name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniValue" /> class.
        /// </summary>
        /// <param name="parent">The key/value pair's parent section</param>
        public IniValue(IniSection parent)
        {
            this.Parent = parent;
        }

        /// <summary>
        /// Gets the key/value pair's parent section
        /// </summary>
        public IniSection Parent { get; internal set; }

        /// <summary>
        /// Gets or sets the key/value pair's inline comment
        /// </summary>
        public string InlineComment
        {
            get
            {
                return this.inlineComment;
            }
            
            set
            {
                if (value != null)
                {
                    this.inlineComment = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the key/value pair's full-line comments
        /// </summary>
        public string PrelineComment
        {
            get
            {
                return this.prelineComment;
            }

            set
            {
                if (value != null)
                {
                    this.prelineComment = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the key/value pair's key
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            
            set
            {
                if (value != null)
                {
                    this.name = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the key/value pair's value
        /// </summary>
        public string Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (value != null)
                {
                    this.value = value;
                }
            }
        }
    }
}
