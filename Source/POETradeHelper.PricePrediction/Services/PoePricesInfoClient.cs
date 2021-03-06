﻿using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using POETradeHelper.Common;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.PricePrediction.Models;

namespace POETradeHelper.PricePrediction.Services
{
    public class PoePricesInfoClient : IPoePricesInfoClient
    {
        private readonly IHttpClientWrapper httpClient;
        private readonly IJsonSerializerWrapper jsonSerializer;
        private readonly ILogger<PoePricesInfoClient> logger;

        public PoePricesInfoClient(IHttpClientFactoryWrapper httpClientFactory, IJsonSerializerWrapper jsonSerializer, ILogger<PoePricesInfoClient> logger)
        {
            this.httpClient = httpClientFactory.CreateClient();
            this.jsonSerializer = jsonSerializer;
            this.logger = logger;
        }

        public async Task<PoePricesInfoItem> GetPricePredictionAsync(string league, string itemText, CancellationToken cancellationToken = default)
        {
            PoePricesInfoItem result = null;

            if (string.IsNullOrEmpty(league) || string.IsNullOrEmpty(itemText))
            {
                return result;
            }

            try
            {
                var base64ItemText = Convert.ToBase64String(Encoding.UTF8.GetBytes(itemText));

                var url = $"https://www.poeprices.info/api?i={base64ItemText}&l={league}";

                System.Net.Http.HttpResponseMessage httpResponse = await this.httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);

                if (httpResponse.IsSuccessStatusCode)
                {
                    string jsonResult = await httpResponse.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                    result = this.jsonSerializer.Deserialize<PoePricesInfoItem>(jsonResult, new JsonSerializerOptions { PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy() });
                }
                else
                {
                    this.logger.LogError("Poeprices.info api returned non success status code for league {@league} and item text {@itemText}. Response: {@response}", league, itemText, httpResponse);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "Failed to get price prediction in league {@league} with item text {@itemText}.", league, itemText);
            }

            return result;
        }
    }
}