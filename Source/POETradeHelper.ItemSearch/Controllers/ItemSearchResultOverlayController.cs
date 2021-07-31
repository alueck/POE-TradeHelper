using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using MediatR;
using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Contract.Attributes;
using POETradeHelper.Common.Contract.Commands;
using POETradeHelper.Common.Contract.Queries;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.ItemSearch.Views;

namespace POETradeHelper.ItemSearch.Controllers
{
    [Singleton]
    public class ItemSearchResultOverlayController : IRequestHandler<SearchItemCommand>, IRequestHandler<HideOverlayQuery, HideOverlayResponse>
    {
        private readonly IItemSearchResultOverlayViewModel itemSearchResultOverlayViewModel;
        private readonly IViewLocator viewLocator;

        private CancellationTokenSource searchItemCancellationTokenSource = new CancellationTokenSource();

        public ItemSearchResultOverlayController(
            IItemSearchResultOverlayViewModel itemSearchResultOverlayViewModel,
            IViewLocator viewLocator)
        {
            this.itemSearchResultOverlayViewModel = itemSearchResultOverlayViewModel;
            this.viewLocator = viewLocator;
        }

        private IItemSearchResultOverlayView view;
        private IItemSearchResultOverlayView View => LazyInitializer.EnsureInitialized(ref view, CreateView);

        private IItemSearchResultOverlayView CreateView()
        {
            var view = viewLocator.GetView(itemSearchResultOverlayViewModel);
            if (view is IItemSearchResultOverlayView itemSearchResultOverlay)
            {
                if (view is IControl control)
                {
                    control.DataContext = itemSearchResultOverlayViewModel;
                }

                return itemSearchResultOverlay;
            }

            throw new ArgumentException($"Could not find view for {nameof(IItemSearchResultOverlayViewModel)} that implements {nameof(IItemSearchResultOverlayView)}");
        }

        private void CancelSearchItemToken()
        {
            lock (this)
            {
                this.searchItemCancellationTokenSource.Cancel();
                this.searchItemCancellationTokenSource.Dispose();
                this.searchItemCancellationTokenSource = new CancellationTokenSource();
            }
        }

        public async Task<Unit> Handle(SearchItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                this.CancelSearchItemToken();

                this.View.Show();

                await this.itemSearchResultOverlayViewModel
                    .SetListingForItemUnderCursorAsync(this.searchItemCancellationTokenSource.Token)
                    .ConfigureAwait(true);
            }
            catch (Exception exception) when (exception is OperationCanceledException or TaskCanceledException)
            {
            }

            return Unit.Value;
        }

        public Task<HideOverlayResponse> Handle(HideOverlayQuery request, CancellationToken cancellationToken)
        {
            var handled = false;
            if (View.IsVisible)
            {
                this.CancelSearchItemToken();
                View.Hide();
                handled = true;
            }

            return Task.FromResult(new HideOverlayResponse(handled));
        }
    }
}