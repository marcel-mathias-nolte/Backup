using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinBackup
{
    public partial class pnlTaskSelect : UserControl
    {
        public pnlTaskSelect()
        {
            InitializeComponent();
            btnBackupIncremental.GotFocus += this.HighlightButton;
            btnBackupIncremental.LostFocus += this.HighlightButton;
            btnBackupMirror.GotFocus += this.HighlightButton;
            btnBackupMirror.LostFocus += this.HighlightButton;
            btnBackupProtocol.GotFocus += this.HighlightButton;
            btnBackupProtocol.LostFocus += this.HighlightButton;
            btnBackupIncremental.Click += (o, e) => { if (SelectionMade != null) SelectionMade(SelectionOption.Snapshot); };
            btnBackupMirror.Click += (o, e) => { if (SelectionMade != null) SelectionMade(SelectionOption.Synchronize); };
            btnBackupProtocol.Click += (o, e) => { if (SelectionMade != null) SelectionMade(SelectionOption.Protocol); };
        }

        private void HighlightButton(object sender, EventArgs e)
        {
            try
            {
                Button b = (Button)sender;
                b.BackColor = b.Focused ? Color.LightGreen : default(Color);
                if (!b.Focused)
                    b.UseVisualStyleBackColor = true;
            }
            catch (Exception)
            {
            }
        }

        public delegate void SelectionMadeDelegate(SelectionOption e);
        public enum SelectionOption
        {
            Synchronize,
            Snapshot,
            Protocol
        }

        public event SelectionMadeDelegate SelectionMade;
    }
}
