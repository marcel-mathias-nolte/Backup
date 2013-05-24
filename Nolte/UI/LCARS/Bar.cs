using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace LCARS
{
    public partial class Bar : UserControl
    {
        public Bar()
        {
            InitializeComponent();
            this.strfmt.Alignment = StringAlignment.Far;
            this.strfmt.LineAlignment = StringAlignment.Far;
            //this.Font = new Font("Lucida Console", 10f, FontStyle.Bold);
        }

        private string label = "";
        public string Label
        {
            get
            {
                return this.label;
            }
            set
            {
                this.label = value;
                this.Invalidate();
            }
        }

        private Color currentColor = Color.Blue;
        private StringFormat strfmt = new StringFormat();

        private Color backgroundColor = Color.Blue;
        public Color BackgroundColor
        {
            get
            {
                return this.backgroundColor;
            }
            set
            {
                this.backgroundColor = value;
                this.currentColor = value;
                this.Invalidate();
            }
        }

        private Color hoverColor = Color.Silver;
        public Color HoverColor
        {
            get
            {
                return this.hoverColor;
            }
            set
            {
                this.hoverColor = value;
            }
        }

        private void Bar_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            Brush TextPen = new SolidBrush(Color.Black);
            Brush BackPen = new SolidBrush(this.currentColor);
            e.Graphics.FillRectangle(BackPen, e.ClipRectangle);
            e.Graphics.DrawString(this.label, this.Font, TextPen, e.ClipRectangle, this.strfmt);
        }

        private void Bar_MouseLeave(object sender, EventArgs e)
        {
            this.currentColor = this.backgroundColor;
            this.Invalidate();
        }

        private void Bar_MouseEnter(object sender, EventArgs e)
        {
            this.currentColor = this.hoverColor;
            this.Invalidate();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020;
                return cp;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // dont code anything here. Leave blank
        }

        protected void InvalidateEx()
        {
            if (Parent == null)
                return;
            Rectangle rc = new Rectangle(this.Location, this.Size);
            Parent.Invalidate(rc, true);
        }
    }
}
