using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Nolte.Reflection
{
    /// <summary>
    /// Provides an interface for automatic inclusion of all class libraries in an directory, which implement the given interface
    /// </summary>
    public class PluginLoader
    {
        protected List<Plugin> availablePlugins = new List<Plugin>();
        protected string classNameToProvide;
        protected string interfaceToImplement;

        /// <summary>
        /// Event handler for log entries
        /// </summary>
        /// <param name="Timestamp">When it happened</param>
        /// <param name="Type">What kind of message</param>
        /// <param name="Source">The method which caused the entry</param>
        /// <param name="Message">The message sent</param>
        public delegate void LogHandler(DateTime Timestamp, string Type, string Source, string Message);

        /// <summary>
        /// Occurs, if a new log message was generated
        /// </summary>
        public event LogHandler Log;

        /// <summary>
        /// Create a new instance of the plugin loader
        /// </summary>
        /// <param name="ClassNameToProvide">The classnames to search for in the found libraries</param>
        /// <param name="InterfaceToImplement">The interface, which the found classes have to implement</param>
        public PluginLoader(string ClassNameToProvide, string InterfaceToImplement)
        {
            this.classNameToProvide = ClassNameToProvide;
            this.interfaceToImplement = InterfaceToImplement;
        }

        /// <summary>
        /// Generates an log entry
        /// </summary>
        /// <param name="Type">Log type, i.e. WRANING, INFO, ERROR</param>
        /// <param name="Source">Method, which caused the entry</param>
        /// <param name="Message">The log message</param>
        protected void OnLog(string Type, string Source, string Message)
        {
            if (this.Log != null)
            {
                this.Log(DateTime.Now, Type, "Nolte.PluginLoader." + Source, Message);
            }
        }

        /// <summary>
        /// Tries to load the assembly from the given filename and checks if it fullfills all criteria
        /// </summary>
        /// <param name="Filename">The filename of the library file to load</param>
        private void AddPlugin(string Filename)
        {
            try
            {
                Assembly pluginAssembly = Assembly.LoadFrom(Filename);
                foreach (Type pluginType in pluginAssembly.GetTypes())
                {
                    if (pluginType.Name == this.classNameToProvide)
                    {
                        if (pluginType.IsPublic)
                        {
                            if (!pluginType.IsAbstract)
                            {
                                Type typeInterface = pluginType.GetInterface(this.interfaceToImplement, true);
                                if (typeInterface != null)
                                {
                                    this.availablePlugins.Add(new Plugin(pluginType, pluginAssembly, Filename, this.interfaceToImplement));
                                }
                                typeInterface = null;
                            }
                        }
                    }
                }
                pluginAssembly = null;
            }
            catch (Exception ex)
            {
                this.OnLog("INFO", "AddPlugin", "Fehler beim Laden der Plugin-DLL " + Filename + "\n\nTrace:\n\n" + ex.StackTrace);
            }
            finally
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// Try to load all dll files in the current directory
        /// </summary>
        public void Load()
        {
            this.Load((new FileInfo(Application.ExecutablePath)).DirectoryName, "");
        }

        /// <summary>
        /// Try to load all dll files in the given directory
        /// </summary>
        /// <param name="Path">The directory to search in</param>
        public void Load(string Path)
        {
            this.Load(Path, "");
        }

        /// <summary>
        /// Try to load all dll files in the given directory which's filename start with the given prefix
        /// </summary>
        /// <param name="Path">The directory to search in</param>
        /// <param name="Prefix">The file prefix to check for</param>
        public void Load(string Path, string Prefix)
        {
            foreach (string fileOn in Directory.GetFiles(Path))
            {
                FileInfo file = new FileInfo(fileOn);
                if (file.Extension.Equals(".dll") && file.Name.StartsWith(Prefix))
                {
                    this.AddPlugin(fileOn);
                }
            }
        }

        /// <summary>
        /// Get a List of all Plugins
        /// </summary>
        public List<Plugin> AvailablePlugins
        {
            get
            {
                return this.availablePlugins;
            }
        }
    }
}
