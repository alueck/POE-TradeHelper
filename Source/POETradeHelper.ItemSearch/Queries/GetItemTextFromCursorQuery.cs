using MediatR;
using POETradeHelper.Common.Contract;

namespace POETradeHelper.ItemSearch.Queries;

public class GetItemTextFromCursorQuery : IRequest<string>
{
}

public class GetItemTextFromCursorQueryHandler : IRequestHandler<GetItemTextFromCursorQuery, string>
{
    private readonly IClipboardHelper clipboardHelper;
    private readonly IUserInputSimulator userInputSimulator;

    public GetItemTextFromCursorQueryHandler(IClipboardHelper clipboardHelper, IUserInputSimulator userInputSimulator)
    {
        this.clipboardHelper = clipboardHelper;
        this.userInputSimulator = userInputSimulator;
    }

    public async Task<string> Handle(GetItemTextFromCursorQuery request, CancellationToken cancellationToken)
    {
        const int maxTries = 8;
        string? clipBoardTemp = await this.clipboardHelper.GetTextAsync();

        this.userInputSimulator.SendCopyAdvancedItemStringCommand();

        string? itemString;
        int tries = 0;

        do
        {
            // small delay, because the text is not always directly available after the copy key command
            await Task.Delay(200, cancellationToken);

            itemString = await this.clipboardHelper.GetTextAsync();

            tries++;
        }
        while (itemString == clipBoardTemp && tries < maxTries);

        if (string.IsNullOrEmpty(clipBoardTemp))
        {
            await this.clipboardHelper.ClearAsync();
        }
        else
        {
            await this.clipboardHelper.SetTextAsync(clipBoardTemp);
        }

        return itemString ?? string.Empty;
    }
}