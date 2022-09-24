using System.Threading;
using System.Threading.Tasks;

using POETradeHelper.ItemSearch.Contract.Models;

using ReactiveUI;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels.Abstractions;

public interface IResultsViewModel : IRoutableViewModel
{
    ItemListingsViewModel? ItemListings { get; }

    Task InitializeAsync(Item? item, CancellationToken cancellationToken);
}
