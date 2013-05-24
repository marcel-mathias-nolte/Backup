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
    public partial class Calendar : UserControl
    {
        protected Color specialDayColor = Color.Red;
        protected Color hilightColor = Color.Orange;
        protected bool sundayIsFirstDayOfWeek = false;

        public Calendar()
        {
            InitializeComponent();
            this.Font = new Font("Lucida Console", 8f, FontStyle.Bold);
        }

        private void ThreeMonthCalendar_Paint(object sender, PaintEventArgs e)
        {
            string[] WeekDays = new string[] { "So", "Mo", "Di", "Mi", "Do", "Fr", "Sa", "So" };
            int i;
            Brush b = new SolidBrush(this.ForeColor);
            Brush bh = new SolidBrush(this.specialDayColor);
            for (i = 0; i < 7; i++)
            {
                e.Graphics.DrawString(WeekDays[i + (sundayIsFirstDayOfWeek ? 0 : 1)], this.Font, b, i * 20, 0);
            }
            DateTime d = DateTime.Today.AddDays(-DateTime.Today.Day);
            int row = 1, dow = 0;
            while (d.Day < DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
            {
                d = d.AddDays(1);
                if ((d.DayOfWeek == DayOfWeek.Monday && !sundayIsFirstDayOfWeek) || (d.DayOfWeek == DayOfWeek.Sunday && sundayIsFirstDayOfWeek))
                    row++;
                dow = (int)d.DayOfWeek;
                if (dow == 0)
                    dow = 7;
                dow--;
                e.Graphics.DrawString(d.Day.ToString().PadLeft(2, '0'), this.Font, (d == DateTime.Today) ? bh : b, dow * 20, row * 15);
            }
        }

        private void 
    }
}
