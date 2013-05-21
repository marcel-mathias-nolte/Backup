//-----------------------------------------------------------------------
// <copyright file="IniFile.cs" company="RAN COMMUNITY SERVER">
//     (C)2010-2013 Marcel Nolte
// </copyright>
//-----------------------------------------------------------------------

namespace Nolte.IO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    /// <summary>
    /// Representing an initialization file structure and provide parsing and rendering functions
    /// </summary>
    public class IniFile
    {
        /// <summary>
        /// The sections of this initialization structure
        /// </summary>
        private readonly Dictionary<string, IniSection> sections = new Dictionary<string, IniSection>();

        /// <summary>
        /// The source file from which the initialization structure was loaded
        /// </summary>
        private string sourceFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        public IniFile()
        {
            this.sections.Add(string.Empty, new IniSection(this));
        }

        /// <summary>
        /// Gets the rendered initialization file's content
        /// </summary>
        public string AsText
        {
            get
            {
                var result = string.Empty;
                foreach (var line in this.RenderedContent)
                {
                    result += line + "\r\n";
                }

                return result;
            }
        }

        /// <summary>
        /// Gets or sets the global section of the initialization structure
        /// </summary>
        public IniSection Global
        {
            get
            {
                return this.sections[string.Empty];
            }

            set
            {
                this.sections[string.Empty] = value;
            }
        }

        /// <summary>
        /// Gets the rendered initialization file's content as single lines
        /// </summary>
        protected List<string> RenderedContent
        {
            get
            {
                var lines = new List<string>();
                if (!string.IsNullOrWhiteSpace(this.Global.PrelineComment))
                {
                    foreach (var comment in this.Global.PrelineComment.Split('\n'))
                    {
                        lines.Add("; " + comment);
                    }
                }

                foreach (var p in this.Global.Values)
                {
                    if (!string.IsNullOrWhiteSpace(p.Value.PrelineComment))
                    {
                        foreach (var comment in p.Value.PrelineComment.Split('\n'))
                        {
                            lines.Add("; " + comment);
                        }
                    }

                    lines.Add((p.Value.Name.StartsWith(" ") || p.Value.Name.EndsWith(" ") || p.Value.Name.Contains('=') || p.Value.Name.StartsWith("\"") ? "\"" + p.Value.Name.Replace("\"", "\"\"") + "\"" : p.Value.Name) + " = " + (p.Value.Value.StartsWith(" ") || p.Value.Value.EndsWith(" ") || p.Value.Value.Contains('=') || p.Value.Value.StartsWith("\"") ? "\"" + p.Value.Value.Replace("\"", "\"\"") + "\"" : p.Value.Value) + (string.IsNullOrWhiteSpace(p.Value.InlineComment) ? string.Empty : " ; " + p.Value.InlineComment));
                }

                foreach (var s in this.sections)
                {
                    if (string.IsNullOrWhiteSpace(s.Value.Name))
                    {
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(s.Value.PrelineComment))
                    {
                        foreach (var comment in s.Value.PrelineComment.Split('\n'))
                        {
                            lines.Add("; " + comment);
                        }
                    }

                    lines.Add("[" + s.Value.Name + "]" + (string.IsNullOrWhiteSpace(s.Value.InlineComment) ? string.Empty : " ; " + s.Value.InlineComment));
                    foreach (var p in s.Value.Values)
                    {
                        if (!string.IsNullOrWhiteSpace(p.Value.PrelineComment))
                        {
                            foreach (var comment in p.Value.PrelineComment.Split('\n'))
                            {
                                lines.Add("; " + comment);
                            }
                        }

                        lines.Add((p.Value.Name.StartsWith(" ") || p.Value.Name.EndsWith(" ") || p.Value.Name.Contains('=') || p.Value.Name.StartsWith("\"") ? "\"" + p.Value.Name.Replace("\"", "\"\"") + "\"" : p.Value.Name) + " = " + (p.Value.Value.StartsWith(" ") || p.Value.Value.EndsWith(" ") || p.Value.Value.Contains('=') || p.Value.Value.StartsWith("\"") ? "\"" + p.Value.Value.Replace("\"", "\"\"") + "\"" : p.Value.Value) + (string.IsNullOrWhiteSpace(p.Value.InlineComment) ? string.Empty : " ; " + p.Value.InlineComment));
                    }
                }

                return lines;
            }
        }

        /// <summary>
        /// Gets the given initialization section identified by it's name
        /// </summary>
        /// <param name="sectionName">The section's name</param>
        /// <returns>The section</returns>
        public IniSection this[string sectionName]
        {
            get
            {
                if (!this.sections.ContainsKey(sectionName))
                {
                    this.sections.Add(sectionName, new IniSection(this, sectionName));
                }

                return this.sections[sectionName];
            }

            set
            {
                if (value == null)
                {
                    return;
                }

                if (!this.sections.ContainsKey(sectionName))
                {
                    this.sections[sectionName] = value;
                }
                else
                {
                    this.sections.Add(sectionName, value);
                }

                value.Parent = this;
            }
        }

        /// <summary>
        /// Gets the given initialization section identified by it's position 
        /// </summary>
        /// <param name="position">The section's position</param>
        /// <returns>The section</returns>
        public IniSection this[int position]
        {
            get
            {
                if (position >= this.sections.Keys.Count)
                {
                    throw new ArgumentOutOfRangeException("position");
                }

                var i = 0;
                foreach (var p in this.sections)
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

        /// <summary>
        /// Creates an initialization structure by loading the given file.
        /// </summary>
        /// <param name="filename">The file to load</param>
        /// <returns>The rendered initialization structure</returns>
        public static IniFile FromFile(string filename)
        {
            if (!System.IO.File.Exists(filename))
            {
                throw new System.IO.FileNotFoundException("Cannot open file", filename);
            }

            var f = new IniFile();
            var section = string.Empty;
            var nextComment = string.Empty;
            foreach (string line in System.IO.File.ReadAllLines(filename))
            {
                var tmp = line.Trim();
                var comment = string.Empty;

                // section start
                switch (tmp[0])
                {
                    case '[':
                        tmp = tmp.Substring(1);
                        if (tmp.Contains(']'))
                        {
                            section = tmp.Substring(0, tmp.IndexOf(']'));
                            tmp = tmp.Substring(tmp.IndexOf(']') + 1);
                        }
                        else if (tmp.Contains(';'))
                        {
                            section = tmp.Substring(0, tmp.IndexOf(';')).TrimEnd();
                            tmp = tmp.Substring(tmp.IndexOf(';'));
                        }

                        if (tmp.Contains(';'))
                        {
                            comment = tmp.Substring(tmp.IndexOf(';') + 1).TrimStart();
                        }

                        f[section].InlineComment = comment;
                        if (!string.IsNullOrWhiteSpace(nextComment))
                        {
                            f[section].PrelineComment = nextComment;
                            nextComment = string.Empty;
                        }

                        break;
                    case ';':
                        nextComment += (string.IsNullOrWhiteSpace(nextComment) ? string.Empty : "\n") + tmp.Substring(1).TrimStart();
                        break;
                    default:
                        {
                            var inQuotes = false;
                            for (var i = 0; i < tmp.Length; i++)
                            {
                                if (tmp[i] == '"')
                                {
                                    inQuotes = !inQuotes;
                                }
                                else if (!inQuotes && tmp[i] == '=')
                                {
                                    var key = tmp.Substring(0, i).TrimEnd();
                                    if (key[0] == '"')
                                    {
                                        key = key.Trim(new[] { '"' }).Replace("\"\"", "\"");
                                    }

                                    var value = tmp.Substring(i + 1).TrimStart();
                                    for (var j = 0; j < value.Length; j++)
                                    {
                                        if (value[j] == '"')
                                        {
                                            inQuotes = !inQuotes;
                                        }
                                        else if (!inQuotes && value[j] == ';')
                                        {
                                            comment = value.Substring(j + 1).TrimStart();
                                            value = value.Substring(0, j).TrimEnd();
                                            break;
                                        }
                                    }

                                    if (value[0] == '"')
                                    {
                                        value = value.Trim(new[] { '"' }).Replace("\"\"", "\"");
                                    }

                                    f[section][key].Value = value;
                                    f[section][key].InlineComment = comment;
                                    if (!string.IsNullOrWhiteSpace(nextComment))
                                    {
                                        f[section][key].PrelineComment = nextComment;
                                        nextComment = string.Empty;
                                    }

                                    break;
                                }
                            }
                        }

                        break;
                }
            }

            f.sourceFile = filename;
            return f;
        }

        /// <summary>
        /// Save the initialization file to the file it was loaded from
        /// </summary>
        public void Save()
        {
            this.SaveAs(this.sourceFile);
        }

        /// <summary>
        /// Save the data to the given file
        /// </summary>
        /// <param name="filename">The file to save to</param>
        public void SaveAs(string filename)
        {
            System.IO.File.WriteAllLines(filename, this.RenderedContent);
        }
    }
}
