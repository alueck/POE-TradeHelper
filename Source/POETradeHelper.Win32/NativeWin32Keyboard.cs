using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using POETradeHelper.Common.Contract;

namespace POETradeHelper.Win32
{
    [ExcludeFromCodeCoverage]
    public class NativeWin32Keyboard : INativeKeyboard
    {
        public void SendCopyCommand()
        {
            SendKeys.SendWait("^C");
        }

        public void SendGotoHideoutCommand()
        {
            SendKeys.SendWait("{ENTER}/hideout{ENTER}{ENTER}{UP}{UP}{ESC}");
        }
    }
}