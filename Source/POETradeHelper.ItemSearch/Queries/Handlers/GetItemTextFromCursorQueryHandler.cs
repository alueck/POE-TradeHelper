using System.Threading;
using System.Threading.Tasks;
using MediatR;
using POETradeHelper.Common.Contract;
using POETradeHelper.ItemSearch.Contract.Queries;

namespace POETradeHelper.ItemSearch.Queries.Handlers
{
    public class GetItemTextFromCursorQueryHandler : IRequestHandler<GetItemTextFromCursorQuery, string>
    {
        private readonly IClipboardHelper clipboardHelper;
        private readonly INativeKeyboard nativeKeyboard;

        public GetItemTextFromCursorQueryHandler(IClipboardHelper clipboardHelper, INativeKeyboard nativeKeyboard)
        {
            this.clipboardHelper = clipboardHelper;
            this.nativeKeyboard = nativeKeyboard;
        }

        public async Task<string> Handle(GetItemTextFromCursorQuery request, CancellationToken cancellationToken)
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