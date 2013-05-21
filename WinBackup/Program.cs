using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Nolte.Security;

namespace WinBackup
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Contains("/uac") && !UAC.IsRunAsAdmin())
            {
                UAC.DoElevate();
            }
            Application.Run(new frmMain());
        }
    }
}
