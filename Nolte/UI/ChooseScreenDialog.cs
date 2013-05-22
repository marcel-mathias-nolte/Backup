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
    public partial class ChooseScreenDialog : Form
    {
        public ChooseScreenDialog()
        {
            InitializeComponent();
            foreach (Screen scr in Screen.AllScreens)
            {
                if (scr.Equals(Screen.PrimaryScreen))
                {
                    AddRadioButton("Primärmonitor (" + scr.Bounds.Width.ToString() + "x" + scr.Bounds.Height.ToString() + ")", scr);
                }
                else
                {
                    AddRadioButton("Sekundärmonitor (" + scr.Bounds.Width.ToString() + "x" + scr.Bounds.Height.ToString() + ")", scr);
                }
            }
            AddRadioButton("alle", null);
            radioButtons[radioButtons.Count - 1].Checked = true;
        }
        private List<RadioButton> radioButtons = new List<RadioButton>();
        private void AddRadioButton(string Label, Screen scr)
        {
            RadioButton radioButton = new System.Windows.Forms.RadioButton();
            radioButton.AutoSize = true;
            radioButton.Location = new System.Drawing.Point(12, 12 + (radioButtons.Count) * 23);
            radioButton.Size = new System.Drawing.Size(85, 17);
            radioButton.TabIndex = 0;
            radioButton.TabStop = true;
            radioButton.Text = Label;
            radioButton.Tag = scr;
            radioButton.UseVisualStyleBackColor = true;
            radioButtons.Add(radioButton);
            Controls.Add(radioButton);
            this.Height += 23;
        }
        public Screen ChoosenScreen
        {
            get
            {
                foreach (RadioButton r in radioButtons)
                {
                    if (r.Checked)
                    {
                        return r.Tag == null ? null : (Screen)r.Tag;
                    }
                }
                return null;
            }
        }
    }
}
