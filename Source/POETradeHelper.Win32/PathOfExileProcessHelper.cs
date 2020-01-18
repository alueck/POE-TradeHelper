using POETradeHelper.Common.Contract;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace POETradeHelper
{
    [ExcludeFromCodeCoverage]
    public class PathOfExileProcessHelper : IPathOfExileProcessHelper
    {
        private const string PATH_OF_EXILE_PROCESS_TITLE = "Path of Exile";

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        public bool IsPathOfExileActiveWindow()
        {
            GetWindowThreadProcessId(GetForegroundWindow(), out int processID);
            var processToCheck = Process.GetProcessById(processID);

            return processToCheck?.MainWindowTitle == PATH_OF_EXILE_PROCESS_TITLE;
        }
    }
}