using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using POETradeHelper.Common.Contract;
using Polly;

namespace POETradeHelper.PathOfExileTradeApi
{
    [ExcludeFromCodeCoverage]
    internal class PathOfExileTradeApiModule : IModule
    {
        public void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddHttpClient(Constants.HttpClientNames.PoeTradeApiDataClient, ConfigurePoeTradeApiHttpClient)
                .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromSeconds(retryAttempt * 15)));

            serviceCollection.AddHttpClient(Constants.HttpClientNames.PoeTradeApiItemSearchClient, ConfigurePoeTradeApiHttpClient);
        }

        private static void ConfigurePoeTradeApiHttpClient(HttpClient client)
        {
            client.BaseAddress = new Uri(Properties.Resources.PoeTradeApiBaseUrl);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("POETradeHelper");
        }
    }
}