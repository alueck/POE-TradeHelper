using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using MediatR;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Contract.Commands;

using SharpHook;
using SharpHook.Native;
using SharpHook.Reactive;

namespace POETradeHelper.Common.Tests
{
    public class UserInputEventProviderTests : IDisposable
    {
        private readonly Subject<KeyboardHookEventArgs> keyPressed;
        private readonly IReactiveGlobalHook hookMock;
        private readonly IPathOfExileProcessHelper pathOfExileProcessHelperMock;
        private readonly IMediator mediatorMock;
        private readonly IUserInputEventProvider userInputEventProvider;

        public UserInputEventProviderTests()
        {
            this.keyPressed = new Subject<KeyboardHookEventArgs>();
            this.hookMock = Substitute.For<IReactiveGlobalHook>();
            this.hookMock
                .KeyPressed
                .Returns(this.keyPressed);
            this.pathOfExileProcessHelperMock = Substitute.For<IPathOfExileProcessHelper>();
            this.mediatorMock = Substitute.For<IMediator>();
            this.userInputEventProvider = new UserInputEventProvider(
                this.hookMock,
                this.pathOfExileProcessHelperMock,
                this.mediatorMock);
        }

        [SetUp]
        public async Task SetUp()
        {
            await this.userInputEventProvider.OnInitAsync();
        }

        public void Dispose()
        {
            this.keyPressed.Dispose();
            this.hookMock.Dispose();
            this.userInputEventProvider.Dispose();
        }

        [Test]
        public async Task SearchItemKeyCombinationShouldSendSearchItemCommandIfPathOfExileIsActiveWindow()
        {
            KeyboardHookEventArgs keyEventArgs = new(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcD },
                Mask = ModifierMask.Ctrl,
                Type = EventType.KeyPressed,
            });
            this.pathOfExileProcessHelperMock.IsPathOfExileActiveWindow()
                .Returns(true);

            this.keyPressed.OnNext(keyEventArgs);

            await this.mediatorMock
                .Received()
                .Send(Arg.Any<SearchItemCommand>(), Arg.Any<CancellationToken>());
            keyEventArgs.SuppressEvent.Should().BeTrue();
        }

        [Test]
        public async Task SearchItemKeyCombinationShouldNotSendSearchItemCommandIfPathOfExileIsNotActiveWindow()
        {
            KeyboardHookEventArgs keyEventArgs = new(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcD },
                Mask = ModifierMask.Ctrl,
                Type = EventType.KeyPressed,
            });

            this.keyPressed.OnNext(keyEventArgs);

            await this.mediatorMock
                .DidNotReceive()
                .Send(Arg.Any<SearchItemCommand>(), Arg.Any<CancellationToken>());
            keyEventArgs.SuppressEvent.Should().BeFalse();
        }

        [Test]
        public async Task HideOverlayKeyCombinationShouldSendHideOverlayQuery()
        {
            KeyboardHookEventArgs keyEventArgs = new(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcEscape },
                Type = EventType.KeyPressed,
            });

            this.keyPressed.OnNext(keyEventArgs);

            await this.mediatorMock
                .Received()
                .Send(Arg.Any<HideOverlayCommand>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public void ShouldSetEventArgsHandledFromHideOverlayQueryWithOnHandledAction()
        {
            KeyboardHookEventArgs keyEventArgs = new(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcEscape },
                Type = EventType.KeyPressed,
            });

            Action? onHandledAction = null;
            this.mediatorMock
                .When(x => x.Send(Arg.Any<HideOverlayCommand>(), Arg.Any<CancellationToken>()))
                .Do(ctx => onHandledAction = ctx.Arg<HideOverlayCommand>().OnHandled);

            this.keyPressed.OnNext(keyEventArgs);

            onHandledAction.Should().NotBeNull();
            keyEventArgs.SuppressEvent.Should().BeFalse();
            onHandledAction!.Invoke();
            keyEventArgs.SuppressEvent.Should().BeTrue();
        }

        [Test]
        public async Task GotoHideoutKeyCombinationShouldSendGotoHideoutCommandIfPathOfExileIsActiveWindow()
        {
            KeyboardHookEventArgs keyEventArgs = new(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcF5 },
                Type = EventType.KeyPressed,
            });
            this.pathOfExileProcessHelperMock.IsPathOfExileActiveWindow()
                .Returns(true);

            this.keyPressed.OnNext(keyEventArgs);

            await this.mediatorMock
                .Received()
                .Send(Arg.Any<GotoHideoutCommand>(), Arg.Any<CancellationToken>());
            keyEventArgs.SuppressEvent.Should().BeTrue();
        }

        [Test]
        public async Task GotoHideoutKeyCombinationShouldNotSendGotoHideoutCommandIfPathOfExileIsNotActiveWindow()
        {
            KeyboardHookEventArgs keyEventArgs = new(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcF5 },
                Type = EventType.KeyPressed,
            });

            this.keyPressed.OnNext(keyEventArgs);

            await this.mediatorMock
                .DidNotReceive()
                .Send(Arg.Any<GotoHideoutCommand>(), Arg.Any<CancellationToken>());
            keyEventArgs.SuppressEvent.Should().BeFalse();
        }

        [Test]
        public async Task OpenWikiKeyCombinationShouldSendOpenWikiCommandIfPathOfExileIsActiveWindow()
        {
            KeyboardHookEventArgs keyEventArgs = new(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcW },
                Mask = ModifierMask.Alt,
                Type = EventType.KeyPressed,
            });
            this.pathOfExileProcessHelperMock.IsPathOfExileActiveWindow()
                .Returns(true);

            this.keyPressed.OnNext(keyEventArgs);

            await this.mediatorMock
                .Received()
                .Send(Arg.Any<OpenWikiCommand>(), Arg.Any<CancellationToken>());
            keyEventArgs.SuppressEvent.Should().BeTrue();
        }

        [Test]
        public async Task OpenWikiKeyCombinationShouldNotSendOpenWikiCommandIfPathOfExileIsNotActiveWindow()
        {
            KeyboardHookEventArgs keyEventArgs = new(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcW },
                Mask = ModifierMask.Alt,
                Type = EventType.KeyPressed,
            });

            this.keyPressed.OnNext(keyEventArgs);

            await this.mediatorMock
                .DidNotReceive()
                .Send(Arg.Any<OpenWikiCommand>(), Arg.Any<CancellationToken>());
            keyEventArgs.SuppressEvent.Should().BeFalse();
        }
    }
}