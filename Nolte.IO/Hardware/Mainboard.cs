using System;
using System.Collections.Generic;
using System.Management;
using System.Text;

namespace Nolte
{
    public static partial class Hardware
    {
        public static class Mainboard
        {
            private static string serialNumber = String.Empty;
            private static string manufacturer = String.Empty;
            private static string model = String.Empty;
            private static string name = String.Empty;
            public static class BIOS
            {
                internal static string serialNumber = String.Empty;
                internal static string manufacturer = String.Empty;
                internal static string identificationCode = String.Empty;
                internal static string sMBIOSVersion = String.Empty;
                internal static string version = String.Empty;
                internal static string releaseDate = String.Empty;
                /// <summary>
                /// The BIOS' serial number
                /// </summary>
                public static string SerialNumber
                {
                    get
                    {
                        if (Nolte.Hardware.Mainboard.BIOS.serialNumber.Equals(String.Empty))
                        {
                            Nolte.Hardware.Mainboard.ReadInformation();
                        }
                        return Nolte.Hardware.Mainboard.BIOS.serialNumber;
                    }
                }
                /// <summary>
                /// The BIOS Manufacturer
                /// </summary>
                public static string Manufacturer
                {
                    get
                    {
                        if (Nolte.Hardware.Mainboard.BIOS.manufacturer.Equals(String.Empty))
                        {
                            Nolte.Hardware.Mainboard.ReadInformation();
                        }
                        return Nolte.Hardware.Mainboard.BIOS.manufacturer;
                    }
                }
                /// <summary>
                /// The identification code of the BIOS
                /// </summary>
                public static string IdentificationCode
                {
                    get
                    {
                        if (Nolte.Hardware.Mainboard.BIOS.identificationCode.Equals(String.Empty))
                        {
                            Nolte.Hardware.Mainboard.ReadInformation();
                        }
                        return Nolte.Hardware.Mainboard.BIOS.identificationCode;
                    }
                }
                /// <summary>
                /// The SMBIOS' version
                /// </summary>
                public static string SMBIOSVersion
                {
                    get
                    {
                        if (Nolte.Hardware.Mainboard.BIOS.sMBIOSVersion.Equals(String.Empty))
                        {
                            Nolte.Hardware.Mainboard.ReadInformation();
                        }
                        return Nolte.Hardware.Mainboard.BIOS.sMBIOSVersion;
                    }
                }
                /// <summary>
                /// The BIOS' version
                /// </summary>
                public static string Version
                {
                    get
                    {
                        if (Nolte.Hardware.Mainboard.BIOS.version.Equals(String.Empty))
                        {
                            Nolte.Hardware.Mainboard.ReadInformation();
                        }
                        return Nolte.Hardware.Mainboard.BIOS.version;
                    }
                }
                /// <summary>
                /// The BIOS' realease date
                /// </summary>
                public static string ReleaseDate
                {
                    get
                    {
                        if (Nolte.Hardware.Mainboard.BIOS.releaseDate.Equals(String.Empty))
                        {
                            Nolte.Hardware.Mainboard.ReadInformation();
                        }
                        return Nolte.Hardware.Mainboard.BIOS.releaseDate;
                    }
                }
                /// <summary>
                /// Returns an string representation of the BIOS
                /// </summary>
                /// <returns>The calculated string</returns>
                public static new string ToString()
                {
                    if (Nolte.Hardware.Mainboard.BIOS.releaseDate.Equals(String.Empty))
                    {
                        Nolte.Hardware.Mainboard.ReadInformation();
                    }
                    return Nolte.Hardware.Mainboard.BIOS.manufacturer + " Ver " + Nolte.Hardware.Mainboard.BIOS.version + "/" + Nolte.Hardware.Mainboard.BIOS.sMBIOSVersion + " #" + Nolte.Hardware.Mainboard.BIOS.serialNumber + " at " + Nolte.Hardware.Mainboard.BIOS.releaseDate + " (" + Nolte.Hardware.Mainboard.BIOS.identificationCode + ")";
                }
                /// <summary>
                /// A summary hash of all information
                /// </summary>
                public static string AsMD5
                {
                    get
                    {
                        return Nolte.Hardware.Helper.MD5(Nolte.Hardware.Mainboard.BIOS.ToString());
                    }
                }
                /// <summary>
                /// A list of hashes for each single information
                /// </summary>
                public static List<string> AsMD5List
                {
                    get
                    {
                        if (Nolte.Hardware.Mainboard.BIOS.releaseDate.Equals(String.Empty))
                        {
                            Nolte.Hardware.Mainboard.ReadInformation();
                        }
                        List<string> md5List = new List<string>();
                        md5List.Add(Nolte.Hardware.Helper.MD5(Nolte.Hardware.Mainboard.BIOS.identificationCode));
                        md5List.Add(Nolte.Hardware.Helper.MD5(Nolte.Hardware.Mainboard.BIOS.manufacturer));
                        md5List.Add(Nolte.Hardware.Helper.MD5(Nolte.Hardware.Mainboard.BIOS.releaseDate));
                        md5List.Add(Nolte.Hardware.Helper.MD5(Nolte.Hardware.Mainboard.BIOS.serialNumber));
                        md5List.Add(Nolte.Hardware.Helper.MD5(Nolte.Hardware.Mainboard.BIOS.sMBIOSVersion));
                        md5List.Add(Nolte.Hardware.Helper.MD5(Nolte.Hardware.Mainboard.BIOS.version));
                        return md5List;
                    }
                }
            }
            /// <summary>
            /// The mainboard's serial number
            /// </summary>
            public static string SerialNumber
            {
                get
                {
                    if (Nolte.Hardware.Mainboard.serialNumber.Equals(String.Empty))
                    {
                        Nolte.Hardware.Mainboard.ReadInformation();
                    }
                    return Nolte.Hardware.Mainboard.serialNumber;
                }
            }
            /// <summary>
            /// The mainboard's manufacturer
            /// </summary>
            public static string Manufacturer
            {
                get
                {
                    if (Nolte.Hardware.Mainboard.manufacturer.Equals(String.Empty))
                    {
                        Nolte.Hardware.Mainboard.ReadInformation();
                    }
                    return Nolte.Hardware.Mainboard.manufacturer;
                }
            }
            /// <summary>
            /// The mainboard model
            /// </summary>
            public static string Model
            {
                get
                {
                    if (Nolte.Hardware.Mainboard.model.Equals(String.Empty))
                    {
                        Nolte.Hardware.Mainboard.ReadInformation();
                    }
                    return Nolte.Hardware.Mainboard.model;
                }
            }
            /// <summary>
            /// The mainboard's name
            /// </summary>
            public static string Name
            {
                get
                {
                    if (Nolte.Hardware.Mainboard.name.Equals(String.Empty))
                    {
                        Nolte.Hardware.Mainboard.ReadInformation();
                    }
                    return Nolte.Hardware.Mainboard.name;
                }
            }
            private static void ReadInformation()
            {
                ManagementObjectSearcher mbs;
                ManagementObjectCollection mbsList;

                mbs = new ManagementObjectSearcher("Select * From Win32_BaseBoard");
                mbsList = mbs.Get();
                foreach (ManagementObject mo in mbsList)
                {
                    try
                    {
                        Nolte.Hardware.Mainboard.serialNumber = mo["SerialNumber"].ToString();
                    }
                    catch
                    {
                    }
                    try
                    {
                        Nolte.Hardware.Mainboard.manufacturer = mo["Manufacturer"].ToString();
                    }
                    catch
                    {
                    }
                    try
                    {
                        Nolte.Hardware.Mainboard.name = mo["Name"].ToString();
                    }
                    catch
                    {
                    }
                    try
                    {
                        Nolte.Hardware.Mainboard.model = mo["Product"].ToString();
                    }
                    catch
                    {
                    }
                    break;
                }

                mbs = new ManagementObjectSearcher("Select * From Win32_BIOS");
                mbsList = mbs.Get();
                foreach (ManagementObject mo in mbsList)
                {
                    try
                    {
                        Nolte.Hardware.Mainboard.BIOS.serialNumber = mo["SerialNumber"].ToString();
                    }
                    catch
                    {
                    }
                    try
                    {
                        Nolte.Hardware.Mainboard.BIOS.manufacturer = mo["Manufacturer"].ToString();
                    }
                    catch
                    {
                    }
                    try
                    {
                        Nolte.Hardware.Mainboard.BIOS.sMBIOSVersion = mo["SMBIOSBIOSVersion"].ToString();
                    }
                    catch
                    {
                    }
                    try
                    {
                        Nolte.Hardware.Mainboard.BIOS.version = mo["Version"].ToString();
                    }
                    catch
                    {
                    }
                    try
                    {
                        Nolte.Hardware.Mainboard.BIOS.identificationCode = mo["IdentificationCode"].ToString();
                    }
                    catch
                    {
                    }
                    try
                    {
                        Nolte.Hardware.Mainboard.BIOS.releaseDate = mo["ReleaseDate"].ToString();
                    }
                    catch
                    {
                    }
                    break;
                }
            }
            /// <summary>
            /// Returns an string representation of teh mainboard
            /// </summary>
            /// <returns>The calculated string</returns>
            public static new string ToString()
            {
                if (Nolte.Hardware.Mainboard.name.Equals(String.Empty))
                {
                    Nolte.Hardware.Mainboard.ReadInformation();
                } 
                return Nolte.Hardware.Mainboard.manufacturer + " " + Nolte.Hardware.Mainboard.model + " (" + Nolte.Hardware.Mainboard.name + ") #" + Nolte.Hardware.Mainboard.BIOS.serialNumber;
            }
            /// <summary>
            /// A summary hash of all information
            /// </summary>
            public static string AsMD5
            {
                get
                {
                    return Nolte.Hardware.Helper.MD5(Nolte.Hardware.Mainboard.ToString());
                }
            }
            /// <summary>
            /// A list of hashes for each single information
            /// </summary>
            public static List<string> AsMD5List
            {
                get
                {
                    if (Nolte.Hardware.Mainboard.name.Equals(String.Empty))
                    {
                        Nolte.Hardware.Mainboard.ReadInformation();
                    }
                    List<string> md5List = new List<string>();
                    md5List.Add(Nolte.Hardware.Helper.MD5(Nolte.Hardware.Mainboard.name));
                    md5List.Add(Nolte.Hardware.Helper.MD5(Nolte.Hardware.Mainboard.manufacturer));
                    md5List.Add(Nolte.Hardware.Helper.MD5(Nolte.Hardware.Mainboard.model));
                    md5List.Add(Nolte.Hardware.Helper.MD5(Nolte.Hardware.Mainboard.serialNumber));
                    return md5List;
                }
            }
        }
    }
}
