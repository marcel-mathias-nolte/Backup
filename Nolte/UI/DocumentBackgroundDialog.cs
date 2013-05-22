using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Nolte.UI
{
    public partial class DocumentBackgroundDialog : Form
    {
        public DocumentBackgroundDialog()
        {
            InitializeComponent();
            if (!Settings.Default.EditorAllowBackground)
            {
                checkBox2.Checked = false;
                checkBox2.Enabled = false;
                button2.Enabled = false;
                label2.Enabled = false;
                comboBox1.Enabled = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            button1.Enabled = checkBox1.Checked;
            RenderPreview();
        }

        private void RenderPreview()
        {
            textBox1.BackColor = checkBox1.Checked ? backColor : SystemColors.Window;
            if (checkBox2.Checked &&  backImage != null)
            {
                textBox1.BackgroundImage = backImage;
                textBox1.BackgroundImageLayout = backImageLayout;
            }
            else
            {
                textBox1.BackgroundImage = null;
            }
        }

        private Color backColor = default(Color);

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (ColorDialog dlg = new ColorDialog())
                {
                    dlg.Color = backColor;
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        backColor = dlg.Color;
                    }
                }
                RenderPreview();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Fehler");
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            button2.Enabled = checkBox2.Checked;
            label2.Enabled = checkBox2.Checked;
            comboBox1.Enabled = checkBox2.Checked;
            RenderPreview();
        }

        private Image backImage = null;

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Bilddateien|*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff;*.bmp";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (System.IO.MemoryStream ms = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(dlg.FileName)))
                        {
                            backImage = CloneImage(Image.FromStream(ms));
                        }
                        RenderPreview();
                    }
                    catch
                    {
                        MessageBox.Show("Bild konnte nicht geladen werden", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
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

        private ImageLayout backImageLayout = ImageLayout.None;

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    backImageLayout = ImageLayout.None;
                    break;
                case 1:
                    backImageLayout = ImageLayout.Tile;
                    break;
                case 2:
                    backImageLayout = ImageLayout.Center;
                    break;
                case 3:
                    backImageLayout = ImageLayout.Stretch;
                    break;
                case 4:
                    backImageLayout = ImageLayout.Zoom;
                    break;
            }
            RenderPreview();
        }

        public Image DocumentBackgroundImage
        {
            get
            {
                return checkBox2.Checked ? backImage : null;
            }
            set
            {
                backImage = value;
                checkBox2.Checked = true;
                RenderPreview();
            }
        }

        public ImageLayout DocumentBackgroundImageLayout
        {
            get
            {
                switch (comboBox1.SelectedIndex)
                {
                    case 1:
                        return ImageLayout.Tile;
                    case 2:
                        return ImageLayout.Center;
                    case 3:
                        return ImageLayout.Stretch;
                    case 4:
                        return ImageLayout.Zoom;
                    default:
                        return ImageLayout.None;
                }
            }
            set
            {
                backImageLayout = value;
                switch (value)
                {
                    case ImageLayout.None:
                        comboBox1.SelectedIndex = 0;
                        break;
                    case ImageLayout.Tile:
                        comboBox1.SelectedIndex = 1;
                        break;
                    case ImageLayout.Center:
                        comboBox1.SelectedIndex = 2;
                        break;
                    case ImageLayout.Stretch:
                        comboBox1.SelectedIndex = 3;
                        break;
                    case ImageLayout.Zoom:
                        comboBox1.SelectedIndex = 4;
                        break;
                }
                RenderPreview();
            }
        }

        public Color DocumentBackgroundColor
        {
            get
            {
                return checkBox1.Checked ? backColor : Color.Empty;
            }
            set
            {
                checkBox1.Checked = true;
                backColor = value;
                RenderPreview();
            }
        }
    }
}
