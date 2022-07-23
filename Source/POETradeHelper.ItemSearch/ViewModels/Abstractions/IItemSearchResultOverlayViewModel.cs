using System;
using System.Threading;
using System.Threading.Tasks;

using ReactiveUI;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public interface IItemSearchResultOverlayViewModel : IScreen
    {
        Task SetListingForItemUnderCursorAsync(CancellationToken token = default);

        void HandleException(Exception exception);
    }
}