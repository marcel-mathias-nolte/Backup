using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Nolte.UI
{
    public static class Api
    {
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        public const int GWL_EXSTYLE = (-20);
        public const int WS_EX_TRANSPARENT = 0x20;
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public override string ToString()
            {
                return ("Left :" + left.ToString() + "," + "Top :" + top.ToString() + "," + "Right :" + right.ToString() + "," + "Bottom :" + bottom.ToString());
            }
        }
        [DllImport("user32.dll", EntryPoint = "FindWindowEx", CharSet = CharSet.Auto)]
        public extern static IntPtr FindWindowEx(
            IntPtr hwndParent,
            IntPtr hwndChildAfter,
            [MarshalAs(UnmanagedType.LPTStr)]
  			string lpszClass,
            [MarshalAs(UnmanagedType.LPTStr)]
  			string lpszWindow);
        [DllImport("user32.dll", EntryPoint = "DrawAnimatedRects", CharSet = CharSet.Auto)]
        public static extern bool DrawAnimatedRects(IntPtr hwnd, int idAni, ref RECT lprcFrom, ref RECT lprcTo);
        [DllImport("user32.dll", EntryPoint = "GetWindowRect", CharSet = CharSet.Auto)]
        public extern static bool GetWindowRect(IntPtr hwnd, ref RECT lpRect);
        public const int WM_QUERYENDSESSION = 0x11;
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MINIMIZE = 0xF020;
        public const int IDANI_CAPTION = 3;
        [DllImport("User32.dll")]
        public static extern int SetClipboardViewer(int hWndNewViewer);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
        public const int WM_DRAWCLIPBOARD = 0x308;
        public const int WM_CHANGECBCHAIN = 0x030D;
        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0;
        public const uint SHGFI_SMALLICON = 0x1;
        public const int SHIL_JUMBO = 0x4;
        public const int SHIL_EXTRALARGE = 0x2;
        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
        [DllImport("user32.dll")]
        public static extern int DestroyIcon(IntPtr hIcon);
    }
}
