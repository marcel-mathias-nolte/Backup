using System;
using System.Collections.Generic;
using System.Management;
using System.Text;

namespace Nolte
{
    public static partial class Hardware
    {
        private static Nolte.Hardware.Helper.ProcessorInformationIndexer processorInformationIndexer = new Nolte.Hardware.Helper.ProcessorInformationIndexer();
        /// <summary>
        /// Information about the processor(s)
        /// </summary>
        /// <param name="index">The number of the processor (starting at 0)</param>
        /// <returns>The processor's information set</returns>
        public static Nolte.Hardware.Helper.ProcessorInformationIndexer Processors
        {
            get
            {
                if (Nolte.Hardware.processorInformationIndexer.Count == 0)
                {
                    Nolte.Hardware.ReadProcessorInformation();
                }
                return Nolte.Hardware.processorInformationIndexer;
            }
        }
        public static partial class Helper
        {
            /// <summary>
            /// Helper clas for providing static indexers
            /// </summary>
            public sealed class ProcessorInformationIndexer
            {
                private List<Nolte.Hardware.Helper.ProcessorInformation> processorInfo = new List<Nolte.Hardware.Helper.ProcessorInformation>();
                /// <summary>
                /// Returns the specified processor's information set
                /// </summary>
                /// <param name="index">the processor number (starting from 0)</param>
                /// <returns>the choosen information set</returns>
                public Nolte.Hardware.Helper.ProcessorInformation this[int index]
                {
                    get
                    {
                        return this.processorInfo[index];
                    }
                }
                /// <summary>
                /// The number of processors found
                /// </summary>
                public int Count
                {
                    get
                    {
                        return this.processorInfo.Count;
                    }
                }
                internal void Add(Nolte.Hardware.Helper.ProcessorInformation Info)
                {
                    this.processorInfo.Add(Info);
                }
            }
            /// <summary>
            /// Information about a processor
            /// </summary>
            public sealed class ProcessorInformation
            {
                private string id;
                private string maxClockSpeed;
                private string modelName;
                /// <summary>
                /// Creates a new processor's information set
                /// </summary>
                /// <param name="ID">The processor's id</param>
                /// <param name="MaxClockSpeed">The processor's maximum speed</param>
                /// <param name="ModelName">The processor's model name</param>
                public ProcessorInformation(string ID, string MaxClockSpeed, string ModelName)
                {
                    this.id = ID;
                    this.maxClockSpeed = MaxClockSpeed;
                    this.modelName = ModelName;
                }
                /// <summary>
                /// The processor's id
                /// </summary>
                public string ID
                {
                    get
                    {
                        return this.id;
                    }
                }
                /// <summary>
                /// The processor's maximum speed
                /// </summary>
                public string MaxClockSpeed
                {
                    get
                    {
                        return this.maxClockSpeed;
                    }
                }
                /// <summary>
                /// The processor's model name
                /// </summary>
                public string ModelName
                {
                    get
                    {
                        return this.modelName;
                    }
                }
                /// <summary>
                /// Returns an string representation of this processor information set
                /// </summary>
                /// <returns>The calculated string</returns>
                public override string ToString()
                {
                    return this.modelName + " #" + this.id + " @" + this.maxClockSpeed;
                }
                /// <summary>
                /// A summary hash of all information
                /// </summary>
                public string AsMD5
                {
                    get
                    {
                        return Nolte.Hardware.Helper.MD5(this.ToString());
                    }
                }
                /// <summary>
                /// A list of hashes for each single information
                /// </summary>
                public List<string> AsMD5List
                {
                    get
                    {
                        List<string> md5List = new List<string>();
                        md5List.Add(Nolte.Hardware.Helper.MD5(this.modelName));
                        md5List.Add(Nolte.Hardware.Helper.MD5(this.id));
                        md5List.Add(Nolte.Hardware.Helper.MD5(this.maxClockSpeed));
                        return md5List;
                    }
                }
            }
        }
        private static void ReadProcessorInformation()
        {
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                Nolte.Hardware.Helper.ProcessorInformation pi = new Nolte.Hardware.Helper.ProcessorInformation(mo.Properties["processorID"].Value.ToString(), mo.Properties["MaxClockSpeed"].Value.ToString(), mo.Properties["Name"].Value.ToString());
                Nolte.Hardware.processorInformationIndexer.Add(pi);
            }
        }
    }
}
