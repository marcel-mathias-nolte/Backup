using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LCARS
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            ReadCalendarItems();
        }

        List<Button> CalendarItems = new List<Button>();
        int CurrentCalPosition = 0;

        public int ShowCalenderItems()
        {
            foreach (Button bb in this.CalendarItems)
                bb.Location = new Point(bb.Location.X, this.Height + 1000);
            int i = CurrentCalPosition;
            Button b;
            do
            {
                if (i < CalendarItems.Count)
                {
                    b = CalendarItems[i];
                    b.Location = new System.Drawing.Point(503, 149 + 26 * (i - CurrentCalPosition));
                }
                i++;
            }
            while (149 + 26 * (i - CurrentCalPosition) + 26 < bar11.Location.Y - 26);
            return i;
        }

        public void ReadCalendarItems()
        {
            int i = 1, j;
            Button b;
            iCalToken t;
            TableItem ti;
            string s, sort;

            iCalParser p = new iCalParser();
            p.ParseFile("Geburtsage.ics");
            table1.Columns.Add(new ColumnData("", 17));
            table1.Columns.Add(new ColumnData("", 78));
            table1.Columns.Add(new ColumnData("", 5));

            for (j = 0; j < ((iCalListToken) p.Root["VCALENDAR"]["VEVENT"]).Count; j++)
            {
                t = ((iCalListToken)p.Root["VCALENDAR"]["VEVENT"])[j];
                /*
                b = new Button();
                b.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
                b.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
                b.FillStyle = LCARS.ButtonFillStyle.UnfilledColumns;
                b.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                b.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
                b.Label = "";
                b.Location = new System.Drawing.Point(503, 149 + 26 * i);
                b.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
                b.Name = "button4";
                b.Size = new System.Drawing.Size(414, 20);
                b.TabIndex = 28;
                b.Visible = false;
                b.Click += new System.EventHandler(this.button4_Click);
                 * */
                if (t["DTSTART;VALUE=DATE"] != null)
                {
                    s = ((iCalPropertyToken)t["DTSTART;VALUE=DATE"]).Value;
                    sort = s;
                    s = s.Substring(6, 2) + "." + s.Substring(4, 2) + "." + s.Substring(0, 4);
                }
                else if (t["DTSTART;TZID=Europe/Berlin"] != null)
                {
                    s = ((iCalPropertyToken)t["DTSTART;TZID=Europe/Berlin"]).Value;
                    sort = s;
                    s = s.Substring(6, 2) + "." + s.Substring(4, 2) + "." + s.Substring(0, 4);
                }
                else
                {
                    sort = "";
                    s = "";
                }
                if (t["RRULE"] != null && ((iCalPropertyToken)t["RRULE"]).Value == "FREQ=YEARLY")
                {
                }
                ti = new TableItem();
                ti.Data.Add(s);
                ti.Data.Add(((iCalPropertyToken)t["SUMMARY"]).Value);
                ti.Data.Add("G");
                ti.Sorting = sort;
                table1.Items.Add(ti);
                /*
                b.Columns.Add(new ColumnData(s, 17));
                b.Columns.Add(new ColumnData(((iCalPropertyToken)t["SUMMARY"]).Value, 78));
                b.Columns.Add(new ColumnData("G", 5));
                this.CalendarItems.Add(b);
                this.Controls.Add(b);
                i++;
                 * */
            }
            table1.Items.Sort();

        }

        private void bar1_Click(object sender, EventArgs e)
        {
            calendar1.Visible = !calendar1.Visible;
            threeMonthCalendar1.Visible = !threeMonthCalendar1.Visible;
            label1.Visible = !label1.Visible;
            label2.Visible = !label2.Visible;
            button1.Visible = !button1.Visible;
            button2.Visible = !button2.Visible;
            if (calendar1.Visible)
            {
                threeMonthCalendar1.Invalidate();
                label1.Text = "Datum:   " + DateTime.Now.ToShortDateString();
                bar2.Label = "KALENDERFUNKTION";
                button7.FillStyle = ButtonFillStyle.UnfilledColumns;
                button8.FillStyle = ButtonFillStyle.UnfilledColumns;
            }
            else
            {
                bar2.Label = "";
                table1.Visible = false;
                foreach (Button b in this.CalendarItems)
                    b.Visible = false;
                button6.Visible = false;
                button7.Visible = false;
                button8.Visible = false;
            }
            curve1.Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (calendar1.Visible)
            {
                calendar1.Invalidate();
                if (DateTime.Now.Hour == 0 && DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
                {
                    threeMonthCalendar1.Invalidate();
                    label1.Text = "Datum:   " + DateTime.Now.ToShortDateString();
                }
                label2.Text = "Uhrzeit: " + DateTime.Now.ToLongTimeString();
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            timer1.Start();
            foreach (Control c in this.Controls)
                c.Invalidate();
            this.WindowState = FormWindowState.Maximized;
        }

        private void bar5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            table1.Visible = !table1.Visible;
          //  ShowCalenderItems();
            table1.ShowTable();
            foreach (Button b in this.CalendarItems)
                b.Visible = !b.Visible;
            button6.Visible = !button6.Visible;
            button7.Visible = !button7.Visible;
            button8.Visible = !button8.Visible;
            button2.Visible = !button2.Visible;
        }

        private Button SelectedTermin = null;

        private void button4_Click(object sender, EventArgs e)
        {
            if (SelectedTermin != null)
            {
                SelectedTermin.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
                SelectedTermin.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            }
            SelectedTermin = (Button)sender;
            SelectedTermin.BackgroundColor = Color.Red;
            SelectedTermin.HoverColor = Color.Red;
            button7.FillStyle = ButtonFillStyle.Filled;
            button8.FillStyle = ButtonFillStyle.Filled;
        }

        private void bar4_Click(object sender, EventArgs e)
        {
            this.BackColor = this.BackColor == Color.Black ? Color.White : Color.Black;
        }

        private void upDownControl1_Up(object sender, EventArgs e)
        {
            table1.ScrollUp();
        }

        private void upDownControl1_Down(object sender, EventArgs e)
        {
            table1.ScrollDown();
        }
    }
}
