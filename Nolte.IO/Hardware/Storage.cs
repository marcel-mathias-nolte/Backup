using System;
using System.Collections.Generic;
using System.Management;
using System.Text;

namespace Nolte
{
    public static partial class Hardware
    {
        public static class Storage
        {
            private static string volumeSerial = String.Empty;
            public static string VolumeSerial
            {
                get
                {
                    if (Nolte.Hardware.Storage.volumeSerial.Equals(String.Empty))
                    {
                        string drive = Environment.GetEnvironmentVariable("SystemDrive").Substring(0, 1);
                        ManagementObject dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + drive + @":""");
                        dsk.Get();
                        Nolte.Hardware.Storage.volumeSerial = dsk["VolumeSerialNumber"].ToString();
                    }
                    return Nolte.Hardware.Storage.volumeSerial;
                }
            }
        }
        private static Nolte.Hardware.Helper.DiskDriveInformationIndexer diskDriveInformationIndexer = new Nolte.Hardware.Helper.DiskDriveInformationIndexer();
        /// <summary>
        /// Information about the disk drive(s)
        /// </summary>
        /// <param name="index">The number of the disk drive (starting at 0)</param>
        /// <returns>The disk drive's information set</returns>
        public static Nolte.Hardware.Helper.DiskDriveInformationIndexer DiskDrives
        {
            get
            {
                if (Nolte.Hardware.diskDriveInformationIndexer.Count == 0)
                {
                    Nolte.Hardware.ReadDiskDriveInformation();
                }
                return Nolte.Hardware.diskDriveInformationIndexer;
            }
        }
        public static partial class Helper
        {
            /// <summary>
            /// Helper clas for providing static indexers
            /// </summary>
            public sealed class DiskDriveInformationIndexer
            {
                private List<Nolte.Hardware.Helper.DiskDriveInformation> diskDriveInfo = new List<Nolte.Hardware.Helper.DiskDriveInformation>();
                /// <summary>
                /// Returns the specified processor's information set
                /// </summary>
                /// <param name="index">the processor number (starting from 0)</param>
                /// <returns>the choosen information set</returns>
                public Nolte.Hardware.Helper.DiskDriveInformation this[int index]
                {
                    get
                    {
                        return this.diskDriveInfo[index];
                    }
                }
                /// <summary>
                /// The number of processors found
                /// </summary>
                public int Count
                {
                    get
                    {
                        return this.diskDriveInfo.Count;
                    }
                }
                internal void Add(Nolte.Hardware.Helper.DiskDriveInformation Info)
                {
                    this.diskDriveInfo.Add(Info);
                }
            }
            /// <summary>
            /// Information about a disk drive
            /// </summary>
            public sealed class DiskDriveInformation
            {
                private string model;
                private string manufacturer;
                private string signature;
                private string serialNumber;
                private int totalCylinders;
                private long totalSectors;
                private int totalTracks;
                private int totalHeads;
                private int tracksPerCylinder;
                private int bytesPerSector;
                private int sectorsPerTrack;
                private long totalBytes;
                private string firmwareRevision;
                private string interfaceType;
                private string caption;
                /// <summary>
                /// Creates a new disk drive's information set
                /// </summary>
                /// <param name="Model">The disk drive's model</param>
                /// <param name="Manufacturer">The disk drive's manufacturer</param>
                /// <param name="Signature">The disk drive's signature</param>
                /// <param name="SerialNumber">The disk drive's serial number</param>
                /// <param name="TotalCylinders">Number of cylinders on the disk drive</param>
                /// <param name="TotalHeads">Number of heads on the disk drive</param>
                /// <param name="TotalSectors">Number of sectors on the disk drive</param>
                /// <param name="TotalTracks">Number of tracks on the disk drive</param>
                /// <param name="TracksPerCylinder">Number of tracks per cylinder on the disk drive</param>
                /// <param name="SectorsPerTrack">Number of sectors per track on the disk drive</param>
                /// <param name="BytesPerSector">Number of bytes per sector on the disk drive</param>
                /// <param name="TotalBytes">Number of bytes on the disk drive</param>
                /// <param name="FirmwareRevision">The disk drive's firmware revision</param>
                /// <param name="InterfaceType">The disk drive's interface type</param>
                /// <param name="Caption">The disk drive's caption</param>
                public DiskDriveInformation(string Model, string Manufacturer, string Signature, string SerialNumber, string TotalCylinders, string TotalHeads, string TotalSectors, string TotalTracks, string TracksPerCylinder, string SectorsPerTrack, string BytesPerSector, string TotalBytes, string FirmwareRevision, string InterfaceType, string Caption)
                {
                    this.model = Model;
                    this.manufacturer = Manufacturer;
                    this.signature = Signature;
                    this.serialNumber = SerialNumber;
                    this.totalCylinders = TotalCylinders.Equals(String.Empty) ? 0 : Int32.Parse(TotalCylinders);
                    this.totalHeads = TotalHeads.Equals(String.Empty) ? 0 : Int32.Parse(TotalHeads);
                    this.totalSectors = TotalSectors.Equals(String.Empty) ? 0 : Int64.Parse(TotalSectors);
                    this.totalTracks = TotalTracks.Equals(String.Empty) ? 0 : Int32.Parse(TotalTracks);
                    this.tracksPerCylinder = TracksPerCylinder.Equals(String.Empty) ? 0 : Int32.Parse(TracksPerCylinder);
                    this.sectorsPerTrack = SectorsPerTrack.Equals(String.Empty) ? 0 : Int32.Parse(SectorsPerTrack);
                    this.bytesPerSector = BytesPerSector.Equals(String.Empty) ? 0 : Int32.Parse(BytesPerSector);
                    this.totalBytes = TotalBytes.Equals(String.Empty) ? 0 : Int64.Parse(TotalBytes);
                    this.firmwareRevision = FirmwareRevision;
                    this.interfaceType = InterfaceType;
                    this.caption = Caption;
                }
                /// <summary>
                /// The disk drive's model
                /// </summary>
                public string Model
                {
                    get
                    {
                        return this.model;
                    }
                }
                /// <summary>
                /// The disk drive's manufacturer
                /// </summary>
                public string Manufacturer
                {
                    get
                    {
                        return this.manufacturer;
                    }
                }
                /// <summary>
                /// The disk drive's signature
                /// </summary>
                public string Signature
                {
                    get
                    {
                        return this.signature;
                    }
                }
                /// <summary>
                /// The disk drive's serial number
                /// </summary>
                public string SerialNumber
                {
                    get
                    {
                        return this.serialNumber;
                    }
                }
                /// <summary>
                /// Number of cylinders on the disk drive
                /// </summary>
                public int TotalCylinders
                {
                    get
                    {
                        return this.totalCylinders;
                    }
                }
                /// <summary>
                /// Number of heads on the disk drive
                /// </summary>
                public int TotalHeads
                {
                    get
                    {
                        return this.totalHeads;
                    }
                }
                /// <summary>
                /// Number of sectors on the disk drive
                /// </summary>
                public long TotalSectors
                {
                    get
                    {
                        return this.totalSectors;
                    }
                }
                /// <summary>
                /// Number of tracks on the disk drive
                /// </summary>
                public int TotalTracks
                {
                    get
                    {
                        return this.totalTracks;
                    }
                }
                /// <summary>
                /// Number of tracks per cylinder on the disk drive
                /// </summary>
                public int TracksPerCylinder
                {
                    get
                    {
                        return this.tracksPerCylinder;
                    }
                }
                /// <summary>
                /// Number of sectors per track on the disk drive
                /// </summary>
                public int SectorsPerTrack
                {
                    get
                    {
                        return this.sectorsPerTrack;
                    }
                }
                /// <summary>
                /// Number of bytes per sector on the disk drive
                /// </summary>
                public int BytesPerSector
                {
                    get
                    {
                        return this.bytesPerSector;
                    }
                }
                /// <summary>
                /// Number of bytes on the disk drive
                /// </summary>
                public long TotalBytes
                {
                    get
                    {
                        return this.totalBytes;
                    }
                }
                /// <summary>
                /// The disk drive's firmware revision
                /// </summary>
                public string FirmwareRevision
                {
                    get
                    {
                        return this.firmwareRevision;
                    }
                }
                /// <summary>
                /// The disk drive's interface type
                /// </summary>
                public string InterfaceType
                {
                    get
                    {
                        return this.interfaceType;
                    }
                }
                /// <summary>
                /// The disk drive's caption
                /// </summary>
                public string Caption
                {
                    get
                    {
                        return this.caption;
                    }
                }
                /// <summary>
                /// Returns an string representation of this disk drive information set
                /// </summary>
                /// <returns>The calculated string</returns>
                public override string ToString()
                {
                    return this.Caption + " (" + this.manufacturer + " " + this.model + ") @" + this.interfaceType + " Rev. " + this.firmwareRevision + " #" + this.serialNumber + " #" + this.signature + " CHS " + this.totalCylinders.ToString() + "/" + this.totalHeads.ToString() + "/" + this.totalSectors.ToString() + " on " + this.totalTracks.ToString() + " tracks with " + this.tracksPerCylinder.ToString() + " tracks per head, " + this.sectorsPerTrack.ToString() + " sectors per track and " + this.bytesPerSector.ToString() + " bytes per sector are " + this.totalBytes.ToString() + " bytes";
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
                        md5List.Add(Nolte.Hardware.Helper.MD5(this.caption));
                        md5List.Add(Nolte.Hardware.Helper.MD5(this.firmwareRevision));
                        md5List.Add(Nolte.Hardware.Helper.MD5(this.interfaceType));
                        md5List.Add(Nolte.Hardware.Helper.MD5(this.manufacturer));
                        md5List.Add(Nolte.Hardware.Helper.MD5(this.model));
                        md5List.Add(Nolte.Hardware.Helper.MD5(this.serialNumber));
                        md5List.Add(Nolte.Hardware.Helper.MD5(this.signature));
                        md5List.Add(Nolte.Hardware.Helper.MD5(this.totalBytes.ToString()));
                        md5List.Add(Nolte.Hardware.Helper.MD5(this.totalCylinders.ToString() + "/" + this.totalHeads.ToString() + "/" + this.totalSectors.ToString()));
                        md5List.Add(Nolte.Hardware.Helper.MD5(this.totalTracks.ToString()));
                        return md5List;
                    }
                }
            }
        }
        private static void ReadDiskDriveInformation()
        {
            ManagementClass mc = new ManagementClass("Win32_DiskDrive");
            ManagementObjectCollection moc = mc.GetInstances();
            string a = "", b = "", c = "", d = "", e = "", f = "", g = "", h = "", i = "", j = "", k = "", /*l = "",*/ m = "", n = "", o = "", p = "";
            foreach (ManagementObject mo in moc)
            {
                try
                {
                    a = mo.Properties["Model"].Value.ToString();
                }
                catch
                {
                }
                try
                {
                    b = mo.Properties["Manufacturer"].Value.ToString();
                }
                catch
                {
                }
                try
                {
                    c = mo.Properties["Signature"].Value.ToString();
                }
                catch
                {
                }
                try
                {
                    d = mo.Properties["SerialNumber"].Value.ToString();
                }
                catch
                {
                }
                try
                {
                    e = mo.Properties["TotalCylinders"].Value.ToString();
                }
                catch
                {
                }
                try
                {
                    f = mo.Properties["TotalHeads"].Value.ToString();
                }
                catch
                {
                }
                try
                {
                    g = mo.Properties["TotalSectors"].Value.ToString();
                }
                catch
                {
                }
                try
                {
                    h = mo.Properties["TotalTracks"].Value.ToString();
                }
                catch
                {
                }
                try
                {
                    i = mo.Properties["TracksPerCylinder"].Value.ToString();
                }
                catch
                {
                }
                try
                {
                    j = mo.Properties["SectorsPerTrack"].Value.ToString();
                }
                catch
                {
                }
                try
                {
                    k = mo.Properties["BytesPerSector"].Value.ToString();
                }
                catch
                {
                }
                try
                {
                    m = mo.Properties["Size"].Value.ToString();
                }
                catch
                {
                }
                try
                {
                    n = mo.Properties["FirmwareRevision"].Value.ToString();
                }
                catch
                {
                }
                try
                {
                    o = mo.Properties["InterfaceType"].Value.ToString();
                }
                catch
                {
                }
                try
                {
                    p = mo.Properties["Caption"].Value.ToString();
                }
                catch
                {
                } 
                Nolte.Hardware.Helper.DiskDriveInformation ddi = new Nolte.Hardware.Helper.DiskDriveInformation(a, b, c, d, e, f, g, h, i, j, k, m, n, o, p);
                Nolte.Hardware.diskDriveInformationIndexer.Add(ddi);
            }
        }
    }
}
