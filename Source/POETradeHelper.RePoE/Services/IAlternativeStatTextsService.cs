using POETradeHelper.RePoE.Models;

namespace POETradeHelper.RePoE.Services;

public interface IAlternativeStatTextsService
{
    IAsyncEnumerable<StatTextsGroup> GetAlternativeStatTexts(CancellationToken cancellationToken = default);
}