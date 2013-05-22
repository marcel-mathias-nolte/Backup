using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Nolte.UI
{
    public partial class VirtualFolder : UserControl //, PasswortManager.IMainMenuCapable
    {
        private Dictionary<string, int> imageExtensionCache = new Dictionary<string, int>();
        public VirtualFolder()
        {
            InitializeComponent();
            listView1.AfterLabelEdit += new LabelEditEventHandler(listView1_AfterLabelEdit);
        }

        void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            ((VirtualFile)listView1.Items[e.Item].Tag).Name = e.Label;
            listView1.LabelEdit = false;
        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (string file in files)
            {
                AddFile(file);
            }
        }
        private SystemImageList l = new SystemImageList(SystemImageListSize.ExtraLargeIcons);
        /// <summary>
        ///   Copies data from a source stream to a target stream.</summary>
        /// <param name="source">
        ///   The source stream to copy from.</param>
        /// <param name="target">
        ///   The destination stream to copy to.</param>
        public void AddFile(string File)
        {
            byte[] data;
            int imageIndex = 0;
            DateTime Filedate;
            if (System.IO.Directory.Exists(File))
            {
                string fileName;
                // ist ein verzeichnis
                do
                {
                    fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString();
                }
                while (System.IO.Directory.Exists(fileName));
                System.IO.Directory.CreateDirectory(fileName);
                fileName += "\\" + File.Substring(File.LastIndexOf('\\') + 1) + ".zip";
                using (Zip zip = Zip.Create(fileName, "Original source: " + File))
                {
                    zip.AddFolder(Zip.Compression.Deflate, File, "");
                    //zip.Close();
                }
                data = System.IO.File.ReadAllBytes(fileName);
                imageIndex = ReadIcons(fileName);
                System.IO.File.Delete(fileName);
                System.IO.Directory.Delete(fileName.Substring(0, fileName.LastIndexOf('\\')));
                Filedate = System.IO.Directory.GetLastWriteTime(File);
            }
            else
            {
                data = System.IO.File.ReadAllBytes(File);
                imageIndex = ReadIcons(File);
                Filedate = System.IO.File.GetLastWriteTime(File);
            }
            files.Add(new VirtualFile(smallImageList.Images[imageIndex], largeImageList.Images[imageIndex], File.Substring(File.LastIndexOf("\\") + 1), File, data, Filedate, DateTime.Now));
            files[files.Count - 1].AddToListView(listView1);
        }
        private List<VirtualFile> files = new List<VirtualFile>();
        private int ReadIcons(string File)
        {
            string ext = File.Substring(File.LastIndexOf('.') + 1).ToLower();
            if (!imageExtensionCache.ContainsKey(ext) || ext == "exe")
            {
                Api.SHFILEINFO shinfo = new Api.SHFILEINFO();
                IntPtr hImgSmall = Api.SHGetFileInfo(File, 0, ref shinfo, (uint)System.Runtime.InteropServices.Marshal.SizeOf(shinfo), Api.SHGFI_ICON | Api.SHGFI_SMALLICON);
                smallImageList.Images.Add(Icon.FromHandle(shinfo.hIcon));
                Api.DestroyIcon(shinfo.hIcon);

                //largeImageList.Images.Add(l.Icon(l.IconIndex(File)));

                shinfo = new Api.SHFILEINFO();
                IntPtr hImgLarge = Api.SHGetFileInfo(File, 0, ref shinfo, (uint)System.Runtime.InteropServices.Marshal.SizeOf(shinfo), Api.SHGFI_ICON | Api.SHGFI_LARGEICON);
                largeImageList.Images.Add(Icon.FromHandle(shinfo.hIcon));
                Api.DestroyIcon(shinfo.hIcon);

                if (ext != "exe")
                {
                    imageExtensionCache.Add(ext, smallImageList.Images.Count - 1);
                }
                return smallImageList.Images.Count - 1;
            }
            else
            {
                return imageExtensionCache[ext];
            }
        }
        private void changeDisplayStyle(object sender, EventArgs e)
        {
            großeSymboleToolStripMenuItem.Checked = false;
            kleineSymboleToolStripMenuItem.Checked = false;
            listeToolStripMenuItem.Checked = false;
            detailsToolStripMenuItem.Checked = false;
            kachelnToolStripMenuItem.Checked = false;
            ((ToolStripMenuItem)sender).Checked = true;
            if (sender == großeSymboleToolStripMenuItem)
            {
                listView1.View = View.LargeIcon;
            }
            if (sender == kleineSymboleToolStripMenuItem)
            {
                listView1.View = View.SmallIcon;
            }
            if (sender == listeToolStripMenuItem)
            {
                listView1.View = View.List;
            }
            if (sender == detailsToolStripMenuItem)
            {
                listView1.View = View.Details;
            }
            if (sender == kachelnToolStripMenuItem)
            {
                listView1.View = View.Tile;
            }
        }
        public string Serialized
        {
            get
            {
                string result = "";
                foreach (ListViewItem i in listView1.Items)
                {
                    try
                    {
                        using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                        {
                            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                            bFormatter.Serialize(ms, (VirtualFile)i.Tag);
                            result += Convert.ToBase64String(ms.ToArray()) + " ";
                        }
                    }
                    catch
                    {
                    }
                }
                result = result.TrimEnd(' ') + "\0" + this.listView1.Columns[0].Width.ToString() + "\0" + this.listView1.Columns[1].Width.ToString() + "\0" + this.listView1.Columns[2].Width.ToString() + "\0" + this.listView1.Columns[3].Width.ToString() + "\0" + ((int)listView1.View).ToString();
                return result;
            }
            set
            {
                listView1.Items.Clear();
                string[] settings = value.Split('\0');
                string[] data = settings[0].Split(' ');
                VirtualFile f;
                foreach (string val in data)
                {
                    try
                    {
                        using (System.IO.MemoryStream ms = new System.IO.MemoryStream(Convert.FromBase64String(val)))
                        {
                            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                            f = (VirtualFile)bFormatter.Deserialize(ms);
                            f.AddToListView(listView1);
                            files.Add(f);
                        }
                    }
                    catch
                    {
                    }
                }
                if (settings.Length > 1)
                {
                    listView1.Columns[0].Width = Int32.Parse(settings[1]);
                    listView1.Columns[1].Width = Int32.Parse(settings[2]);
                    listView1.Columns[2].Width = Int32.Parse(settings[3]);
                    listView1.Columns[3].Width = Int32.Parse(settings[4]);
                    Layout = settings.Length > 5 ? (View)Int32.Parse(settings[5]) : View.LargeIcon;
                }
            }
        }

        public new View Layout
        {
            get
            {
                return listView1.View;
            }
            set
            {
                listView1.View = value;
                großeSymboleToolStripMenuItem.Checked = false;
                kleineSymboleToolStripMenuItem.Checked = false;
                listeToolStripMenuItem.Checked = false;
                detailsToolStripMenuItem.Checked = false;
                kachelnToolStripMenuItem.Checked = false;
                switch (listView1.View)
                {
                    case View.LargeIcon:
                        großeSymboleToolStripMenuItem.Checked = true;
                        break;
                    case View.SmallIcon:
                        kleineSymboleToolStripMenuItem.Checked = true;
                        break;
                    case View.List:
                        listeToolStripMenuItem.Checked = true;
                        break;
                    case View.Details:
                        detailsToolStripMenuItem.Checked = true;
                        break;
                    case View.Tile:
                        kachelnToolStripMenuItem.Checked = true;
                        break;
                }
            }
        }
        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                //Point p = listView1.PointToScreen(e.Location);
                ListViewItem i = listView1.GetItemAt(e.X, e.Y);
                if (i != null)
                {
                    itemContextMenu.Tag = i;
                    öffnenToolStripMenuItem.Visible = ((VirtualFile)i.Tag).AssociatedOpenCommand != "";
                    itemContextMenu.Show(listView1, e.Location);
                }
            }
        }

        private void löschenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem i = (ListViewItem)itemContextMenu.Tag;
            listView1.Items.Remove(i);
            files.Remove((VirtualFile)i.Tag);
        }

        private void speichernUnterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                ListViewItem i = (ListViewItem)itemContextMenu.Tag;
                VirtualFile f = (VirtualFile)i.Tag;
                sfd.Filter = f.FileDescription + " (*." + f.Extension + ")|*." + f.Extension;
                sfd.FileName = f.Source.Substring(0, f.Source.LastIndexOf('\\') + 1) + f.Name;
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    System.IO.File.WriteAllBytes(sfd.FileName, f.Data);
                }
            }
        }

        private void öffnenMitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName;
            ListViewItem i = (ListViewItem)itemContextMenu.Tag;
            VirtualFile f = (VirtualFile)i.Tag;
            do
            {
                fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + "." + f.Extension;
            }
            while (System.IO.File.Exists(fileName));
            System.IO.File.WriteAllBytes(fileName, f.Data);
            var args = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.System), "shell32.dll");
            args += ",OpenAs_RunDLL " + fileName;
            System.Diagnostics.Process p = System.Diagnostics.Process.Start("rundll32.exe", args);
            while (!p.HasExited)
            {
                Application.DoEvents();
            }
            if (MessageBox.Show("Soll die Datei wieder importiert werden?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (System.IO.File.Exists(fileName))
                {
                    try
                    {
                        f.Data = System.IO.File.ReadAllBytes(fileName);
                        f.Filedate = System.IO.File.GetLastWriteTime(fileName);
                        i.SubItems[0].Text = Math.Ceiling((float)f.Data.LongLength / 1024f).ToString() + " kB";
                        i.SubItems[1].Text = f.Filedate.ToString();
                    }
                    catch
                    {
                        MessageBox.Show("Die Datei konnte nicht gelesen werden!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    try
                    {
                        System.IO.File.Delete(fileName);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    MessageBox.Show("Die Datei existiert nicht mehr!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void öffnenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem i = (ListViewItem)itemContextMenu.Tag;
            VirtualFile f = (VirtualFile)i.Tag;
            string command = f.AssociatedOpenCommand;
            if (command != "")
            {
                string fileName;
                do
                {
                    fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + "." + f.Extension;
                }
                while (System.IO.File.Exists(fileName));
                System.IO.File.WriteAllBytes(fileName, f.Data);

                System.Diagnostics.Process p = System.Diagnostics.Process.Start(fileName);
                while (!p.HasExited)
                {
                    Application.DoEvents();
                }
                if (MessageBox.Show("Soll die Datei wieder importiert werden?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (System.IO.File.Exists(fileName))
                    {
                        try
                        {
                            f.Data = System.IO.File.ReadAllBytes(fileName);
                            f.Filedate = System.IO.File.GetLastWriteTime(fileName);
                            i.SubItems[0].Text = Math.Ceiling((float)f.Data.LongLength / 1024f).ToString() + " kB";
                            i.SubItems[1].Text = f.Filedate.ToString();
                        }
                        catch
                        {
                            MessageBox.Show("Die Datei konnte nicht gelesen werden!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        try
                        {
                            System.IO.File.Delete(fileName);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        MessageBox.Show("Die Datei existiert nicht mehr!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

            }
            else
            {
                öffnenMitToolStripMenuItem_Click(sender, e);
            }
        }

        public void DoAddFolder()
        {
            toolStripButton3_Click(null, EventArgs.Empty);
        }

        public void DoAddFile()
        {
            toolStripButton2_Click(null, EventArgs.Empty);
        }

        public void DoEdit()
        {
            if (listView1.SelectedItems.Count == 1)
            {
                listView1.LabelEdit = true;
                listView1.SelectedItems[0].BeginEdit();
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Multiselect = true;
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    foreach (string s in ofd.FileNames)
                    {
                        AddFile(s);
                    }
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog(this) == DialogResult.OK)
                {
                    AddFile(fbd.SelectedPath);
                }
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                //Point p = listView1.PointToScreen(e.Location);
                ListViewItem i = listView1.GetItemAt(e.X, e.Y);
                if (i != null)
                {
                    VirtualFile f = (VirtualFile)i.Tag;
                    string command = f.AssociatedOpenCommand;
                    string fileName;
                    do
                    {
                        fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + "." + f.Extension;
                    }
                    while (System.IO.File.Exists(fileName));
                    System.IO.File.WriteAllBytes(fileName, f.Data);
                    System.Diagnostics.Process p;
                    if (command != "")
                    {
                        p = System.Diagnostics.Process.Start(fileName);
                    }
                    else
                    {
                        var args = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.System), "shell32.dll");
                        args += ",OpenAs_RunDLL " + fileName;
                        p = System.Diagnostics.Process.Start("rundll32.exe", args);
                    }
                    while (!p.HasExited)
                    {
                        Application.DoEvents();
                    }
                    if (MessageBox.Show("Soll die Datei wieder importiert werden?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (System.IO.File.Exists(fileName))
                        {
                            try
                            {
                                f.Data = System.IO.File.ReadAllBytes(fileName);
                                f.Filedate = System.IO.File.GetLastWriteTime(fileName);
                                i.SubItems[0].Text = Math.Ceiling((float)f.Data.LongLength / 1024f).ToString() + " kB";
                                i.SubItems[1].Text = f.Filedate.ToString();
                            }
                            catch
                            {
                                MessageBox.Show("Die Datei konnte nicht gelesen werden!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            try
                            {
                                System.IO.File.Delete(fileName);
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            MessageBox.Show("Die Datei existiert nicht mehr!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
        }

        private void umbenennenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem i = (ListViewItem)itemContextMenu.Tag;
            listView1.LabelEdit = true;
            i.BeginEdit();
        }

    }
}
