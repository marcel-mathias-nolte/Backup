//-----------------------------------------------------------------------
// <copyright file="IniSection.cs" company="RAN COMMUNITY SERVER">
//     (C)2010-2013 Marcel Nolte
// </copyright>
//-----------------------------------------------------------------------

namespace Nolte.IO
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Represents an Initialization file's section
    /// </summary>
    public class IniSection
    {
        /// <summary>
        /// The section's name
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// The section's inline comment
        /// </summary>
        private string inlineComment = string.Empty;

        /// <summary>
        /// The section's full-line comments
        /// </summary>
        private string prelineComment = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="IniSection" /> class.
        /// </summary>
        /// <param name="parent">The <see cref="IniFile" /> object containing this section</param>
        /// <param name="name">The section's name</param>
        public IniSection(IniFile parent, string name)
        {
            this.Values = new Dictionary<string, IniValue>();
            this.Parent = parent;
            this.name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniSection" /> class.
        /// </summary>
        /// <param name="parent">The <see cref="IniFile" /> object containing this section</param>
        public IniSection(IniFile parent)
        {
            this.Values = new Dictionary<string, IniValue>();
            this.Parent = parent;
        }

        /// <summary>
        /// Gets the <see cref="IniFile" /> object containing this section
        /// </summary>
        public IniFile Parent { get; internal set; }

        /// <summary>
        /// Gets the values of this initialization file's section
        /// </summary>
        public Dictionary<string, IniValue> Values { get; internal set; }

        /// <summary>
        /// Gets or sets the section's inline comment
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
        /// Gets or sets the section's full-line comments
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
        /// Gets or sets the section's name
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
        /// Gets the given key/value pair identified by it's key
        /// </summary>
        /// <param name="keyName">The key/value pair's key</param>
        /// <returns>The key/value pair</returns>
        public IniValue this[string keyName]
        {
            get
            {
                if (!this.Values.ContainsKey(keyName))
                {
                    this.Values.Add(keyName, new IniValue(this, keyName));
                }

                return this.Values[keyName];
            }

            set
            {
                if (value == null)
                {
                    return;
                }

                if (!this.Values.ContainsKey(keyName))
                {
                    this.Values[keyName] = value;
                }
                else
                {
                    this.Values.Add(keyName, value);
                }

                value.Parent = this;
            }
        }

        /// <summary>
        /// Gets the given key/value pair identified by it's position
        /// </summary>
        /// <param name="position">The key/value pair's position</param>
        /// <returns>The key/value pair</returns>
        public IniValue this[int position]
        {
            get
            {
                if (position >= this.Values.Keys.Count)
                {
                    throw new ArgumentOutOfRangeException("position");
                }

                var i = 0;
                foreach (var p in this.Values)
                {
                    if (i == position)
                    {
                        return p.Value;
                    }

                    i++;
                }

                return null; // will never be reached
            }
        }
    }
}
