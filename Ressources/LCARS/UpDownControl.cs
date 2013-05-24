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
    public delegate void DirectionEventHandler(object sender, EventArgs e);

    public partial class UpDownControl : UserControl
    {

        public event DirectionEventHandler Up, Down;

        protected virtual void OnUp(EventArgs e)
        {
            if (Up != null)
                Up(this, e);
        }

        protected virtual void OnDown(EventArgs e)
        {
            if (Down != null)
                Down(this, e);
        }

        public UpDownControl()
        {
            InitializeComponent();
        }

        private Color currentColor = Color.White;

        private Color backgroundColor = Color.White;
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

        private void UpDownControl_Paint(object sender, PaintEventArgs e)
        {
            GraphicsPath gp;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            Rectangle r = e.ClipRectangle;
            Brush BackPen = new SolidBrush(this.currentColor);
            gp = new GraphicsPath();
            try
            {
                gp.StartFigure();
                gp.AddLine(r.Left + r.Width / 2, r.Top, r.Right, r.Top + (r.Height - 5) / 2);
                gp.AddLine(r.Right, r.Top + (r.Height - 5) / 2, r.Left, r.Top + (r.Height - 5) / 2);
                gp.CloseFigure();
            }
            catch { }
            try
            {
                gp.StartFigure();
                gp.AddLine(r.Left + r.Width / 2, r.Bottom, r.Right, r.Bottom - (r.Height - 5) / 2);
                gp.AddLine(r.Right, r.Bottom - (r.Height - 5) / 2, r.Left, r.Bottom - (r.Height - 5) / 2);
                gp.CloseFigure();
            }
            catch { }
            e.Graphics.FillPath(BackPen, gp);
        }

        private void UpDownControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Location.Y > this.ClientRectangle.Top + this.ClientRectangle.Height / 2)
                OnDown(EventArgs.Empty);
            else
                OnUp(EventArgs.Empty);
        }
    }
}
