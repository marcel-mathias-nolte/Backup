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
    public partial class PasswordChangeDialog : Form
    {
        
        private bool hasNumber = false;
        private bool hasSpecialSign = false;
        private bool hasSmallLetter = false;
        private bool hasCapitalLetter = false;

        private bool mustHaveNumber = false;
        private bool mustHaveSpecialSign = false;
        private bool mustHaveSmallLetter = false;
        private bool mustHaveCapitalLetter = false;

        private int minimumLength = 0;
        private int maximumLength = Int32.MaxValue;

        public PasswordChangeDialog()
        {
            InitializeComponent();
        }

        public string Password
        {
            get
            {
                return textBox1.Text;
            }
        }

        public bool ContainsCapitalLetters
        {
            get
            {
                return hasCapitalLetter;
            }
        }

        public bool ContainsSmallLetters
        {
            get
            {
                return hasSmallLetter;
            }
        }

        public bool ContainsSpecialSigns
        {
            get
            {
                return hasSpecialSign;
            }
        }

        public bool ContainsNumbers
        {
            get
            {
                return hasNumber;
            }
        }

        public bool MustContainCapitalLetters
        {
            get
            {
                return mustHaveCapitalLetter;
            }
            set
            {
                mustHaveCapitalLetter = value;
            }
        }

        public bool MustContainSmallLetters
        {
            get
            {
                return mustHaveSmallLetter;
            }
            set
            {
                mustHaveSmallLetter = value;
            }
        }

        public bool MustContainSpecialSigns
        {
            get
            {
                return mustHaveSpecialSign;
            }
            set
            {
                mustHaveSpecialSign = value;
            }
        }

        public bool MustContainNumbers
        {
            get
            {
                return mustHaveNumber;
            }
            set
            {
                mustHaveNumber = value;
            }
        }

        public int MinimumLength
        {
            get
            {
                return this.minimumLength;
            }
            set
            {
                this.minimumLength = value;
            }
        }

        public int MaximumLength
        {
            get
            {
                return this.maximumLength;
            }
            set
            {
                this.maximumLength = value;
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

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "Passwort wiederholen")
            {
                textBox2.Font = new Font(textBox2.Font, FontStyle.Regular);
                textBox2.ForeColor = SystemColors.WindowText;
                textBox2.Text = "";
                textBox2.PasswordChar = '●';
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text.Trim() == "")
            {
                textBox2.Font = new Font(textBox2.Font, FontStyle.Italic);
                textBox2.ForeColor = SystemColors.GrayText;
                textBox2.Text = "Passwort wiederholen";
                textBox2.PasswordChar = default(char);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "Passwort eingeben")
            {
                foreach (char c in textBox1.Text)
                {
                    if (c == '0' || c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' || c == '9')
                    {
                        hasNumber = true;
                    }
                    else if (
                        c == 'A' || c == 'B' || c == 'C' || c == 'D' || c == 'E' || c == 'F' || c == 'G' || c == 'H' || c == 'I' || c == 'J' || c == 'K' || c == 'L' || c == 'M' ||
                        c == 'N' || c == 'O' || c == 'P' || c == 'Q' || c == 'R' || c == 'S' || c == 'T' || c == 'U' || c == 'V' || c == 'W' || c == 'X' || c == 'Y' || c == 'Z'
                    )
                    {
                        hasCapitalLetter = true;
                    }
                    else if (
                        c == 'a' || c == 'b' || c == 'c' || c == 'd' || c == 'e' || c == 'f' || c == 'g' || c == 'h' || c == 'i' || c == 'j' || c == 'k' || c == 'l' || c == 'm' ||
                        c == 'n' || c == 'o' || c == 'p' || c == 'q' || c == 'r' || c == 's' || c == 't' || c == 'u' || c == 'v' || c == 'w' || c == 'x' || c == 'y' || c == 'z'
                    )
                    {
                        hasSmallLetter = true;
                    }
                    else
                    {
                        hasSpecialSign = true;
                    }
                }
                int securelevel = 0;
                if (hasCapitalLetter)
                {
                    securelevel++;
                }
                if (hasNumber)
                {
                    securelevel++;
                }
                if (hasSmallLetter)
                {
                    securelevel++;
                }
                if (hasSmallLetter)
                {
                    securelevel++;
                }
                if (textBox1.Text.Length > 8)
                {
                    securelevel++;
                }
                if (textBox1.Text.Length > 12)
                {
                    securelevel++;
                }
                progressBar1.Value = securelevel;
            }
            else
            {
                progressBar1.Value = 0;
            }
            verifyPassword();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            verifyPassword();
        }

        private void verifyPassword()
        {
            bool anforderung = (!mustHaveCapitalLetter || hasCapitalLetter) && (!mustHaveNumber || hasNumber) && (!mustHaveSmallLetter || hasSmallLetter) && (!mustHaveSpecialSign || hasSpecialSign) && textBox1.Text.Length > this.minimumLength && textBox1.Text.Length < this.maximumLength;
            if (anforderung)
            {
                label1.Text = "";
            }
            else
            {
                label1.Text = this.passwordRule;

            }
            if (textBox1.Text != textBox2.Text && textBox2.Text != "Passwort wiederholen")
            {
                textBox2.ForeColor = Color.Red;
                button1.Enabled = false;
            }
            else if (textBox2.Text != "Passwort wiederholen")
            {
                textBox2.ForeColor = SystemColors.WindowText;
                button1.Enabled = anforderung;
            }
        }

        string passwordRule = "";

        private void PasswordChangeDialog_Shown(object sender, EventArgs e)
        {
            List<string> s = new List<string>();
            if (mustHaveCapitalLetter)
            {
                s.Add("Großbuchstaben");
            }
            if (mustHaveSmallLetter)
            {
                s.Add("Kleinbuchstaben");
            }
            if (mustHaveNumber)
            {
                s.Add("Ziffern");
            }
            if (mustHaveSpecialSign)
            {
                s.Add("Sonderzeichen");
            }
            string t = "Das Passwort muss ";
            if (s.Count > 1)
            {
                while (s.Count > 1)
                {
                    t += s[0] + (s.Count > 2 ? ", " : " und ");
                    s.RemoveAt(0);
                }
                t += s[0] + " enthalten";
                s.RemoveAt(0);
            }
            if (s.Count == 1)
            {
                t += s[0] + " enthalten";
                s.RemoveAt(0);
            }
            if (this.minimumLength > 0 || this.maximumLength < Int32.MaxValue)
            {
                if (t != "Das Passwort muss ")
                {
                    t += ", sowie ";
                }
                if (this.minimumLength > 0 && this.maximumLength < Int32.MaxValue)
                {
                    t += "zwischen " + this.minimumLength.ToString() + " und " + this.maximumLength.ToString() + " Zeichen lang sein";
                }
                else if (this.minimumLength > 0)
                {
                    t += "mindestens " + this.minimumLength.ToString() + " Zeichen lang sein";
                }
                else if (this.minimumLength > 0 && this.maximumLength < Int32.MaxValue)
                {
                    t += "maximal " + this.maximumLength.ToString() + " Zeichen lang sein";
                }
            }
            t += ".";
            this.passwordRule = t;
        }

    }
}
