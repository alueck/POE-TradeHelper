using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;

using POETradeHelper.RePoE.Models;

namespace POETradeHelper.RePoE.Services;

internal sealed partial class AlternativeStatTextsService : IAlternativeStatTextsService
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
    };

    private readonly HttpClient httpClient;

    public AlternativeStatTextsService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async IAsyncEnumerable<StatTextsGroup> GetAlternativeStatTexts([EnumeratorCancellation]CancellationToken cancellationToken = default)
    {
        var result = this.httpClient.GetFromJsonAsAsyncEnumerable<Data>("stat_translations.min.json", JsonSerializerOptions, cancellationToken);

        await foreach (var data in result)
        {
            if (data!.English.Count > 1 && data.TradeStats != null)
            {
                foreach (var tradeStat in data.TradeStats)
                {
                    yield return new StatTextsGroup(tradeStat.Id, data.English.Select(text => string.Format(text.String, text.NormalizedFormat)).ToArray());
                }
            }
        }
    }

    [GeneratedRegex(@"[\+\-]")]
    private static partial Regex NumberSignRegex();

    private sealed record Data(IReadOnlyCollection<Translation> English, IReadOnlyList<TradeStat>? TradeStats);

    private sealed record Translation(string String, string[] Format)
    {
        public string[] NormalizedFormat => this.Format.Select(x => NumberSignRegex().Replace(x, "")).ToArray();
    }

    private sealed record TradeStat(string Id, string Type);
}