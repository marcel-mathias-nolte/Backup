using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Nolte.Reflection
{
    /// <summary>
    /// A found plugin
    /// </summary>
    public struct Plugin
    {
        /// <summary>
        /// The PluginType
        /// </summary>
        public Type Type;

        /// <summary>
        /// The plugin's assembly
        /// </summary>
        public Assembly Assembly;

        /// <summary>
        /// The filename, the plugin resists into
        /// </summary>
        public string AssemblyPath;

        /// <summary>
        /// The interface, searched for
        /// </summary>
        public Type Interface;

        /// <summary>
        /// Generate a new plugin data container
        /// </summary>
        /// <param name="Type">The datatype</param>
        /// <param name="Assembly">The plugin's assembly</param>
        /// <param name="AssemblyPath">The plugin's filename</param>
        /// <param name="InterfaceName">The interface, searched for</param>
        public Plugin(Type Type, Assembly Assembly, string AssemblyPath, string InterfaceName)
        {
            this.Type = Type;
            this.Assembly = Assembly;
            this.AssemblyPath = AssemblyPath;
            this.Interface = Type.GetType(InterfaceName);
        }

        /// <summary>
        /// Create a new instance of the plugin
        /// </summary>
        /// <typeparam name="T">The interface/datatype to force</typeparam>
        /// <returns>A new instance of the plugin</returns>
        public T CreateInstance<T>()
        {
            T i = (T)Activator.CreateInstance(this.Assembly.GetType(this.Type.ToString()));
            return i;
        }
    }
}
