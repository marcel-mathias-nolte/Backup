using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LCARS
{
    public partial class Clock : UserControl
    {
        protected Color faceColor = Color.Orange;
        protected Pen facePen = new Pen(Color.Orange);
        protected Color handColor = Color.Red;
        protected Pen handPen = new Pen(Color.Red);

        protected bool drawCircle = true;
        protected bool drawMinutes = true;
        protected bool drawFiveMinutes = true;
        protected bool drawHourHand = true;
        protected bool drawMinuteHand = true;
        protected bool drawSecondHand = true;

        protected float hourHandWidth = 5;
        protected float minuteHandWidth = 3;
        protected float secondHandWidth = 1;

        protected Timer timer;

        protected DateTime time = DateTime.Now;

        public Clock()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
        }

        private void Calendar_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                e.Graphics.FillRectangle(new SolidBrush(this.BackColor), e.ClipRectangle);
                int i;
                float x = (e.ClipRectangle.Width - 1) / 2;
                float y = (e.ClipRectangle.Height - 1) / 2;
                float r = default(float);
                Pen p = facePen;
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                if (drawCircle)
                    e.Graphics.DrawEllipse(facePen, new Rectangle(e.ClipRectangle.Location, new Size(e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1)));
                if (drawMinutes)
                {
                    for (i = 0; i < 360; i += 6)
                    {
                        r = (drawFiveMinutes && i % 30 == 0) ? x / 6 : x / 10;
                        e.Graphics.DrawLine(facePen, x + x * (float)Math.Cos(i * Math.PI / 180), y + y * (float)Math.Sin(i * Math.PI / 180), x + (x - r) * (float)Math.Cos(i * Math.PI / 180), y + (y - r) * (float)Math.Sin(i * Math.PI / 180));
                    }
                }
                else if (drawFiveMinutes)
                {
                    r = x / 6;
                    for (i = 0; i < 360; i += 30)
                    {
                        e.Graphics.DrawLine(facePen, x + x * (float)Math.Cos(i * Math.PI / 180), y + y * (float)Math.Sin(i * Math.PI / 180), x + (x - r) * (float)Math.Cos(i * Math.PI / 180), y + (y - r) * (float)Math.Sin(i * Math.PI / 180));
                    }
                }
                if (drawHourHand)
                {
                    //Stundenzeiger
                    i = time.Hour * 360 / 12 - 90 + time.Minute * 30 / 60;
                    handPen.Width = hourHandWidth;
                    e.Graphics.DrawLine(handPen, x + x / 2 * (float)Math.Cos(i * Math.PI / 180), y + y / 2 * (float)Math.Sin(i * Math.PI / 180), x, y);
                }
                if (drawMinuteHand)
                {
                    //Stundenzeiger
                    i = time.Minute * 360 / 60 - 90;
                    handPen.Width = minuteHandWidth;
                    e.Graphics.DrawLine(handPen, x + x * 3 / 4 * (float)Math.Cos(i * Math.PI / 180), y + y * 3 / 4 * (float)Math.Sin(i * Math.PI / 180), x, y);
                }
                if (drawSecondHand)
                {
                    //Sekundenzeiger
                    i = time.Second * 360 / 60 - 90;
                    handPen.Width = secondHandWidth;
                    e.Graphics.DrawLine(handPen, x + x * (float)Math.Cos(i * Math.PI / 180), y + y * (float)Math.Sin(i * Math.PI / 180), x, y);
                }
            }
            catch
            {
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

        public Color FaceColor { 
            get { return faceColor; }
            set 
            {
                faceColor = value;
                facePen = new Pen(faceColor);
                this.Invalidate();
            }
        }

        public Color HandColor
        {
            get { return handColor; }
            set
            {
                handColor = value;
                handPen = new Pen(handColor);
                this.Invalidate();
            }
        }

        public float FaceLineWidth
        {
            get { return facePen.Width; }
            set
            {
                facePen.Width = value;
                this.Invalidate();
            }
        }

        public float HourHandWidth
        {
            get { return hourHandWidth; }
            set
            {
                hourHandWidth = value;
                this.Invalidate();
            }
        }

        public float MinuteHandWidth
        {
            get { return minuteHandWidth; }
            set
            {
                minuteHandWidth = value;
                this.Invalidate();
            }
        }

        public float SecondHandWidth
        {
            get { return secondHandWidth; }
            set
            {
                secondHandWidth = value;
                this.Invalidate();
            }
        }

        public bool DrawCircle
        {
            get { return drawCircle; }
            set
            {
                drawCircle = value;
                this.Invalidate();
            }
        }

        public bool DrawMinutes
        {
            get { return drawMinutes; }
            set
            {
                drawMinutes = value;
                this.Invalidate();
            }
        }

        public bool DrawFiveMinutes
        {
            get { return drawFiveMinutes; }
            set
            {
                drawFiveMinutes = value;
                this.Invalidate();
            }
        }

        public bool DrawHourHand
        {
            get { return drawHourHand; }
            set
            {
                drawHourHand = value;
                this.Invalidate();
            }
        }

        public bool DrawMinuteHand
        {
            get { return drawMinuteHand; }
            set
            {
                drawMinuteHand = value;
                this.Invalidate();
            }
        }

        public bool DrawSecondHand
        {
            get { return drawSecondHand; }
            set
            {
                drawSecondHand = value;
                this.Invalidate();
            }
        }

        public bool AutoTimer
        {
            get { return timer != null && timer.Enabled; }
            set
            {
                if (value && timer == null)
                {
                    timer = new Timer();
                    timer.Interval = 1000;
                    timer.Tick += (o, e) => { Refresh(DateTime.Now); };
                    timer.Start();
                }
                else if (!value && timer != null)
                {
                    timer.Stop();
                }
                this.Invalidate();
            }
        }

        public void Refresh(DateTime d)
        {
            time = d;
            Invalidate();
        }

        public void Refresh()
        {
            Invalidate();
        }
    }
}
