using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using POETradeHelper.Common.Contract;

namespace POETradeHelper.Win32
{
    [ExcludeFromCodeCoverage]
    public class PathOfExileProcessHelper : IPathOfExileProcessHelper
    {
        private const string PathOfExileProcessTitle = "Path of Exile";

        public bool IsPathOfExileActiveWindow()
        {
            GetWindowThreadProcessId(GetForegroundWindow(), out int processID);
            Process processToCheck = Process.GetProcessById(processID);

            return processToCheck.MainWindowTitle == PathOfExileProcessTitle;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
    }
}