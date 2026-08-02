using System;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Threading;

using AwesomeAssertions;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using NUnit.Framework;

using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Contract.Commands;
using POETradeHelper.Common.UI;
using POETradeHelper.ItemSearch.UI.Avalonia.Controllers;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels.Abstractions;
using POETradeHelper.ItemSearch.UI.Avalonia.Views;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Tests.Controllers
{
    public class ItemSearchResultOverlayControllerTests
    {
        private readonly IItemSearchResultOverlayView viewMock;
        private readonly IItemSearchResultOverlayViewModel viewModelMock;
        private readonly IUiThreadDispatcher uiThreadDispatcherMock;
        private readonly ItemSearchResultOverlayController sut;

        public ItemSearchResultOverlayControllerTests()
        {
            this.viewMock = Substitute.For<IItemSearchResultOverlayView>();
            this.viewModelMock = Substitute.For<IItemSearchResultOverlayViewModel>();
            var viewLocatorMock = Substitute.For<IViewLocator>();
            viewLocatorMock.GetView(Arg.Any<IItemSearchResultOverlayViewModel>())
                .Returns(this.viewMock);
            this.uiThreadDispatcherMock = Substitute.For<IUiThreadDispatcher>();
            this.sut = new ItemSearchResultOverlayController(this.viewModelMock, viewLocatorMock, this.uiThreadDispatcherMock);
        }

        [Test]
        public async Task ExecuteHideOverlayCommand_ShouldCallHideOnOverlayIfOverlayIsVisible()
        {
            this.viewMock.IsVisible.Returns(true);

            await this.ExecuteHideOverlayCommand(new HideOverlayCommand());

            this.viewMock
                .Received()
                .Hide();
        }

        [Test]
        public async Task OverlayStatusProviderIsVisible_ShouldReturnTrue_AfterShowingOverlay()
        {
            ((IOverlayStatusProvider)this.sut).IsVisible.Should().BeFalse();

            await this.ExecuteSearchItemCommand(new SearchItemCommand());

            ((IOverlayStatusProvider)this.sut).IsVisible.Should().BeTrue();
        }

        [Test]
        public async Task OverlayStatusProviderIsVisible_ShouldReturnFalse_AfterHidingOverlay()
        {
            // arrange
            this.viewMock.IsVisible.Returns(true);
            await this.ExecuteSearchItemCommand(new SearchItemCommand());
            ((IOverlayStatusProvider)this.sut).IsVisible.Should().BeTrue();

            // act
            await this.ExecuteHideOverlayCommand(new HideOverlayCommand());

            // assert
            ((IOverlayStatusProvider)this.sut).IsVisible.Should().BeFalse();
        }

        [Test]
        public async Task HandleSearchItemQuery_ShouldCallSetListingForItemUnderCursorAsyncOnViewModel()
        {
            await this.ExecuteSearchItemCommand(new SearchItemCommand());

            await this.viewModelMock
                .Received()
                .SetListingForItemUnderCursorAsync(Arg.Is<CancellationToken>(c => c != CancellationToken.None));
        }

        [Test]
        public async Task HandleSearchItemQuery_ShouldOpenOverlay()
        {
            await this.ExecuteSearchItemCommand(new SearchItemCommand());

            this.viewMock
                .Received()
                .Show();
        }

        [Test]
        public async Task HandleSearchItemQuery_ShouldCatchOperationCancelledException()
        {
            this.viewModelMock
                .SetListingForItemUnderCursorAsync(Arg.Any<CancellationToken>())
                .Throws<OperationCanceledException>();

            Func<Task> action = () => this.ExecuteSearchItemCommand(new SearchItemCommand());

            await action.Should().NotThrowAsync();
        }

        private async Task ExecuteHideOverlayCommand(HideOverlayCommand command)
        {
            Action? action = null;
            this.uiThreadDispatcherMock
                .When(x => x.InvokeAsync(Arg.Any<Action>(), Arg.Any<DispatcherPriority>()))
                .Do(ctx => action = ctx.Arg<Action>());

            await this.sut.Handle(command, default);
            action!();
        }

        private async Task ExecuteSearchItemCommand(SearchItemCommand command)
        {
            Func<Task>? action = null;
            this.uiThreadDispatcherMock
                .When(x => x.InvokeAsync(Arg.Any<Func<Task>>(), Arg.Any<DispatcherPriority>()))
                .Do(ctx => action = ctx.Arg<Func<Task>>());

            await this.sut.Handle(command, default);
            await action!();
        }
    }
}