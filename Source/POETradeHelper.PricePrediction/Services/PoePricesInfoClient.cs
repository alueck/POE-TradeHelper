using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Logging;

using POETradeHelper.Common.Wrappers;
using POETradeHelper.PricePrediction.Models;

namespace POETradeHelper.PricePrediction.Services
{
    public class PoePricesInfoClient : IPoePricesInfoClient
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
        };

        private readonly IHttpClientWrapper httpClient;
        private readonly IJsonSerializerWrapper jsonSerializer;
        private readonly ILogger<PoePricesInfoClient> logger;

        public PoePricesInfoClient(IHttpClientFactoryWrapper httpClientFactory, IJsonSerializerWrapper jsonSerializer, ILogger<PoePricesInfoClient> logger)
        {
            this.httpClient = httpClientFactory.CreateClient();
            this.jsonSerializer = jsonSerializer;
            this.logger = logger;
        }

        public async Task<PoePricesInfoPrediction?> GetPricePredictionAsync(string league, string itemText, CancellationToken cancellationToken = default)
        {
            PoePricesInfoPrediction? result = null;

            if (string.IsNullOrEmpty(league) || string.IsNullOrEmpty(itemText))
            {
                return result;
            }

            try
            {
                var url = GetUrl(league, itemText);
                HttpResponseMessage httpResponse = await this.httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);

                if (httpResponse.IsSuccessStatusCode)
                {
                    string jsonResult = await httpResponse.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                    result = this.jsonSerializer.Deserialize<PoePricesInfoPrediction>(jsonResult, JsonSerializerOptions);
                }
                else
                {
                    this.logger.LogError("Poeprices.info api returned non success status code for league {@league} and item text {@itemText}. Response: {@response}", league, itemText, httpResponse);
                }
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                this.logger.LogError(exception, "Failed to get price prediction in league {@league} with item text {@itemText}.", league, itemText);
            }

            return result;
        }

        private static string GetUrl(string league, string itemText)
        {
            var base64ItemText = Convert.ToBase64String(Encoding.UTF8.GetBytes(itemText));

            return $"https://www.poeprices.info/api?i={base64ItemText}&l={league}";
        }
    }
}