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
    public partial class Button : UserControl
    {
        public Button()
        {
            InitializeComponent();
            this.strfmt.Alignment = StringAlignment.Far;
            this.strfmt.LineAlignment = StringAlignment.Far;
            this.Font = new Font("Lucida Console", 8f, FontStyle.Bold);
        }

        List<ColumnData> columns = new List<ColumnData>();
        public List<ColumnData> Columns
        {
            get
            {
                return columns;
            }
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

        private ButtonFillStyle fillStyle = ButtonFillStyle.Filled;
        public ButtonFillStyle FillStyle
        {
            get
            {
                return this.fillStyle;
            }
            set
            {
                this.fillStyle = value;
                this.Invalidate();
            }
        }

        private void Button_Paint(object sender, PaintEventArgs e)
        {
            int totalPixels;
            int totalPercentage;
            List<Rectangle> cells;
            GraphicsPath gp;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            Rectangle r = e.ClipRectangle;
            float minR = r.Width > r.Height ? r.Height / 2 : r.Width / 2;
            Brush BackPen = new SolidBrush(this.currentColor);
            Brush TextPen = new SolidBrush(Color.Black);
            gp = new GraphicsPath();
            try
            {
                gp.StartFigure();
                gp.AddArc(r.Left, r.Top, 2 * minR , 2 * minR, 270, -180);
                gp.AddLine(r.Left + minR, r.Bottom, r.Left + minR, r.Top);
                gp.CloseFigure();
            }
            catch { }
            try
            {
                gp.StartFigure();
                gp.AddArc(r.Right - 2 * minR - 1, r.Top, 2 * minR, 2 * minR, 270, 180);
                gp.AddLine(r.Right - minR - 1, r.Bottom, r.Right - minR - 1, r.Top);
                gp.CloseFigure();
            }
            catch { }
            if (this.fillStyle == ButtonFillStyle.Filled)
            {
                this.strfmt.Alignment = StringAlignment.Far;
                this.strfmt.LineAlignment = StringAlignment.Far;
                try
                {
                    gp.StartFigure();
                    gp.AddLine(r.Left + minR, r.Bottom, r.Left + minR, r.Top);
                    gp.AddLine(r.Left + minR, r.Top, r.Right - minR - 1, r.Top);
                    gp.AddLine(r.Right - minR - 1, r.Bottom, r.Right - minR - 1, r.Top);
                    gp.AddLine(r.Right - minR - 1, r.Bottom, r.Left + minR, r.Bottom);
                    gp.CloseFigure();
                    e.Graphics.FillPath(BackPen, gp);
                    gp = new GraphicsPath();
                    e.Graphics.DrawString(this.label, this.Font, TextPen, new Rectangle((int)r.Left + (int)minR, (int)r.Top, (int)r.Width - 1 - 2 * (int)minR, (int)r.Height - 1), this.strfmt);
                }
                catch { }
                e.Graphics.FillPath(BackPen, gp);
            }
            else if (this.fillStyle == ButtonFillStyle.FilledColumns)
            {
                strfmt.Alignment = StringAlignment.Near;
                try
                {
                    totalPixels = (int)r.Width - 7 - 2 * (int)minR;
                    totalPercentage = 0;
                    foreach (ColumnData d in this.columns)
                        totalPercentage += d.WidthPercent;
                    totalPixels -= (this.columns.Count - 1) * 3;
                    cells = new List<Rectangle>();
                    foreach (ColumnData d in this.columns)
                    {
                        cells.Add(new Rectangle(cells.Count == 0 ? (int)minR + 3 : cells[cells.Count - 1].Right + 3, 0, d.WidthPercent * totalPixels / totalPercentage, r.Height));
                    }
                    gp.AddRectangles(cells.ToArray());
                    e.Graphics.FillPath(BackPen, gp);
                    gp = new GraphicsPath();
                    for(int i = 0; i< this.columns.Count; i++)
                        e.Graphics.DrawString(this.columns[i].Label, this.Font, TextPen, cells[i], this.strfmt);
                }
                catch { }
                e.Graphics.FillPath(BackPen, gp);
            }
            else if (this.fillStyle == ButtonFillStyle.UnfilledColumns)
            {
                strfmt.Alignment = StringAlignment.Near;
                strfmt.LineAlignment = StringAlignment.Center;
                try
                {
                    totalPixels = (int)r.Width - 7 - 2 * (int)minR;
                    totalPercentage = 0;
                    foreach (ColumnData d in this.columns)
                        totalPercentage += d.WidthPercent;
                    totalPixels -= (this.columns.Count - 1) * 3;
                    cells = new List<Rectangle>();
                    foreach (ColumnData d in this.columns)
                    {
                        cells.Add(new Rectangle(cells.Count == 0 ? (int)minR + 3 : cells[cells.Count - 1].Right + 3, 0, d.WidthPercent * totalPixels / totalPercentage, r.Height));
                    }
                    // gp.AddRectangles(cells.ToArray());
                    e.Graphics.FillPath(BackPen, gp);
                    gp = new GraphicsPath();
                    for (int i = 0; i < this.columns.Count; i++)
                        e.Graphics.DrawString(this.columns[i].Label, this.Font, BackPen, cells[i], this.strfmt);
                }
                catch { }
                e.Graphics.FillPath(BackPen, gp);
            }
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
    }
    public class ColumnData
    {
        public string Label;
        public int WidthPercent;
        public ColumnData(string Label, int WidthPercentage)
        {
            this.Label = Label;
            this.WidthPercent = WidthPercentage;
        }
    }
    public enum ButtonFillStyle
    {
        UnFilled,
        Filled,
        FilledColumns,
        UnfilledColumns
    }
}
