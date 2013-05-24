using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

namespace LCARS
{
    public partial class Table : UserControl
    {
        public Table()
        {
            InitializeComponent();
            Button b = new Button();
            b.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            b.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            b.FillStyle = LCARS.ButtonFillStyle.FilledColumns;
            b.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            b.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            b.Location = new System.Drawing.Point(0, 0);
            b.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            b.Name = "button3";
            b.Size = new System.Drawing.Size(this.Width, 20);
            b.TabIndex = 27;
            this.Controls.Add(b);
            b.Columns.Add(new ColumnData("Termin", 17));
            b.Columns.Add(new ColumnData("Beschreibung", 78));
            b.Columns.Add(new ColumnData("", 5));
            this.Headline = b;
        }

        private Button Headline;

        private List<ColumnData> columns = new List<ColumnData>();
        public List<ColumnData> Columns
        {
            get
            {
                return columns;
            }
        }

        private List<TableItem> items = new List<TableItem>();
        public List<TableItem> Items
        {
            get
            {
                return items;
            }
        }

        private int currentPosition = 0;

        public void ScrollDown()
        {
            if (this.currentPosition + this.CalculateItemCountPerPage() < this.Items.Count)
                this.currentPosition += this.CalculateItemCountPerPage();
            this.ShowTable();
        }

        public void ScrollUp()
        {
            if (this.currentPosition - this.CalculateItemCountPerPage() >= 0)
                this.currentPosition -= this.CalculateItemCountPerPage();
            else
                this.currentPosition = 0;
            this.ShowTable();
        }

        public void ShowTable()
        {
            Button b;
            int j;
            foreach (Button bb in this.buttons)
            {
                this.Controls.Remove(bb);
                bb.Dispose();
            }
            for (int i = this.currentPosition; i < this.currentPosition + this.CalculateItemCountPerPage(); i++)
            {
                if (i < this.items.Count)
                {
                    b = new Button();
                    b.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
                    b.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
                    b.FillStyle = LCARS.ButtonFillStyle.UnfilledColumns;
                    b.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    b.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
                    b.Label = "";
                    b.Location = new System.Drawing.Point(0, 26 + 26 * (i - this.currentPosition));
                    b.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
                    b.Name = "button4";
                    b.Size = new System.Drawing.Size(this.Width, 20);
                    b.TabIndex = 28;
                    //b.Visible = false;
                    //b.Click += new System.EventHandler(this.button4_Click);
                    j = 0;
                    foreach (ColumnData d in this.columns)
                    {
                        b.Columns.Add(new ColumnData(this.items[i].Data[j], this.columns[j].WidthPercent));
                        j++;
                    }
                    this.Controls.Add(b);
                    buttons.Add(b);
                }
            }
            this.Headline.Invalidate();
        }
        List<Button> buttons = new List<Button>();

        private int CalculateItemCountPerPage()
        {
            return this.Height / 26;
        }

    }
    public class TableItem : IComparable<TableItem>
    {
        public string Sorting = "";
        public List<string> Data = new List<string>();
        public static bool operator < (TableItem i, TableItem i2)
        {
            return i.Sorting.CompareTo(i2.Sorting) < 0;
        }
        public static bool operator > (TableItem i, TableItem i2)
        {
            return i.Sorting.CompareTo(i2.Sorting) < 0;
        }
        public int CompareTo(TableItem other)
        {
            return this.Sorting.CompareTo(other.Sorting);
        }
    }
}
