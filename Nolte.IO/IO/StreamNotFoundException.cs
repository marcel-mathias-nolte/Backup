//-----------------------------------------------------------------------
// <copyright file="StreamNotFoundException.cs" company="RAN COMMUNITY SERVER">
//     (C)2010-2013 Marcel Nolte
// </copyright>
//-----------------------------------------------------------------------

namespace Nolte.IO
{
    /// <summary>
    /// Class to allow stream read operations to raise specific errors if the stream
    /// is not found in the file.
    /// </summary>
    public class StreamNotFoundException : System.IO.FileNotFoundException
    {
        /// <summary>
        /// <para>Initializes a new instance of the <see cref="StreamNotFoundException" /> class.</para>
        /// <para>Constructor called with the name of the file and stream which was unsuccessfully opened.</para>
        /// </summary>
        /// <param name="file">Fully qualified name of the file in which the stream was supposed to reside.</param>
        /// <param name="stream">Stream within the file to open.</param>
        public StreamNotFoundException(string file, string stream) : base(string.Empty, file)
        {
            this.Stream = stream;
        }

        /// <summary>
        /// <para>Gets the stream name</para>
        /// <para>Read-only property to allow the user to query the exception to determine the name of the stream that couldn't be found.</para>
        /// </summary>
        public string Stream { get; private set; }

        /// <summary>
        /// <para>Gets the error Message.</para>
        /// <para>Overridden Message property to return a concise string describing the exception.</para>
        /// </summary>
        public override string Message
        {
            get
            {
                return "Stream \"" + this.Stream + "\" not found in \"" + this.FileName + "\"";
            }
        }
    }
}
