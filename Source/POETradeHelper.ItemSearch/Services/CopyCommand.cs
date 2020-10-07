using System.Threading.Tasks;
using POETradeHelper.Common.Contract;
using POETradeHelper.ItemSearch.Contract.Services;

namespace POETradeHelper.ItemSearch.Services
{
    public class CopyCommand : ICopyCommand
    {
        private readonly IClipboardHelper clipboardHelper;
        private readonly INativeKeyboard nativeKeyboard;

        public CopyCommand(IClipboardHelper clipboardHelper, INativeKeyboard nativeKeyboard)
        {
            this.clipboardHelper = clipboardHelper;
            this.nativeKeyboard = nativeKeyboard;
        }

        public async Task<string> ExecuteAsync(System.Threading.CancellationToken cancellationToken = default)
        {
            string clipBoardTemp = await this.clipboardHelper.GetTextAsync();

            this.nativeKeyboard.SendCopyCommand();

            //small delay, because the text is not always directly available after the copy key command
            await Task.Delay(200, cancellationToken);

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