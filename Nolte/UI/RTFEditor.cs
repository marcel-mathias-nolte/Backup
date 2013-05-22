using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace Nolte.UI
{
    public partial class RTFEditor : UserControl, Nolte.UI.IMainMenuCapable
    {
        public RTFEditor()
        {
            InitializeComponent();
            this.marginRight = 10;
            rtfTextEditor.RightMargin = rtfTextEditor.ClientSize.Width - this.marginRight;
            rtfTextEditor.Resize += new EventHandler(AdjustRightMargin);
            if (!DesignMode)
            {
                this.SizeChanged += new EventHandler(ResizeEditor);

                panel1.PaddingChanged += new EventHandler(ResizeEditor);
                this.Load += new EventHandler(ResizeEditor);
                this.Load += new EventHandler(RTFEditor_Load);
                rtfTextEditor.SelectionChanged += new EventHandler(rtfTextEditor_SelectionChanged);
                rtfTextEditor.LinkClicked += new LinkClickedEventHandler(rtfTextEditor_LinkClicked);
                if (Settings.Default.EditorAllowBackground)
                {
                    Api.SetWindowLong(rtfTextEditor.Handle, Api.GWL_EXSTYLE, Api.WS_EX_TRANSPARENT);
                }
            }


        }

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

        private void rtfTextEditor_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void rtfTextEditor_SelectionChanged(object sender, EventArgs e)
        {
            if (rtfTextEditor.SelectionFont != null)
            {
                rtfBtnBold.Checked = rtfTextEditor.SelectionFont.Bold;
                rtfBtnItalic.Checked = rtfTextEditor.SelectionFont.Italic;
                rtfBtnUnderline.Checked = rtfTextEditor.SelectionFont.Underline;
                rtfBtnStrikeout.Checked = rtfTextEditor.SelectionFont.Strikeout;
                switch (rtfTextEditor.SelectionAlignment)
                {
                    case HorizontalAlignment.Left:
                        rtfBtnLeft.Checked = true;
                        rtfBtnCenter.Checked = false;
                        rtfBtnRight.Checked = false;
                        break;
                    case HorizontalAlignment.Center:
                        rtfBtnLeft.Checked = false;
                        rtfBtnCenter.Checked = true;
                        rtfBtnRight.Checked = false;
                        break;
                    case HorizontalAlignment.Right:
                        rtfBtnLeft.Checked = false;
                        rtfBtnCenter.Checked = false;
                        rtfBtnRight.Checked = true;
                        break;
                }
                rtfBtnList.Checked = rtfTextEditor.SelectionBullet;
                rtfCmbFontFamily.SelectedItem = rtfTextEditor.SelectionFont.FontFamily.Name;
                rtfCmbFontSize.SelectedItem = rtfTextEditor.SelectionFont.Size.ToString();
                rtfBtnWordWrap.Checked = rtfTextEditor.WordWrap;
            }
            rtfBtnUndo.Enabled = rtfTextEditor.CanUndo;
            rtfBtnRedo.Enabled = rtfTextEditor.CanRedo;
        }

        private void RTFEditor_Load(object sender, EventArgs e)
        {
            foreach (FontFamily family in FontFamily.Families)
            {
                rtfCmbFontFamily.Items.Add(family.Name);
            }
            rtfCmbFontFamily.SelectedItem = "Microsoft Sans Serif";
            rtfCmbFontSize.SelectedItem = "9";
        }

        private void ResizeEditor(object sender, EventArgs e)
        {
            rtfTextEditor.Location = new Point(panel1.Padding.Left, panel1.Padding.Top + (rtfToolBar.Visible ? rtfToolBar.Height : 0));
            rtfTextEditor.Height = panel1.ClientSize.Height - (rtfToolBar.Visible ? rtfToolBar.Height : 0) - 20;
            rtfTextEditor.Width = panel1.ClientSize.Width-20;
        }

        private void AdjustRightMargin(object sender, EventArgs e)
        {
            rtfTextEditor.RightMargin = rtfTextEditor.ClientSize.Width > this.marginRight ? rtfTextEditor.ClientSize.Width - this.marginRight : 0;
        }

        #region public properties

        public int ZoomLevel
        {
            get
            {
                return (int)(rtfTextEditor.ZoomFactor * 100);
            }
            set
            {
                rtfTextEditor.ZoomFactor = (float)value / 100.0f;
            }
        }

        private MenuStrip mainMenuStrip = null;

        public MenuStrip MainMenuStrip
        {
            get
            {
                return mainMenuStrip;
            }
            set
            {
                mainMenuStrip = value;
            }
        }

        public new BorderStyle BorderStyle
        {
            get
            {
                return panel1.BorderStyle;
            }
            set
            {
                panel1.BorderStyle = value;
            }
        }

        protected int marginRight;

        public new Padding Padding
        {
            get
            {
                return new Padding(panel1.Padding.Left, panel1.Padding.Top, this.marginRight, panel1.Padding.Bottom);
            }
            set
            {
                this.marginRight = value.Right;
                panel1.Padding = new Padding(value.Left, value.Top, 0, value.Right);
                rtfTextEditor.RightMargin = rtfTextEditor.ClientSize.Width - value.Right;
            }
        }

        public RichTextBox RichTextBox
        {
            get
            {
                return this.rtfTextEditor;
            }
        }

        public bool ReadOnly
        {
            get
            {
                return rtfTextEditor.ReadOnly;
            }
            set
            {
                rtfTextEditor.ReadOnly = value;
                rtfToolBar.Visible = !value;
                ResizeEditor(this, EventArgs.Empty);
            }
        }

        public string Rtf
        {
            get
            {
                return rtfTextEditor.Rtf;
            }
            set
            {
                rtfTextEditor.Rtf = value;
            }
        }

        public override string Text
        {
            get
            {
                return rtfTextEditor.Text;
            }
            set
            {
                rtfTextEditor.Text = value;
            }
        }

        #endregion

        #region Toolbar

        private void rtfBtnBold_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(rtfTextEditor.SelectionFont == null))
                {
                    Font currentFont = rtfTextEditor.SelectionFont;
                    FontStyle newFontStyle = rtfTextEditor.SelectionFont.Style;
                    string txt = (sender as ToolStripButton).Name;
                    newFontStyle = rtfTextEditor.SelectionFont.Style ^ FontStyle.Bold;
                    rtfTextEditor.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                    rtfBtnBold.Checked = rtfTextEditor.SelectionFont.Bold;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Fehler");
            }
        }

        private void rtfBtnItalic_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(rtfTextEditor.SelectionFont == null))
                {
                    Font currentFont = rtfTextEditor.SelectionFont;
                    FontStyle newFontStyle = rtfTextEditor.SelectionFont.Style;
                    string txt = (sender as ToolStripButton).Name;
                    newFontStyle = rtfTextEditor.SelectionFont.Style ^ FontStyle.Italic;
                    rtfTextEditor.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                    rtfBtnItalic.Checked = rtfTextEditor.SelectionFont.Italic;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Fehler");
            }
        }

        private void rtfBtnUnderline_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(rtfTextEditor.SelectionFont == null))
                {
                    Font currentFont = rtfTextEditor.SelectionFont;
                    FontStyle newFontStyle = rtfTextEditor.SelectionFont.Style;
                    string txt = (sender as ToolStripButton).Name;
                    newFontStyle = rtfTextEditor.SelectionFont.Style ^ FontStyle.Underline;
                    rtfTextEditor.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                    rtfBtnUnderline.Checked = rtfTextEditor.SelectionFont.Underline;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Fehler");
            }
        }

        private void rtfBtnLeft_Click(object sender, EventArgs e)
        {
            try
            {
                rtfTextEditor.SelectionAlignment = HorizontalAlignment.Left;
                rtfBtnLeft.Checked = true;
                rtfBtnCenter.Checked = false;
                rtfBtnRight.Checked = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Fehler");
            }
        }

        private void rtfBtnCenter_Click(object sender, EventArgs e)
        {
            try
            {
                rtfTextEditor.SelectionAlignment = HorizontalAlignment.Center;
                rtfBtnLeft.Checked = false;
                rtfBtnCenter.Checked = true;
                rtfBtnRight.Checked = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Fehler");
            }
        }

        private void rtfBtnRight_Click(object sender, EventArgs e)
        {
            try
            {
                rtfTextEditor.SelectionAlignment = HorizontalAlignment.Right;
                rtfBtnLeft.Checked = false;
                rtfBtnCenter.Checked = false;
                rtfBtnRight.Checked = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Fehler");
            }
        }

        private void rtfBtnChooseColor_Click(object sender, EventArgs e)
        {
            try
            {
                using (ColorDialog dlg = new ColorDialog())
                {
                    dlg.Color = rtfTextEditor.SelectionColor;
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        rtfTextEditor.SelectionColor = dlg.Color;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Fehler");
            }
        }

        private void rtfBtnList_Click(object sender, EventArgs e)
        {
            try
            {
                rtfBtnList.Checked = !rtfBtnList.Checked;
                rtfTextEditor.SelectionBullet = rtfBtnList.Checked;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Fehler");
            }
        }

        private void rtfBtnOutdent_Click(object sender, EventArgs e)
        {
            try
            {
                rtfTextEditor.SelectionIndent -= Settings.Default.RTFDefaultIndent;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Fehler");
            }
        }

        private void rtfBtnIndent_Click(object sender, EventArgs e)
        {
            try
            {
                rtfTextEditor.SelectionIndent += Settings.Default.RTFDefaultIndent;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Fehler");
            }
        }

        private void rtfCmbFontFamily_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!(rtfTextEditor.SelectionFont == null))
                {
                    Font currentFont = rtfTextEditor.SelectionFont;
                    FontFamily newFamily = new FontFamily(rtfCmbFontFamily.SelectedItem.ToString());
                    rtfTextEditor.SelectionFont = new Font(newFamily, currentFont.Size, currentFont.Style);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Fehler");
            }
        }

        private void rtfCmbFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!(rtfTextEditor.SelectionFont == null))
                {
                    Font currentFont = rtfTextEditor.SelectionFont;
                    float newSize = Convert.ToSingle(rtfCmbFontSize.SelectedItem.ToString());
                    rtfTextEditor.SelectionFont = new Font(currentFont.FontFamily, newSize, currentFont.Style);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Fehler");
            }
        }

        private void rtfCmbFontSize_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!(rtfTextEditor.SelectionFont == null))
                {
                    Font currentFont = rtfTextEditor.SelectionFont;
                    float newSize = Convert.ToSingle(rtfCmbFontSize.Text);
                    rtfTextEditor.SelectionFont = new Font(currentFont.FontFamily, newSize, currentFont.Style);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Fehler");
            }
        }

        private void rtfBtnFontDialog_Click(object sender, EventArgs e)
        {
            using (FontDialog dlg = new FontDialog())
            {
                if (rtfTextEditor.SelectionFont != null)
                {
                    dlg.Font = rtfTextEditor.SelectionFont;
                }
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    rtfTextEditor.SelectionFont = dlg.Font;
                }
            }
        }

        private void rtfBtnStrikeout_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(rtfTextEditor.SelectionFont == null))
                {
                    Font currentFont = rtfTextEditor.SelectionFont;
                    FontStyle newFontStyle = rtfTextEditor.SelectionFont.Style;
                    newFontStyle = rtfTextEditor.SelectionFont.Style ^ FontStyle.Strikeout;
                    rtfTextEditor.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                    rtfBtnUnderline.Checked = rtfTextEditor.SelectionFont.Underline;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Fehler");
            }
        }

        private void rtfBtnWordWrap_Click(object sender, EventArgs e)
        {
            rtfBtnWordWrap.Checked = !rtfBtnWordWrap.Checked;
            rtfTextEditor.WordWrap = rtfBtnWordWrap.Checked;
        }

        private void rtfBtnInsertImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Bild einfügen";
                dlg.DefaultExt = "jpg";
                dlg.Filter = "Grafikdateien|*.bmp;*.jpg;*.jpeg;*.gif|Alle Dateien|*.*";
                dlg.FilterIndex = 0;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string strImagePath = dlg.FileName;
                        Image img = Image.FromFile(strImagePath);
                        Clipboard.SetDataObject(img);
                        DataFormats.Format df;
                        df = DataFormats.GetFormat(DataFormats.Bitmap);
                        if (this.rtfTextEditor.CanPaste(df))
                        {
                            this.rtfTextEditor.Paste(df);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Kann Bild nicht einfügen.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void rtfBtnUndo_Click(object sender, EventArgs e)
        {
            if (rtfTextEditor.CanUndo)
            {
                rtfTextEditor.Undo();
            }
        }

        private void rtfBtnRedo_Click(object sender, EventArgs e)
        {
            if (rtfTextEditor.CanRedo)
            {
                rtfTextEditor.Redo();
            }
        }

        private void rtfBtnCopy_Click(object sender, EventArgs e)
        {
            rtfTextEditor.Copy();
        }

        private void rtfBtnCut_Click(object sender, EventArgs e)
        {
            rtfTextEditor.Cut();
        }

        private void rtfBtnPaste_Click(object sender, EventArgs e)
        {
            rtfTextEditor.Paste();
        }

        private void rtfBtnHangingIndent_Click(object sender, EventArgs e)
        {
            try
            {
                rtfTextEditor.SelectionHangingIndent += Settings.Default.RTFDefaultIndent;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Fehler");
            }
        }

        private void rtfBtnHangingOutdent_Click(object sender, EventArgs e)
        {
            try
            {
                rtfTextEditor.SelectionHangingIndent -= Settings.Default.RTFDefaultIndent;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Fehler");
            }
        }

        private void rtfBtnClearDocument_Click(object sender, EventArgs e)
        {
            rtfTextEditor.Clear();
        }

        private void rtfBtnSaveAs_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Rich text format|*.rtf";
                dlg.FilterIndex = 0;
                dlg.OverwritePrompt = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        rtfTextEditor.SaveFile(dlg.FileName, RichTextBoxStreamType.RichText);
                    }
                    catch (System.IO.IOException exc)
                    {
                        MessageBox.Show("Fehler beim speichern der Datei: \n" + exc.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (ArgumentException exc_a)
                    {
                        MessageBox.Show("Fehler beim speichern der Datei: \n" + exc_a.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "RTF-Dokument|*.rtf|Alle Dateien|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (dlg.FilterIndex == 0)
                    {
                        try
                        {
                            rtfTextEditor.LoadFile(dlg.FileName);
                        }
                        catch
                        {
                            rtfTextEditor.LoadFile(dlg.FileName, RichTextBoxStreamType.PlainText);
                        }
                    }
                    else
                    {
                        rtfTextEditor.LoadFile(dlg.FileName, RichTextBoxStreamType.PlainText);
                    }
                }
            }
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            FindForm findForm = new FindForm();
            findForm.RtbInstance = rtfTextEditor;
            findForm.InitialText = "";
            findForm.Show();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            ReplaceForm replaceForm = new ReplaceForm();
            replaceForm.RtbInstance = rtfTextEditor;
            replaceForm.InitialText = "";
            replaceForm.Show();
        }

        #endregion

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                using (ColorDialog dlg = new ColorDialog())
                {
                    dlg.Color = rtfTextEditor.SelectionBackColor;
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        rtfTextEditor.SelectionBackColor = dlg.Color;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Fehler");
            }
        }

        public delegate void ZoomChangedEventHandler(int NewZoom);

        public event ZoomChangedEventHandler ZoomChanged;

        protected void OnZoomChanged()
        {
            if (ZoomChanged != null)
            {
                ZoomChanged((int)(rtfTextEditor.ZoomFactor * 100f));
            }
        }

        public string Serialized
        {
            get
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    BinaryFormatter bFormatter = new BinaryFormatter();
                    bFormatter.Serialize(ms, new RTFData(rtfTextEditor));
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
            set
            {
                if (value.Equals(String.Empty))
                {
                    RTFData.Default.ApplyTo(rtfTextEditor);
                }
                else
                {
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream(Convert.FromBase64String(value)))
                    {
                        BinaryFormatter bFormatter = new BinaryFormatter();
                        ((RTFData)bFormatter.Deserialize(ms)).ApplyTo(rtfTextEditor);
                    }
                }
                OnZoomChanged();
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            using (DocumentBackgroundDialog dbd = new DocumentBackgroundDialog())
            {
                if (panel1.BackgroundImage != null)
                {
                    dbd.DocumentBackgroundImage = panel1.BackgroundImage;
                }
                dbd.DocumentBackgroundImageLayout = panel1.BackgroundImageLayout;
                if (panel1.BackColor != SystemColors.Window)
                {
                    dbd.DocumentBackgroundColor = panel1.BackColor;
                }
                if (dbd.ShowDialog(this) == DialogResult.OK)
                {
                    if (Settings.Default.EditorAllowBackground)
                    {
                        panel1.BackgroundImage = dbd.DocumentBackgroundImage == null ? null : CloneImage(dbd.DocumentBackgroundImage);
                        panel1.BackgroundImageLayout = dbd.DocumentBackgroundImageLayout;
                    }
                    else 
                    {
                        rtfTextEditor.BackColor = dbd.DocumentBackgroundColor == Color.Empty ? SystemColors.Window : dbd.DocumentBackgroundColor;
                    }
                    panel1.BackColor = dbd.DocumentBackgroundColor == Color.Empty ? SystemColors.Window : dbd.DocumentBackgroundColor;
                }
            }
        }

        private Image CloneImage(Image i)
        {
            Bitmap b = new Bitmap(i.Width, i.Height);
            Graphics g = Graphics.FromImage(b);
            g.DrawImageUnscaled(i, 0, 0);
            g.Dispose();
            return b;
        }

        private void rtfTextEditor_TextChanged(object sender, EventArgs e)
        {
            panel1.Invalidate();
            rtfTextEditor.Invalidate();
        }
    }
    [Serializable()]
    public class RTFData : ISerializable
    {
        public string RTF;
        public float Zoom;
        public Color BackColor;
        public Image Wallpaper;
        public ImageLayout WallpaperLayout;

        public static RTFData Default
        {
            get
            {
                return new RTFData();
            }
        }

        private RTFData()
        {
            this.RTF = "";
            this.Zoom = 1;
            if (Settings.Default.EditorAllowBackground)
            {
                this.WallpaperLayout = ImageLayout.Zoom;
            }
            this.BackColor = SystemColors.Window;
        }
        
        public RTFData(RichTextBox e)
        {
            this.RTF = e.Rtf;
            this.Zoom = e.ZoomFactor;
            if (Settings.Default.EditorAllowBackground)
            {
                this.Wallpaper = e.Parent.BackgroundImage;
                this.WallpaperLayout = e.Parent.BackgroundImageLayout;
            }
            this.BackColor = e.Parent.BackColor;
        }

        public RTFData(SerializationInfo info, StreamingContext ctxt)
        {
            this.RTF = (string)info.GetValue("RTF", typeof(string));
            this.Zoom = (float)info.GetValue("Zoom", typeof(float));
            this.BackColor = (Color)info.GetValue("BackColor", typeof(Color));
            this.Wallpaper = (Image)info.GetValue("Wallpaper", typeof(Image));
            this.WallpaperLayout = (ImageLayout)info.GetValue("WallpaperLayout", typeof(ImageLayout));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("RTF", this.RTF);
            info.AddValue("Zoom", this.Zoom);
            info.AddValue("BackColor", this.BackColor);
            info.AddValue("Wallpaper", this.Wallpaper);
            info.AddValue("WallpaperLayout", this.WallpaperLayout);
        }

        public void ApplyTo(RichTextBox e)
        {
            e.Rtf = this.RTF;
            e.ZoomFactor = this.Zoom;
            if (Settings.Default.EditorAllowBackground)
            {
                e.Parent.BackgroundImage = this.Wallpaper;
                e.Parent.BackgroundImageLayout = this.WallpaperLayout;
            }
            else
            {
                e.BackColor = this.BackColor;
            }
            e.Parent.BackColor = this.BackColor;
        }
    }
}
