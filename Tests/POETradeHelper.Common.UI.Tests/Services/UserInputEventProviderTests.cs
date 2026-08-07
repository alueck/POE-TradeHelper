using System;
using System.Threading;
using System.Threading.Tasks;

using AwesomeAssertions;

using Mediator;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Contract.Commands;
using POETradeHelper.Common.UI.Services;

using SharpHook;
using SharpHook.Data;

namespace POETradeHelper.Common.UI.Tests.Services
{
    public class UserInputEventProviderTests : IDisposable
    {
        private readonly IGlobalHook hookMock;
        private readonly IPathOfExileProcessHelper pathOfExileProcessHelperMock;
        private readonly IMediator mediatorMock;
        private readonly IOverlayStatusProvider overlayStatusProviderMock;
        private readonly IUserInputEventProvider sut;

        public UserInputEventProviderTests()
        {
            this.hookMock = Substitute.For<IGlobalHook>();
            this.pathOfExileProcessHelperMock = Substitute.For<IPathOfExileProcessHelper>();
            this.mediatorMock = Substitute.For<IMediator>();
            this.overlayStatusProviderMock = Substitute.For<IOverlayStatusProvider>();
            this.sut = new UserInputEventProvider(
                this.hookMock,
                this.pathOfExileProcessHelperMock,
                this.mediatorMock,
                this.overlayStatusProviderMock);
        }

        [SetUp]
        public async Task SetUp()
        {
            await this.sut.OnInitAsync();
        }

        public void Dispose()
        {
            this.hookMock.Dispose();
            this.sut.Dispose();
        }

        [Test]
        public async Task SearchItemKeyCombination_ShouldSendSearchItemCommand_IfPathOfExileIsActiveWindow()
        {
            KeyboardHookEventArgs keyEventArgs = new(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcD },
                Mask = EventMask.Alt,
                Type = EventType.KeyPressed,
            });
            this.pathOfExileProcessHelperMock.IsPathOfExileActiveWindow()
                .Returns(true);

            this.hookMock.KeyPressed += Raise.EventWith(keyEventArgs);

            await this.mediatorMock
                .Received()
                .Send(Arg.Any<SearchItemCommand>(), Arg.Any<CancellationToken>());
            keyEventArgs.SuppressEvent.Should().BeTrue();
        }

        [Test]
        public async Task SearchItemKeyCombination_ShouldNotSendSearchItemCommand_IfPathOfExileIsNotActiveWindow()
        {
            KeyboardHookEventArgs keyEventArgs = new(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcD },
                Mask = EventMask.Ctrl,
                Type = EventType.KeyPressed,
            });

            this.hookMock.KeyPressed += Raise.EventWith(keyEventArgs);

            await this.mediatorMock
                .DidNotReceive()
                .Send(Arg.Any<SearchItemCommand>(), Arg.Any<CancellationToken>());
            keyEventArgs.SuppressEvent.Should().BeFalse();
        }

        [Test]
        public async Task HideOverlayKeyCombination_ShouldSendHideOverlayQuery()
        {
            KeyboardHookEventArgs keyEventArgs = new(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcEscape },
                Type = EventType.KeyPressed,
            });

            this.hookMock.KeyPressed += Raise.EventWith(keyEventArgs);

            await this.mediatorMock
                .Received()
                .Send(Arg.Any<HideOverlayCommand>(), Arg.Any<CancellationToken>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void HideOverlay_ShouldSetEventArgsHandled_BasedOnOverlayVisibility(bool overlayVisible)
        {
            KeyboardHookEventArgs keyEventArgs = new(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcEscape },
                Type = EventType.KeyPressed,
            });
            this.overlayStatusProviderMock.IsVisible.Returns(overlayVisible);

            this.hookMock.KeyPressed += Raise.EventWith(keyEventArgs);

            keyEventArgs.SuppressEvent.Should().Be(overlayVisible);
        }

        [Test]
        public async Task GotoHideoutKeyCombination_ShouldSendGotoHideoutCommand_IfPathOfExileIsActiveWindow()
        {
            KeyboardHookEventArgs keyEventArgs = new(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcF5 },
                Type = EventType.KeyPressed,
            });
            this.pathOfExileProcessHelperMock.IsPathOfExileActiveWindow()
                .Returns(true);

            this.hookMock.KeyPressed += Raise.EventWith(keyEventArgs);

            await this.mediatorMock
                .Received()
                .Send(Arg.Any<GotoHideoutCommand>(), Arg.Any<CancellationToken>());
            keyEventArgs.SuppressEvent.Should().BeTrue();
        }

        [Test]
        public async Task GotoHideoutKeyCombination_ShouldNotSendGotoHideoutCommand_IfPathOfExileIsNotActiveWindow()
        {
            KeyboardHookEventArgs keyEventArgs = new(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcF5 },
                Type = EventType.KeyPressed,
            });

            this.hookMock.KeyPressed += Raise.EventWith(keyEventArgs);

            await this.mediatorMock
                .DidNotReceive()
                .Send(Arg.Any<GotoHideoutCommand>(), Arg.Any<CancellationToken>());
            keyEventArgs.SuppressEvent.Should().BeFalse();
        }

        [Test]
        public async Task OpenWikiKeyCombination_ShouldSendOpenWikiCommand_IfPathOfExileIsActiveWindow()
        {
            KeyboardHookEventArgs keyEventArgs = new(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcW },
                Mask = EventMask.Alt,
                Type = EventType.KeyPressed,
            });
            this.pathOfExileProcessHelperMock.IsPathOfExileActiveWindow()
                .Returns(true);

            this.hookMock.KeyPressed += Raise.EventWith(keyEventArgs);

            await this.mediatorMock
                .Received()
                .Send(Arg.Any<OpenWikiCommand>(), Arg.Any<CancellationToken>());
            keyEventArgs.SuppressEvent.Should().BeTrue();
        }

        [Test]
        public async Task OpenWikiKeyCombination_ShouldNotSendOpenWikiCommand_IfPathOfExileIsNotActiveWindow()
        {
            KeyboardHookEventArgs keyEventArgs = new(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcW },
                Mask = EventMask.Alt,
                Type = EventType.KeyPressed,
            });

            this.hookMock.KeyPressed += Raise.EventWith(keyEventArgs);

            await this.mediatorMock
                .DidNotReceive()
                .Send(Arg.Any<OpenWikiCommand>(), Arg.Any<CancellationToken>());
            keyEventArgs.SuppressEvent.Should().BeFalse();
        }
    }
}