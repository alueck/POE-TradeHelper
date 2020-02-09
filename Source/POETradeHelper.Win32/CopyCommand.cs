using POETradeHelper.Common.Contract;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POETradeHelper.Win32
{
    public class CopyCommand : ICopyCommand
    {
        private readonly IClipboardHelper clipboardHelper;

        public CopyCommand(IClipboardHelper clipboardHelper)
        {
            this.clipboardHelper = clipboardHelper;
        }

        public async Task<string> ExecuteAsync(System.Threading.CancellationToken cancellationToken = default)
        {
            string clipBoardTemp = await this.clipboardHelper.GetTextAsync();

            SendKeys.SendWait("^{c}");

            //small delay, because the text is not always directly available after the copy key command
            await Task.Delay(100, cancellationToken);

            string itemString = await this.clipboardHelper.GetTextAsync();

            if (string.IsNullOrEmpty(clipBoardTemp))
            {
                await this.clipboardHelper.ClearAsync();
            }
            else
            {
                await this.clipboardHelper.SetTextAsync(clipBoardTemp);
            }

            return itemString;
        }
    }
}