using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinBackup
{
    public partial class frmWizard : Form
    {
        public frmWizard()
        {
            InitializeComponent();
            pnlTaskSelect1.SelectionMade += (e) =>
            {
                switch (e)
                {
                    case pnlTaskSelect.SelectionOption.Protocol:
                        break;
                    case pnlTaskSelect.SelectionOption.Snapshot:
                        break;
                    case pnlTaskSelect.SelectionOption.Synchronize:
                        break;
                }
            };
        }
    }
}
