using System;
using System.Windows.Forms;

namespace WinBackup
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();


            Nolte.Security.UAC.CreateElevateButton(button1);
            imageEditor1.MenuVisible = true;
            timer1.Tick += (o, e) => { txtStatus.Text = DateTime.Now.ToString(); };
            timer1.Start();
        }

        public delegate void InvokeDelegate();

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BeginInvoke(new InvokeDelegate(() =>
                {
                    var form = new FrmAbout();
                    form.ShowDialog(this);                  
                }));
        }

        private void newJobToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BeginInvoke(new InvokeDelegate(() =>
            {
                var form = new frmWizard();
                form.ShowDialog(this);
            }));
        }


    }
}
