using System;
using System.ComponentModel;
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

using ReactiveUI.Builder;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Tests.Controllers
{
    public class ItemSearchResultOverlayControllerTests : IDisposable
    {
        private readonly IReactiveUIBuilder builder;
        private readonly IItemSearchResultOverlayView viewMock;
        private readonly IItemSearchResultOverlayViewModel viewModelMock;
        private readonly ItemSearchResultOverlayController sut;

        public ItemSearchResultOverlayControllerTests()
        {
            this.builder = RxAppBuilder.CreateReactiveUIBuilder()
                .WithCoreServices()
                .UseCurrentSplatLocator()
                .BuildApp();

            this.viewMock = Substitute.For<IItemSearchResultOverlayView>();
            this.viewModelMock = Substitute.For<IItemSearchResultOverlayViewModel>();
            var viewLocatorMock = Substitute.For<IViewLocator>();
            viewLocatorMock.GetView(Arg.Any<IItemSearchResultOverlayViewModel>())
                .Returns(this.viewMock);
            this.sut = new ItemSearchResultOverlayController(this.viewModelMock, viewLocatorMock, new TestUiThreadDispatcher());
        }

        public void Dispose() => this.sut.Dispose();

        [Test]
        public async Task ExecuteHideOverlayCommand_ShouldCallHideOnOverlayIfOverlayIsVisible()
        {
            this.viewMock.IsVisible.Returns(true);

            await this.sut.Handle(new HideOverlayCommand(), default);

            this.viewMock
                .Received()
                .Hide();
        }

        [Test]
        public async Task OverlayStatusProviderIsVisible_ShouldReturnTrue_IfViewIsVisibleChangesToTrue()
        {
            // arrange
            // trigger view initialization
            await this.sut.Handle(new SearchItemCommand(), default);

            ((IOverlayStatusProvider)this.sut).IsVisible.Should().BeFalse();
            this.viewMock.IsVisible.Returns(true);

            // act
            this.viewMock.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(this, new PropertyChangedEventArgs(nameof(IItemSearchResultOverlayView.IsVisible)));

            // assert
            ((IOverlayStatusProvider)this.sut).IsVisible.Should().BeTrue();
        }

        [Test]
        public async Task OverlayStatusProviderIsVisible_ShouldReturnFalse_IfViewIsVisibleChangesToFalse()
        {
            // arrange
            this.viewMock.IsVisible.Returns(true);

            // trigger view initialization
            await this.sut.Handle(new SearchItemCommand(), default);

            ((IOverlayStatusProvider)this.sut).IsVisible.Should().BeTrue();
            this.viewMock.IsVisible.Returns(false);

            // act
            this.viewMock.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(this, new PropertyChangedEventArgs(nameof(IItemSearchResultOverlayView.IsVisible)));

            // assert
            ((IOverlayStatusProvider)this.sut).IsVisible.Should().BeFalse();
        }

        [Test]
        public async Task HandleSearchItemQuery_ShouldCallSetListingForItemUnderCursorAsyncOnViewModel()
        {
            await this.sut.Handle(new SearchItemCommand(), default);

            await this.viewModelMock
                .Received()
                .SetListingForItemUnderCursorAsync(Arg.Is<CancellationToken>(c => c != CancellationToken.None));
        }

        [Test]
        public async Task HandleSearchItemQuery_ShouldOpenOverlay()
        {
            await this.sut.Handle(new SearchItemCommand(), default);

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

            Func<Task> action = async () => await this.sut.Handle(new SearchItemCommand(), default);

            await action.Should().NotThrowAsync();
        }

        private sealed class TestUiThreadDispatcher : IUiThreadDispatcher
        {
            public bool CheckAccess() => true;

            public void VerifyAccess()
            {
            }

            public void Post(Action action, DispatcherPriority priority = new DispatcherPriority()) => action();

            public Task InvokeAsync(Action action, DispatcherPriority priority = default)
            {
                action();
                return Task.CompletedTask;
            }

            public Task InvokeAsync(Func<Task> function, DispatcherPriority priority = default) => function();

            public Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> function, DispatcherPriority priority = default) => function();
        }
    }
}