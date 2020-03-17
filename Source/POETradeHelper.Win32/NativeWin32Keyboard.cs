using POETradeHelper.Common.Contract;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace POETradeHelper.Win32
{
    [ExcludeFromCodeCoverage]
    public class NativeWin32Keyboard : INativeKeyboard
    {
        public void SendCopyCommand()
        {
            SendKeys.SendWait("^C");
        }
    }
}