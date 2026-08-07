using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Mediator;

using POETradeHelper.Common.UI.Models;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Exceptions;
using POETradeHelper.ItemSearch.Queries;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels.Abstractions;

using ReactiveUI;
using ReactiveUI.Primitives;
using ReactiveUI.SourceGenerators;

using Splat;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels
{
    public partial class ItemSearchResultOverlayViewModel : ReactiveObject, IItemSearchResultOverlayViewModel
    {
        private readonly IMediator mediator;
        private readonly Func<IItemSearchResultOverlayViewModel, IItemResultsViewModel> itemResultsViewModelFactory;
        private readonly Func<IItemSearchResultOverlayViewModel, IExchangeResultsViewModel> exchangeResultsViewModelFactory;

        public ItemSearchResultOverlayViewModel(
            IMediator mediator,
            Func<IItemSearchResultOverlayViewModel, IItemResultsViewModel> itemResultsViewModelFactory,
            Func<IItemSearchResultOverlayViewModel, IExchangeResultsViewModel> exchangeResultsViewModelFactory)
        {
            this.mediator = mediator;
            this.itemResultsViewModelFactory = itemResultsViewModelFactory;
            this.exchangeResultsViewModelFactory = exchangeResultsViewModelFactory;
            this.Router = new RoutingState();
        }

        public RoutingState Router { get; }

        [Reactive]
        public partial Message? Message { get; private set; }

        [Reactive]
        public partial bool IsBusy { get; private set; }

        [Reactive]
        public partial Item? Item { get; private set; }

        [Reactive]
        public partial Uri? Url { get; private set; }

        [Reactive]
        public partial IResultsViewModel? ResultsViewModel { get; private set; }

        public async Task SetListingForItemUnderCursorAsync(CancellationToken token = default)
        {
            try
            {
                this.IsBusy = true;
                this.Message = null;

                this.Item = await this.mediator.Send(new GetItemFromCursorQuery(), token).ConfigureAwait(true);

                if (this.Item is CurrencyItem or FragmentItem)
                {
                    await this.GoToView(this.exchangeResultsViewModelFactory, token);
                }
                else
                {
                    await this.GoToView(this.itemResultsViewModelFactory, token);
                }
            }
            catch (Exception exception)
            {
                if (exception is not OperationCanceledException and not TaskCanceledException)
                {
                    this.HandleException(exception, "Error occurred getting listings for item under cursor.");
                }
            }
            finally
            {
                if (!token.IsCancellationRequested)
                {
                    this.IsBusy = false;
                }
            }
        }

        public void HandleException(Exception exception, string logMessage)
        {
            this.Message = new Message
            {
                Text = exception is InvalidItemStringException
                    ? "Invalid item string"
                    : $"An error occurred. Please try again.{Environment.NewLine}If the error persists, check the logs and create an issue on GitHub.",
                Type = MessageType.Error,
            };

            this.Log().Error(exception, logMessage);
        }

        private async Task GoToView<T>(Func<IItemSearchResultOverlayViewModel, T> factory, CancellationToken cancellationToken)
            where T : IResultsViewModel
        {
            T viewModel;
            if (this.Router.NavigationStack.FirstOrDefault() is T existingViewModel)
            {
                viewModel = existingViewModel;
            }
            else
            {
                viewModel = factory(this);
                using var subscription = this.Router.NavigateAndReset.Execute(viewModel).Subscribe();
            }

            await viewModel.InitializeAsync(this.Item, cancellationToken);
            this.ResultsViewModel = viewModel;
        }
    }
}