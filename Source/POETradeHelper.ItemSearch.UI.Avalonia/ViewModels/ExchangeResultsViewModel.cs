using System.Threading;
using System.Threading.Tasks;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Mappers;
using POETradeHelper.ItemSearch.UI.Avalonia.Factories;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels.Abstractions;
using POETradeHelper.PathOfExileTradeApi.Services;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;

public class ExchangeResultsViewModel : ReactiveObject, IExchangeResultsViewModel
{
    private readonly IPoeTradeApiClient poeTradeApiClient;
    private readonly IItemToExchangeQueryRequestMapper itemToExchangeQueryRequestMapper;
    private readonly IItemListingsViewModelFactory itemListingsViewModelFactory;

    public ExchangeResultsViewModel(
        IScreen screen,
        IPoeTradeApiClient poeTradeApiClient,
        IItemToExchangeQueryRequestMapper itemToExchangeQueryRequestMapper,
        IItemListingsViewModelFactory itemListingsViewModelFactory)
    {
        this.poeTradeApiClient = poeTradeApiClient;
        this.itemToExchangeQueryRequestMapper = itemToExchangeQueryRequestMapper;
        this.itemListingsViewModelFactory = itemListingsViewModelFactory;
        this.HostScreen = screen;
    }

    public string UrlPathSegment => "exchange_results";

    public IScreen HostScreen { get; }

    [Reactive]
    public ItemListingsViewModel? ItemListings { get; private set; }

    public async Task InitializeAsync(Item? item, CancellationToken cancellationToken)
    {
        if (item != null)
        {
            var request = this.itemToExchangeQueryRequestMapper.MapToQueryRequest(item);
            var exchangeQueryResult = await this.poeTradeApiClient.GetListingsAsync(request, cancellationToken);
            this.ItemListings = await this.itemListingsViewModelFactory.CreateAsync(exchangeQueryResult, cancellationToken);
        }
    }
}
