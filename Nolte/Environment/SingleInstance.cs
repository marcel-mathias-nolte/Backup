using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Nolte.Environment
{
    /// <summary>
    /// Ensure that there is only one instance per user / machine
    /// [STAThread]
    /// static void Main()
    /// {
    ///     if (!SingleInstance.Start(SingleInstanceMode.PerUserSession))
    ///     {
    ///         SingleInstance.ShowFirstInstance();
    ///         return;
    ///     }
    ///     Application.EnableVisualStyles();
    ///     Application.SetCompatibleTextRenderingDefault(false);
    ///     Application.Run(new MainForm());
    ///     SingleInstance.Stop();
    /// }
    /// </summary>
    static public class SingleInstance
    {
        public static readonly int WM_SHOWFIRSTINSTANCE = Api.RegisterWindowMessage("WM_SHOWFIRSTINSTANCE|{0}", Api.AssemblyGuid);

        static Mutex mutex;

        static public bool TryStartBlocking(SingleInstanceMode Mode)
        {
            bool onlyInstance = false;
            string mutexName = String.Format(Mode == SingleInstanceMode.PerMachine ? "Global\\{0}" : "Local\\{0}", Api.AssemblyGuid);

            mutex = new Mutex(true, mutexName, out onlyInstance);
            return onlyInstance;
        }

        static public void ShowFirstInstance()
        {
            Api.PostMessage((IntPtr)Api.HWND_BROADCAST, WM_SHOWFIRSTINSTANCE, IntPtr.Zero, IntPtr.Zero);
        }

        static public void StopBlocking()
        {
            mutex.ReleaseMutex();
        }

        public static void ShowToFront(Form form)
        {
            Api.ShowWindow(form.Handle, Api.SW_SHOWNORMAL);
            Api.SetForegroundWindow(form.Handle);
        }
        
        public static void ShowToFront(IntPtr window)
        {
            Api.ShowWindow(window, Api.SW_SHOWNORMAL);
            Api.SetForegroundWindow(window);
        }
    }
}