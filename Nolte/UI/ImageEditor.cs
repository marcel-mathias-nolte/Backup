using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Nolte.UI
{
    public partial class ImageEditor : System.Windows.Forms.UserControl, IMainMenuCapable
    {
        public ImageEditor()
        {
            this.InitializeComponent();

            this.menuItems.Add("Datei", new System.Collections.Generic.List<System.Windows.Forms.ToolStripMenuItem>());

            this.mnuOpenWith = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOpenWith.Image = global::Nolte.UI.Resources.palette_paint_brush;
            this.mnuOpenWith.Name = "mnuOpenWith";
            this.mnuOpenWith.Size = new System.Drawing.Size(242, 22);
            this.mnuOpenWith.Text = "Mit externem Programm öffnen";
            this.mnuOpenWith.Visible = false;
            this.mnuOpenWith.Click += new System.EventHandler(this.doOpenWith);
            this.menuItems["Datei"].Add(this.mnuOpenWith);
            
            this.mnuImport = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuImport.Image = global::Nolte.UI.Resources.folder_open_document;
            this.mnuImport.Name = "mnuImport";
            this.mnuImport.Size = new System.Drawing.Size(242, 22);
            this.mnuImport.Text = "Öffnen / Importieren";
            this.mnuImport.Click += new System.EventHandler(this.doImport);
            this.menuItems["Datei"].Add(this.mnuImport);

            this.mnuExport = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExport.Image = global::Nolte.UI.Resources.save;
            this.mnuExport.Name = "mnuExport";
            this.mnuExport.Size = new System.Drawing.Size(242, 22);
            this.mnuExport.Text = "Speichern / Exportieren";
            this.mnuExport.Click += new System.EventHandler(this.doExport);
            this.menuItems["Datei"].Add(this.mnuExport);

            this.menuItems.Add("Bearbeiten", new System.Collections.Generic.List<System.Windows.Forms.ToolStripMenuItem>());

            this.mnuCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCopy.Image = global::Nolte.UI.Resources.copy;
            this.mnuCopy.Name = "mnuCopy";
            this.mnuCopy.Size = new System.Drawing.Size(148, 22);
            this.mnuCopy.Text = "Kopieren";
            this.mnuCopy.Click += new System.EventHandler(this.doCopy);
            this.menuItems["Bearbeiten"].Add(this.mnuCopy);

            this.mnuPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPaste.Image = global::Nolte.UI.Resources.paste;
            this.mnuPaste.Name = "mnuPaste";
            this.mnuPaste.Size = new System.Drawing.Size(148, 22);
            this.mnuPaste.Text = "Einfügen";
            this.mnuPaste.Click += new System.EventHandler(this.doPaste);
            this.menuItems["Bearbeiten"].Add(this.mnuPaste);

            this.menuItems.Add("Extras", new System.Collections.Generic.List<System.Windows.Forms.ToolStripMenuItem>());

            this.mnuScreenshot = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuScreenshot.Image = global::Nolte.UI.Resources.monitor_image;
            this.mnuScreenshot.Name = "mnuScreenshot";
            this.mnuScreenshot.Size = new System.Drawing.Size(152, 22);
            this.mnuScreenshot.Text = "Bildschirmfoto";
            this.mnuScreenshot.Visible = false;
            this.mnuScreenshot.Click += new System.EventHandler(this.doScreenshot);
            this.menuItems["Extras"].Add(this.mnuScreenshot);

            foreach (var p in this.menuItems)
            {
                System.Windows.Forms.ToolStripMenuItem t = new ToolStripMenuItem(p.Key);
                menuStrip1.Items.Add(t);
                foreach (var pp in p.Value)
                    t.DropDownItems.Add(pp);
            }
        }

        private System.Windows.Forms.ToolStripMenuItem mnuExport;
        private System.Windows.Forms.ToolStripMenuItem mnuImport;
        private System.Windows.Forms.ToolStripMenuItem mnuOpenWith;
        private System.Windows.Forms.ToolStripMenuItem mnuCopy;
        private System.Windows.Forms.ToolStripMenuItem mnuPaste;
        private System.Windows.Forms.ToolStripMenuItem mnuScreenshot;

        private System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<System.Windows.Forms.ToolStripMenuItem>> menuItems = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<System.Windows.Forms.ToolStripMenuItem>>();

        public System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<System.Windows.Forms.ToolStripMenuItem>> MenuItems
        {
            get
            {
                return this.menuItems;
            }
        }

        public bool MenuVisible
        {
            get
            {
                foreach (System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.List<System.Windows.Forms.ToolStripMenuItem>> p in this.menuItems)
                {
                    foreach (System.Windows.Forms.ToolStripMenuItem i in p.Value)
                    {
                        return i.Visible;
                    }
                }
                return false;
            }
            set
            {
                foreach (System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.List<System.Windows.Forms.ToolStripMenuItem>> p in this.menuItems)
                {
                    foreach (System.Windows.Forms.ToolStripMenuItem i in p.Value)
                    {
                        i.Visible = value;
                    }
                }
            }
        }

        public bool InlineMenuVisible
        {
            get
            {
                return menuStrip1.Visible;
            }
            set { menuStrip1.Visible = value; }
        }

        private void ImageEditor_Load(object sender, System.EventArgs e)
        {
            if (!DesignMode)
            {
                this.prepareClipboardViewer();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            Api.ChangeClipboardChain(this.Handle, this.nextClipboardViewer);
            base.Dispose(disposing);
        }

        private System.Drawing.Image CloneImage(System.Drawing.Image i)
        {
            System.Drawing.Bitmap b = new System.Drawing.Bitmap(i.Width, i.Height);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(b);
            g.DrawImageUnscaled(i, 0, 0);
            g.Dispose();
            return b;
        }

        public bool ToolbarVisible
        {
            get
            {
                return this.toolStrip1.Visible;
            }
            set
            {
                this.MenuVisible = value;
                this.toolStrip1.Visible = value;
            }
        }

        public System.Drawing.Image Image
        {
            get
            {
                return this.pictureBox1.Image;
            }
            set
            {
                this.pictureBox1.Image = value;
            }
        }

        private void doOpenWith(object sender, System.EventArgs e)
        {
            string fileName;
            using (System.Drawing.Image i = CloneImage(this.pictureBox1.Image))
            {
                do
                {
                    fileName = System.IO.Path.GetTempPath() + System.Guid.NewGuid().ToString() + "." + Settings.Default.ImageDefaultExtension;
                }
                while (System.IO.File.Exists(fileName));
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    CloneImage(this.pictureBox1.Image).Save(ms, new System.Drawing.Imaging.ImageFormat(Settings.Default.ImageDefaultFormat));
                    System.IO.File.WriteAllBytes(fileName, ms.GetBuffer());
                }
            }
            var args = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.System), "shell32.dll");
            args += ",OpenAs_RunDLL " + fileName;
            System.Diagnostics.Process p = System.Diagnostics.Process.Start("rundll32.exe", args);
            
            while (!p.HasExited)
            {
                System.Windows.Forms.Application.DoEvents();
            }
            if (System.Windows.Forms.MessageBox.Show("Soll das Bild wieder importiert werden?", "", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                if (System.IO.File.Exists(fileName))
                {
                    try
                    {
                        using (System.IO.MemoryStream ms = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(fileName)))
                        {
                            this.pictureBox1.Image = CloneImage(System.Drawing.Image.FromStream(ms));
                        }
                    }
                    catch
                    {
                        System.Windows.Forms.MessageBox.Show("Das Bild konnte nicht gelesen werden!", "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                    }
                    try
                    {
                        System.IO.File.Delete(fileName);
                    }
                    catch
                    { }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Die Bilddatei existiert nicht mehr!", "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                }
            }
        }

        private void doImport(object sender, System.EventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog())
            {
                dlg.Filter = "Bilddateien|*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff;*.bmp";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        using (System.IO.MemoryStream ms = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(dlg.FileName)))
                        {
                            this.pictureBox1.Image = CloneImage(System.Drawing.Image.FromStream(ms));
                        }
                    }
                    catch
                    {
                        System.Windows.Forms.MessageBox.Show("Bild konnte nicht geladen werden", "Fehler", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void doExport(object sender, System.EventArgs e)
        {
            using (System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog())
            {
                dlg.Filter = Settings.Default.ImageDefaultExtension.ToUpper() + "-Datei|*." + Settings.Default.ImageDefaultExtension;
                dlg.OverwritePrompt = true;
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                        {
                            CloneImage(this.pictureBox1.Image).Save(ms, new ImageFormat(Settings.Default.ImageDefaultFormat));
                            System.IO.File.WriteAllBytes(dlg.FileName, ms.GetBuffer());
                        }
                    }
                    catch
                    {
                        System.Windows.Forms.MessageBox.Show("Bild konnte nicht gespeichert werden", "Fehler", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void doCopy(object sender, System.EventArgs e)
        {
            try
            {
                System.Windows.Forms.Clipboard.SetImage(this.pictureBox1.Image);
            }
            catch
            { }
        }

        private void doPaste(object sender, System.EventArgs e)
        {
            if (!System.Windows.Forms.Clipboard.ContainsImage())
            {
                return;
            }
            try
            {
                pictureBox1.Image = this.CloneImage(System.Windows.Forms.Clipboard.GetImage());
            }
            catch
            { }
        }

        private void doScreenshot(object sender, System.EventArgs e)
        {
            System.Drawing.Bitmap b;
            System.Drawing.Graphics g;
            System.Windows.Forms.Screen scr = null;
            if (System.Windows.Forms.Screen.AllScreens.Length > 1)
            {
                using (ChooseScreenDialog csd = new ChooseScreenDialog())
                {
                    if (csd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    {
                        scr = csd.ChoosenScreen;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                scr = System.Windows.Forms.Screen.PrimaryScreen;
            }

            ParentForm.Visible = false;
            System.Windows.Forms.Application.DoEvents();
            System.Threading.Thread.Sleep(200);
            System.Windows.Forms.Application.DoEvents();

            if (scr == null)
            {
                int max_x = 0, max_y = 0, min_x = 0, min_y = 0;
                foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
                {
                    if (screen.Bounds.Top + screen.Bounds.Height > max_y)
                    {
                        max_y = screen.Bounds.Top + screen.Bounds.Height;
                    }
                    if (screen.Bounds.Left + screen.Bounds.Width > max_x)
                    {
                        max_x = screen.Bounds.Left + screen.Bounds.Width;
                    }
                    if (screen.Bounds.Top < min_y)
                    {
                        min_y = screen.Bounds.Top;
                    }
                    if (screen.Bounds.Left < min_x)
                    {
                        min_x = screen.Bounds.Left; ;
                    }
                }
                b = new System.Drawing.Bitmap(max_x + min_x * -1, max_y + min_y * -1);
                g = System.Drawing.Graphics.FromImage(b);
                g.CopyFromScreen(min_x, min_y, 0, 0, b.Size);
            }
            else
            {
                b = new System.Drawing.Bitmap(scr.Bounds.Width, scr.Bounds.Height);
                g = System.Drawing.Graphics.FromImage(b);
                g.CopyFromScreen(scr.Bounds.Left, scr.Bounds.Top, 0, 0, b.Size);
            }

            ParentForm.Visible = true;
            g.Dispose();
            pictureBox1.Image = b;
        }

        #region Clipboard

        System.IntPtr nextClipboardViewer;

        private void prepareClipboardViewer()
        {
            nextClipboardViewer = (System.IntPtr)Api.SetClipboardViewer((int)this.Handle);
            DisplayClipboardData();
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case Api.WM_DRAWCLIPBOARD:
                    DisplayClipboardData();
                    Api.SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                case Api.WM_CHANGECBCHAIN:
                    if (m.WParam == nextClipboardViewer)
                        nextClipboardViewer = m.LParam;
                    else
                        Api.SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void DisplayClipboardData()
        {
            mnuPaste.Enabled = btnPaste.Enabled = System.Windows.Forms.Clipboard.ContainsImage();
        }

        #endregion

    }
}
