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
    public partial class Curve : UserControl
    {
        public Curve()
        {
            InitializeComponent();
            this.strfmt.Alignment = StringAlignment.Far;
            this.strfmt.LineAlignment = StringAlignment.Far;
            this.Font = new Font("Lucida Console", 8f, FontStyle.Bold);
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

        private CurveStyle style = CurveStyle.UpRight;
        public CurveStyle Style
        {
            get
            {
                return this.style;
            }
            set
            {
                this.style = value;
                this.Invalidate();
            }
        }

        private void Curve_Paint(object sender, PaintEventArgs e)
        {
            GraphicsPath gp;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            Rectangle r = e.ClipRectangle;
            float minR = r.Width > r.Height ? r.Height / 2 : r.Width / 2;
            Brush BackPen = new SolidBrush(this.currentColor);
            switch (this.style)
            {
                case CurveStyle.UpRight:
                    gp = new GraphicsPath();
                    gp.StartFigure();
                    try
                    {
                        gp.AddLine(r.Left, r.Bottom, r.Left + r.Width / 2, r.Bottom);
                        gp.AddArc(r.Left + r.Width / 2, r.Top + r.Height / 2, 2 * minR, 2 * minR, 180, 90);
                        gp.AddLine(r.Left + r.Width / 2 + minR, r.Top + r.Height / 2, r.Right, r.Top + r.Height / 2);
                        gp.AddLine(r.Right, r.Top + r.Height / 2, r.Right, r.Top);
                        gp.AddLine(r.Right, r.Top, r.Left + minR, r.Top);
                        gp.AddArc(r.Left, r.Top, 2 * minR, 2 * minR, 270, -90);
                        gp.CloseFigure();
                    }
                    catch { }
                    e.Graphics.FillPath(BackPen, gp);
                    break;
                case CurveStyle.UpLeft:
                    gp = new GraphicsPath();
                    gp.StartFigure();
                    try
                    {
                        gp.AddLine(r.Right, r.Bottom, r.Left + r.Width / 2, r.Bottom);
                        gp.AddArc(r.Left + r.Width / 2 - minR, r.Top + r.Height / 2, 2*minR, 2*minR, 0, -90);
                        gp.AddLine(r.Left + r.Width / 2 - minR, r.Top + r.Height / 2, r.Left, r.Top + r.Height / 2);
                        gp.AddLine(r.Left, r.Top + r.Height / 2, r.Left, r.Top);
                        gp.AddLine(r.Left, r.Top, r.Right - minR, r.Top);
                        gp.AddArc(r.Right - 2*minR, r.Top, 2 * minR, 2* minR, 270, 90);
                        gp.CloseFigure();
                    }
                    catch { }
                    e.Graphics.FillPath(BackPen, gp);
                    break;
                case CurveStyle.DownRight:
                    gp = new GraphicsPath();
                    gp.StartFigure();
                    try
                    {
                        gp.AddLine(r.Right, r.Bottom, r.Left + minR, r.Bottom);
                        gp.AddArc(r.Left, r.Bottom - 2 * minR, 2 * minR, 2 * minR, 90, 90);
                        gp.AddLine(r.Left, r.Bottom - minR,r.Left, r.Top);
                        gp.AddLine(r.Left, r.Top, r.Left + r.Width / 2, r.Top);
                        gp.AddLine(r.Left + r.Width / 2, r.Top, r.Left + r.Width / 2, r.Top + r.Height /2 - minR);
                        gp.AddArc(r.Left + r.Width / 2, r.Top + r.Height /2 - 2 * minR, 2 * minR, 2 * minR, 180, -90);
                        gp.AddLine(r.Left + r.Width / 2 + minR, r.Top + r.Height / 2, r.Right, r.Top + r.Height / 2);
                        gp.CloseFigure();
                    }
                    catch { }
                    e.Graphics.FillPath(BackPen, gp);
                    break;
                case CurveStyle.DownLeft:
                    gp = new GraphicsPath();
                    gp.StartFigure();
                    try
                    {
                        gp.AddLine(r.Left, r.Bottom, r.Right - minR, r.Bottom);
                        gp.AddArc(r.Right - 2* minR, r.Bottom - 2 * minR, 2 * minR, 2 * minR, 90, -90);
                        gp.AddLine(r.Right, r.Bottom - minR, r.Right, r.Top);
                        gp.AddLine(r.Right, r.Top, r.Left + r.Width / 2, r.Top);
                        gp.AddLine(r.Left + r.Width / 2, r.Top, r.Left + r.Width / 2, r.Top + r.Height / 2 - minR);
                        gp.AddArc(r.Left + r.Width / 2 - 2*minR, r.Top + r.Height / 2 - 2 * minR, 2 * minR, 2 * minR, 0, 90);
                        gp.AddLine(r.Left + r.Width / 2 - minR, r.Top + r.Height / 2, r.Left, r.Top + r.Height / 2);
                        gp.CloseFigure();
                    }
                    catch { }
                    e.Graphics.FillPath(BackPen, gp);
                    break;
            }
        }

        private void Curve_MouseLeave(object sender, EventArgs e)
        {
            this.currentColor = this.backgroundColor;
            this.Invalidate();
        }

        private void Curve_MouseEnter(object sender, EventArgs e)
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

    public enum CurveStyle : int
    {
        DownRight,
        DownLeft,
        UpRight,
        UpLeft,
        DownLeftRight,
        UpLeftRight,
        UpDownLeft,
        UpDownRight,
        Cross
    }
}
