using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Nolte.UI
{
    public partial class PasswordDialog : Form
    {
        public PasswordDialog()
        {
            InitializeComponent();
        }

        public string Password
        {
            get
            {
                return textBox1.Text == "Passwort eingeben" ? "" : textBox1.Text;
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Passwort eingeben")
            {
                textBox1.Font = new Font(textBox1.Font, FontStyle.Regular);
                textBox1.ForeColor = SystemColors.WindowText;
                textBox1.Text = "";
                textBox1.PasswordChar = '●';
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "")
            {
                textBox1.Font = new Font(textBox1.Font, FontStyle.Italic);
                textBox1.ForeColor = SystemColors.GrayText;
                textBox1.Text = "Passwort eingeben";
                textBox1.PasswordChar = default(char);
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' || e.KeyChar == '\n')
            {
                e.Handled = true;
                button1.PerformClick();
            }
        }

    }
}
