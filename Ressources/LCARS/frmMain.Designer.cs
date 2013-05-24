namespace LCARS
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.table1 = new LCARS.Table();
            this.upDownControl1 = new LCARS.UpDownControl();
            this.button8 = new LCARS.Button();
            this.button7 = new LCARS.Button();
            this.button6 = new LCARS.Button();
            this.curve5 = new LCARS.Curve();
            this.bar11 = new LCARS.Bar();
            this.curve4 = new LCARS.Curve();
            this.bar10 = new LCARS.Bar();
            this.bar9 = new LCARS.Bar();
            this.bar8 = new LCARS.Bar();
            this.bar7 = new LCARS.Bar();
            this.bar6 = new LCARS.Bar();
            this.button2 = new LCARS.Button();
            this.button1 = new LCARS.Button();
            this.bar5 = new LCARS.Bar();
            this.curve3 = new LCARS.Curve();
            this.bar4 = new LCARS.Bar();
            this.curve2 = new LCARS.Curve();
            this.bar3 = new LCARS.Bar();
            this.threeMonthCalendar1 = new LCARS.ThreeMonthCalendar();
            this.calendar1 = new LCARS.Calendar();
            this.bar2 = new LCARS.Bar();
            this.curve1 = new LCARS.Curve();
            this.bar1 = new LCARS.Bar();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Lucida Console", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Yellow;
            this.label1.Location = new System.Drawing.Point(138, 154);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 19);
            this.label1.TabIndex = 5;
            this.label1.Text = "label1";
            this.label1.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Lucida Console", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Yellow;
            this.label2.Location = new System.Drawing.Point(138, 179);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 19);
            this.label2.TabIndex = 6;
            this.label2.Text = "label2";
            this.label2.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Lucida Console", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Yellow;
            this.label3.Location = new System.Drawing.Point(149, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(803, 48);
            this.label3.TabIndex = 12;
            this.label3.Text = "TACTICAL COMPUTER INTERFACE";
            // 
            // table1
            // 
            this.table1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.table1.Location = new System.Drawing.Point(503, 150);
            this.table1.Name = "table1";
            this.table1.Size = new System.Drawing.Size(414, 331);
            this.table1.TabIndex = 34;
            this.table1.Visible = false;
            // 
            // upDownControl1
            // 
            this.upDownControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.upDownControl1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.upDownControl1.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.upDownControl1.Location = new System.Drawing.Point(925, 398);
            this.upDownControl1.Name = "upDownControl1";
            this.upDownControl1.Size = new System.Drawing.Size(24, 83);
            this.upDownControl1.TabIndex = 33;
            this.upDownControl1.Up += new LCARS.DirectionEventHandler(this.upDownControl1_Up);
            this.upDownControl1.Down += new LCARS.DirectionEventHandler(this.upDownControl1_Down);
            // 
            // button8
            // 
            this.button8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button8.BackgroundColor = System.Drawing.Color.Red;
            this.button8.FillStyle = LCARS.ButtonFillStyle.UnFilled;
            this.button8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button8.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.button8.Label = "LÖSCHEN";
            this.button8.Location = new System.Drawing.Point(929, 313);
            this.button8.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(119, 38);
            this.button8.TabIndex = 32;
            this.button8.Visible = false;
            // 
            // button7
            // 
            this.button7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button7.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.button7.FillStyle = LCARS.ButtonFillStyle.UnFilled;
            this.button7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button7.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.button7.Label = "ÄNDERN";
            this.button7.Location = new System.Drawing.Point(929, 269);
            this.button7.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(119, 38);
            this.button7.TabIndex = 31;
            this.button7.Visible = false;
            // 
            // button6
            // 
            this.button6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button6.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.button6.FillStyle = LCARS.ButtonFillStyle.Filled;
            this.button6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button6.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.button6.Label = "NEU";
            this.button6.Location = new System.Drawing.Point(929, 225);
            this.button6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(119, 38);
            this.button6.TabIndex = 30;
            this.button6.Visible = false;
            // 
            // curve5
            // 
            this.curve5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.curve5.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.curve5.Font = new System.Drawing.Font("Lucida Console", 8F, System.Drawing.FontStyle.Bold);
            this.curve5.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.curve5.Location = new System.Drawing.Point(985, 457);
            this.curve5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.curve5.Name = "curve5";
            this.curve5.Size = new System.Drawing.Size(140, 60);
            this.curve5.Style = LCARS.CurveStyle.DownLeft;
            this.curve5.TabIndex = 26;
            // 
            // bar11
            // 
            this.bar11.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.bar11.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.bar11.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.bar11.Label = "";
            this.bar11.Location = new System.Drawing.Point(220, 487);
            this.bar11.Name = "bar11";
            this.bar11.Size = new System.Drawing.Size(758, 30);
            this.bar11.TabIndex = 25;
            // 
            // curve4
            // 
            this.curve4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.curve4.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.curve4.Font = new System.Drawing.Font("Lucida Console", 8F, System.Drawing.FontStyle.Bold);
            this.curve4.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.curve4.Location = new System.Drawing.Point(14, 457);
            this.curve4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.curve4.Name = "curve4";
            this.curve4.Size = new System.Drawing.Size(199, 60);
            this.curve4.Style = LCARS.CurveStyle.DownRight;
            this.curve4.TabIndex = 24;
            // 
            // bar10
            // 
            this.bar10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.bar10.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.bar10.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.bar10.Label = "";
            this.bar10.Location = new System.Drawing.Point(1055, 179);
            this.bar10.Name = "bar10";
            this.bar10.Size = new System.Drawing.Size(70, 272);
            this.bar10.TabIndex = 23;
            // 
            // bar9
            // 
            this.bar9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.bar9.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.bar9.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.bar9.Label = "SETUP";
            this.bar9.Location = new System.Drawing.Point(13, 363);
            this.bar9.Name = "bar9";
            this.bar9.Size = new System.Drawing.Size(101, 88);
            this.bar9.TabIndex = 19;
            // 
            // bar8
            // 
            this.bar8.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.bar8.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.bar8.Label = "MEDIEN";
            this.bar8.Location = new System.Drawing.Point(13, 317);
            this.bar8.Name = "bar8";
            this.bar8.Size = new System.Drawing.Size(101, 40);
            this.bar8.TabIndex = 18;
            // 
            // bar7
            // 
            this.bar7.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.bar7.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.bar7.Label = "E-MAILS";
            this.bar7.Location = new System.Drawing.Point(13, 271);
            this.bar7.Name = "bar7";
            this.bar7.Size = new System.Drawing.Size(101, 40);
            this.bar7.TabIndex = 17;
            // 
            // bar6
            // 
            this.bar6.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.bar6.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.bar6.Label = "PROGRAMME";
            this.bar6.Location = new System.Drawing.Point(13, 225);
            this.bar6.Name = "bar6";
            this.bar6.Size = new System.Drawing.Size(101, 40);
            this.bar6.TabIndex = 16;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.button2.FillStyle = LCARS.ButtonFillStyle.Filled;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.button2.Label = "JAHRES- KALENDER";
            this.button2.Location = new System.Drawing.Point(929, 225);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(119, 38);
            this.button2.TabIndex = 14;
            this.button2.Visible = false;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.button1.FillStyle = LCARS.ButtonFillStyle.Filled;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.button1.Label = "TERMINE";
            this.button1.Location = new System.Drawing.Point(929, 179);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(119, 40);
            this.button1.TabIndex = 13;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // bar5
            // 
            this.bar5.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.bar5.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.bar5.Label = "EXIT";
            this.bar5.Location = new System.Drawing.Point(13, 1);
            this.bar5.Name = "bar5";
            this.bar5.Size = new System.Drawing.Size(101, 40);
            this.bar5.TabIndex = 11;
            this.bar5.Click += new System.EventHandler(this.bar5_Click);
            // 
            // curve3
            // 
            this.curve3.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.curve3.Font = new System.Drawing.Font("Lucida Console", 8F, System.Drawing.FontStyle.Bold);
            this.curve3.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.curve3.Location = new System.Drawing.Point(13, 47);
            this.curve3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.curve3.Name = "curve3";
            this.curve3.Size = new System.Drawing.Size(200, 60);
            this.curve3.Style = LCARS.CurveStyle.DownRight;
            this.curve3.TabIndex = 10;
            // 
            // bar4
            // 
            this.bar4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.bar4.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.bar4.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.bar4.Label = "LIGHT";
            this.bar4.Location = new System.Drawing.Point(220, 77);
            this.bar4.Name = "bar4";
            this.bar4.Size = new System.Drawing.Size(905, 30);
            this.bar4.TabIndex = 9;
            this.bar4.Click += new System.EventHandler(this.bar4_Click);
            // 
            // curve2
            // 
            this.curve2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.curve2.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.curve2.Font = new System.Drawing.Font("Lucida Console", 8F, System.Drawing.FontStyle.Bold);
            this.curve2.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.curve2.Location = new System.Drawing.Point(925, 113);
            this.curve2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.curve2.Name = "curve2";
            this.curve2.Size = new System.Drawing.Size(200, 60);
            this.curve2.Style = LCARS.CurveStyle.UpLeft;
            this.curve2.TabIndex = 8;
            // 
            // bar3
            // 
            this.bar3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.bar3.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.bar3.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.bar3.Label = "";
            this.bar3.Location = new System.Drawing.Point(503, 113);
            this.bar3.Name = "bar3";
            this.bar3.Size = new System.Drawing.Size(415, 31);
            this.bar3.TabIndex = 7;
            // 
            // threeMonthCalendar1
            // 
            this.threeMonthCalendar1.Font = new System.Drawing.Font("Lucida Console", 8F, System.Drawing.FontStyle.Bold);
            this.threeMonthCalendar1.ForeColor = System.Drawing.Color.Yellow;
            this.threeMonthCalendar1.Location = new System.Drawing.Point(341, 237);
            this.threeMonthCalendar1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.threeMonthCalendar1.Name = "threeMonthCalendar1";
            this.threeMonthCalendar1.Size = new System.Drawing.Size(143, 108);
            this.threeMonthCalendar1.TabIndex = 4;
            this.threeMonthCalendar1.Visible = false;
            // 
            // calendar1
            // 
            this.calendar1.Location = new System.Drawing.Point(151, 215);
            this.calendar1.Name = "calendar1";
            this.calendar1.Size = new System.Drawing.Size(170, 163);
            this.calendar1.TabIndex = 3;
            this.calendar1.Visible = false;
            // 
            // bar2
            // 
            this.bar2.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.bar2.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.bar2.Label = "";
            this.bar2.Location = new System.Drawing.Point(220, 113);
            this.bar2.Name = "bar2";
            this.bar2.Size = new System.Drawing.Size(277, 31);
            this.bar2.TabIndex = 2;
            // 
            // curve1
            // 
            this.curve1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.curve1.Font = new System.Drawing.Font("Lucida Console", 8F, System.Drawing.FontStyle.Bold);
            this.curve1.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.curve1.Location = new System.Drawing.Point(13, 113);
            this.curve1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.curve1.Name = "curve1";
            this.curve1.Size = new System.Drawing.Size(200, 60);
            this.curve1.Style = LCARS.CurveStyle.UpRight;
            this.curve1.TabIndex = 1;
            // 
            // bar1
            // 
            this.bar1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.bar1.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.bar1.Label = "KALENDER";
            this.bar1.Location = new System.Drawing.Point(13, 179);
            this.bar1.Name = "bar1";
            this.bar1.Size = new System.Drawing.Size(101, 40);
            this.bar1.TabIndex = 0;
            this.bar1.Click += new System.EventHandler(this.bar1_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1138, 529);
            this.ControlBox = false;
            this.Controls.Add(this.table1);
            this.Controls.Add(this.upDownControl1);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.curve5);
            this.Controls.Add(this.bar11);
            this.Controls.Add(this.curve4);
            this.Controls.Add(this.bar10);
            this.Controls.Add(this.bar9);
            this.Controls.Add(this.bar8);
            this.Controls.Add(this.bar7);
            this.Controls.Add(this.bar6);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.bar5);
            this.Controls.Add(this.curve3);
            this.Controls.Add(this.bar4);
            this.Controls.Add(this.curve2);
            this.Controls.Add(this.bar3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.threeMonthCalendar1);
            this.Controls.Add(this.calendar1);
            this.Controls.Add(this.bar2);
            this.Controls.Add(this.curve1);
            this.Controls.Add(this.bar1);
            this.Name = "frmMain";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Bar bar1;
        private Curve curve1;
        private Bar bar2;
        private Calendar calendar1;
        private System.Windows.Forms.Timer timer1;
        private ThreeMonthCalendar threeMonthCalendar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private Bar bar3;
        private Curve curve2;
        private Bar bar4;
        private Curve curve3;
        private Bar bar5;
        private System.Windows.Forms.Label label3;
        private Button button1;
        private Button button2;
        private Bar bar6;
        private Bar bar7;
        private Bar bar8;
        private Bar bar9;
        private Bar bar10;
        private Curve curve4;
        private Bar bar11;
        private Curve curve5;
        private Button button6;
        private Button button7;
        private Button button8;
        private UpDownControl upDownControl1;
        private Table table1;
    }
}