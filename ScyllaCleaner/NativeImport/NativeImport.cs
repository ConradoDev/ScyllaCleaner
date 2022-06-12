using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spotify.NativeImport {
    static class NativeImport {

        internal class ConsolePropertyModifiers {
            internal const int SW_HIDE = 0;
            internal const int SW_SHOW = 5;
            internal const int MY_CODE_PAGE = 437;
            internal const uint GENERIC_WRITE = 0x40000000;
            internal const uint GENERIC_READ = 0x80000000;
            internal const uint FILE_SHARE_WRITE = 0x2;
            internal const uint OPEN_EXISTING = 0x3;

            internal const int WM_NCLBUTTONDOWN = 0xA1;
            internal const int HTCAPTION = 0x2;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr CreateFile(
        string lpFileName,
        uint dwDesiredAccess,
        uint dwShareMode,
        uint lpSecurityAttributes,
        uint dwCreationDisposition,
        uint dwFlagsAndAttributes,
        uint hTemplateFile);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll")]
        internal static extern bool SetStdHandle(int nStdHandle, IntPtr hHandle);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetConsoleWindow();

        [DllImport("shell32",
         CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        public static extern string SHGetKnownFolderPath(
        [MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, uint hToken = 0);
    }
}