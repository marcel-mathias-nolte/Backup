using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Nolte.UI
{
    public class InputDialog : Form
    {
        private MessageBoxButtons mbb = MessageBoxButtons.OK;
        private string caption;
        private string message;
        public InputDialog()
        {
            InitializeComponent();
            ChangeButtons();
            caption = this.Text;
            message = "";
            label1.Text = message;
            ResizeLabel();
        }
        private void ResizeLabel()
        {
            int width = label1.PreferredSize.Width + 2 * label1.Left;
            if (width < 267)
                width = 267;
            if (width > Screen.GetWorkingArea(this).Width)
            {
                label1.MaximumSize = new Size(width - label1.Left * 2, 0);
                width = Screen.GetWorkingArea(this).Width;
            }
            int height = label1.PreferredSize.Height + this.Height;
            if (height > Screen.GetWorkingArea(this).Height)
            {
                label1.MaximumSize = new Size(label1.MaximumSize.Width, height - this.Height);
                height = Screen.GetWorkingArea(this).Height;
            }
            this.Size = new Size(width, height);
        }
        private void InputDialog_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
        }
        public string Label
        {
            get
            {
                return caption;
            }
            set
            {
                this.caption = value;
                this.Text = value;
            }
        }
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
                label1.Text = message;
                ResizeLabel();
            }
        }
        public string Value
        {
            get
            {
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
            }
        }
        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                button1.PerformClick();
                e.Handled = true;
            }
        }
        private void ChangeButtons()
        {
            switch (mbb)
            {
                case MessageBoxButtons.OK:
                    button3.Visible = false;
                    button2.Visible = false;
                    button1.Visible = true;
                    button1.Text = "OK";
                    button1.DialogResult = DialogResult.OK;
                    break;
                case MessageBoxButtons.OKCancel:
                    button3.Visible = false;
                    button2.Visible = true;
                    button1.Visible = true;
                    button2.Text = "Abbrechen";
                    button1.Text = "OK";
                    button2.DialogResult = DialogResult.Cancel;
                    button1.DialogResult = DialogResult.OK;
                    break;
                case MessageBoxButtons.RetryCancel:
                    button3.Visible = false;
                    button2.Visible = true;
                    button1.Visible = true;
                    button2.Text = "Abbrechen";
                    button1.Text = "Wiederholen";
                    button2.DialogResult = DialogResult.Cancel;
                    button1.DialogResult = DialogResult.Retry;
                    break;
                case MessageBoxButtons.YesNo:
                    button3.Visible = false;
                    button2.Visible = true;
                    button1.Visible = true;
                    button2.Text = "Nein";
                    button1.Text = "Ja";
                    button2.DialogResult = DialogResult.No;
                    button1.DialogResult = DialogResult.Yes;
                    break;
                case MessageBoxButtons.YesNoCancel:
                    button3.Visible = true;
                    button2.Visible = true;
                    button1.Visible = true;
                    button3.Text = "Abbrechen";
                    button2.Text = "Nein";
                    button1.Text = "Ja";
                    button3.DialogResult = DialogResult.Cancel;
                    button2.DialogResult = DialogResult.No;
                    button1.DialogResult = DialogResult.Yes;
                    break;
                case MessageBoxButtons.AbortRetryIgnore:
                    button3.Visible = true;
                    button2.Visible = true;
                    button1.Visible = true;
                    button3.Text = "Abbrechen";
                    button2.Text = "Wiederholen";
                    button1.Text = "Ignorieren";
                    button3.DialogResult = DialogResult.Cancel;
                    button2.DialogResult = DialogResult.Retry;
                    button1.DialogResult = DialogResult.Ignore;
                    break;
            }
        }
        public MessageBoxButtons Buttons
        {
            get
            {
                return mbb;
            }
            set
            {
                mbb = value;
                ChangeButtons();
            }
        }
        private IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            label1 = new Label();
            textBox1 = new TextBox();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            SuspendLayout();
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(35, 13);
            label1.TabIndex = 0;
            label1.Text = "label1";
            textBox1.Anchor = ((AnchorStyles)(((AnchorStyles.Bottom | AnchorStyles.Left) | AnchorStyles.Right)));
            textBox1.Location = new Point(15, 12);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(237, 20);
            textBox1.TabIndex = 1;
            textBox1.KeyUp += new KeyEventHandler(textBox1_KeyUp);
            button1.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            button1.DialogResult = DialogResult.OK;
            button1.Location = new Point(177, 38);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 2;
            button1.Text = "OK";
            button1.UseVisualStyleBackColor = true;
            button2.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            button2.DialogResult = DialogResult.Cancel;
            button2.Location = new Point(96, 38);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 3;
            button2.Text = "Abbrechen";
            button2.UseVisualStyleBackColor = true;
            button3.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            button3.DialogResult = DialogResult.Cancel;
            button3.Location = new Point(15, 38);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 4;
            button3.Text = "Abbrechen";
            button3.UseVisualStyleBackColor = true;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(261, 71);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(textBox1);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "InputDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Eingabe";
            TopMost = true;
            Load += new System.EventHandler(InputDialog_Load);
            ResumeLayout(false);
            PerformLayout();
        }
        private Label label1;
        private TextBox textBox1;
        private Button button1;
        private Button button2;
        private Button button3;
    }
}
