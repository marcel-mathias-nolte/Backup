using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Nolte.UI
{
    [Serializable()]
    public class VirtualFile : System.Runtime.Serialization.ISerializable
    {
        public Image smallIcon;
        public Image largeIcon;
        public string Name;
        public string Source;
        public byte[] Data;
        public DateTime Filedate;
        public DateTime VirtualFiledate;
        public VirtualFile(Image smallIcon, Image largeIcon, string Name, string Source, byte[] Data, DateTime Filedate, DateTime VirtualFiledate)
        {
            this.smallIcon = smallIcon;
            this.largeIcon = largeIcon;
            this.Name = Name;
            this.Source = Source;
            this.Data = Data;
            this.Filedate = Filedate;
            this.VirtualFiledate = VirtualFiledate;
        }
        public VirtualFile(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext ctxt)
        {
            this.smallIcon = (Image)info.GetValue("smallIcon", typeof(Image));
            this.largeIcon = (Image)info.GetValue("largeIcon", typeof(Image));
            this.Name = (string)info.GetValue("Name", typeof(string));
            this.Source = (string)info.GetValue("Source", typeof(string));
            this.Data = (byte[])info.GetValue("Data", typeof(byte[]));
            this.Filedate = (DateTime)info.GetValue("Filedate", typeof(DateTime));
            this.VirtualFiledate = (DateTime)info.GetValue("VirtualFiledate", typeof(DateTime));
        }
        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext ctxt)
        {
            info.AddValue("Source", this.Source);
            info.AddValue("smallIcon", this.smallIcon);
            info.AddValue("largeIcon", this.largeIcon);
            info.AddValue("Name", this.Name);
            info.AddValue("Data", this.Data);
            info.AddValue("VirtualFiledate", this.VirtualFiledate);
            info.AddValue("Filedate", this.Filedate);
        }
        public void AddToListView(ListView o)
        {
            o.SmallImageList.Images.Add(smallIcon);
            o.LargeImageList.Images.Add(largeIcon);
            o.Items.Add(Name, o.SmallImageList.Images.Count - 1);
            //o.Items[o.Items.Count - 1].SubItems.Add(Name);
            o.Items[o.Items.Count - 1].SubItems.Add(Math.Ceiling((float)Data.LongLength / 1024f).ToString() + " kB");
            o.Items[o.Items.Count - 1].SubItems.Add(Filedate.ToString());
            o.Items[o.Items.Count - 1].SubItems.Add(VirtualFiledate.ToString());
            o.Items[o.Items.Count - 1].Tag = this;
        }
        public string Extension
        {
            get
            {
                return Name.Substring(Name.LastIndexOf('.') + 1);
            }
        }
        public string FileDescription
        {
            get
            {
                string type = Extension.ToUpper() + "-Datei";
                try
                {
                    RegistryKey rkRoot = Registry.ClassesRoot;
                    RegistryKey key = rkRoot.OpenSubKey("." + Extension);
                    type = key.GetValue("").ToString();
                    key = rkRoot.OpenSubKey(type);
                    type = key.GetValue("").ToString();
                }
                catch
                {
                }
                return type;
            }
        }
        public string ContentType
        {
            get
            {
                string type = "application/binary";
                try
                {
                    RegistryKey rkRoot = Registry.ClassesRoot;
                    RegistryKey key = rkRoot.OpenSubKey("." + Extension);
                    type = key.GetValue("").ToString();
                    key = rkRoot.OpenSubKey(type);
                    type = key.GetValue("Content Type").ToString();
                }
                catch
                {
                }
                return type;
            }
        }
        public string AssociatedOpenCommand
        {
            get
            {
                string command = "";
                try
                {
                    RegistryKey rkRoot = Registry.ClassesRoot;
                    RegistryKey key = rkRoot.OpenSubKey("." + Extension);
                    command = key.GetValue("").ToString();
                    key = rkRoot.OpenSubKey(command);
                    key = key.OpenSubKey("shell");
                    key = key.OpenSubKey("open");
                    key = key.OpenSubKey("command");
                    command = key.GetValue("").ToString();
                }
                catch
                {
                    command = "";
                }
                return command;
            }
        }
    }

}
