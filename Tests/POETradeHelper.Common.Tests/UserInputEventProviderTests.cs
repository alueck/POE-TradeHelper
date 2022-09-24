using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using MediatR;

using Moq;

using NUnit.Framework;

using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Contract.Commands;

using SharpHook;
using SharpHook.Native;
using SharpHook.Reactive;

namespace POETradeHelper.Common.Tests
{
    public class UserInputEventProviderTests
    {
        private Subject<KeyboardHookEventArgs> keyPressed;
        private Mock<IReactiveGlobalHook> hookMock;
        private Mock<IPathOfExileProcessHelper> pathOfExileProcessHelperMock;
        private Mock<IMediator> mediatorMock;
        private IUserInputEventProvider userInputEventProvider;

        [SetUp]
        public async Task Setup()
        {
            this.keyPressed = new Subject<KeyboardHookEventArgs>();
            this.hookMock = new Mock<IReactiveGlobalHook>();
            this.hookMock
                .Setup(x => x.KeyPressed)
                .Returns(this.keyPressed);
            this.pathOfExileProcessHelperMock = new Mock<IPathOfExileProcessHelper>();
            this.mediatorMock = new Mock<IMediator>();
            this.userInputEventProvider = new UserInputEventProvider(
                this.hookMock.Object,
                this.pathOfExileProcessHelperMock.Object,
                this.mediatorMock.Object);
            await this.userInputEventProvider.OnInitAsync();
        }

        [Test]
        public void SearchItemKeyCombinationShouldSendSearchItemCommandIfPathOfExileIsActiveWindow()
        {
            var keyEventArgs = new KeyboardHookEventArgs(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcD },
                Mask = ModifierMask.Ctrl,
                Type = EventType.KeyPressed
            });
            this.pathOfExileProcessHelperMock.Setup(x => x.IsPathOfExileActiveWindow())
                .Returns(true);

            this.keyPressed.OnNext(keyEventArgs);

            this.mediatorMock.Verify(x => x.Send(It.IsAny<SearchItemCommand>(), It.IsAny<CancellationToken>()));
            keyEventArgs.Reserved.Should().Be(EventReservedValueMask.SuppressEvent);
        }

        [Test]
        public void SearchItemKeyCombinationShouldNotSendSearchItemCommandIfPathOfExileIsNotActiveWindow()
        {
            var keyEventArgs = new KeyboardHookEventArgs(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcD },
                Mask = ModifierMask.Ctrl,
                Type = EventType.KeyPressed
            });

            this.keyPressed.OnNext(keyEventArgs);

            this.mediatorMock.Verify(x => x.Send(It.IsAny<SearchItemCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            keyEventArgs.Reserved.Should().BeNull();
        }

        [Test]
        public void HideOverlayKeyCombinationShouldSendHideOverlayQuery()
        {
            var keyEventArgs = new KeyboardHookEventArgs(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcEscape },
                Type = EventType.KeyPressed
            });

            this.keyPressed.OnNext(keyEventArgs);

            this.mediatorMock.Verify(x => x.Send(It.IsAny<HideOverlayCommand>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public void ShouldSetEventArgsHandledFromHideOverlayQueryWithOnHandledAction()
        {
            var keyEventArgs = new KeyboardHookEventArgs(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcEscape },
                Type = EventType.KeyPressed
            });

            Action onHandledAction = null;
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<HideOverlayCommand>(), It.IsAny<CancellationToken>()))
                .Callback((IRequest<Unit> command, CancellationToken _) => onHandledAction = ((HideOverlayCommand)command).OnHandled);


            this.keyPressed.OnNext(keyEventArgs);

            onHandledAction.Should().NotBeNull();
            keyEventArgs.Reserved.Should().BeNull();
            onHandledAction.Invoke();
            keyEventArgs.Reserved.Should().Be(EventReservedValueMask.SuppressEvent);
        }

        [Test]
        public void GotoHideoutKeyCombinationShouldSendGotoHideoutCommandIfPathOfExileIsActiveWindow()
        {
            var keyEventArgs = new KeyboardHookEventArgs(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcF5 },
                Type = EventType.KeyPressed
            });
            this.pathOfExileProcessHelperMock.Setup(x => x.IsPathOfExileActiveWindow())
                .Returns(true);

            this.keyPressed.OnNext(keyEventArgs);

            this.mediatorMock.Verify(x => x.Send(It.IsAny<GotoHideoutCommand>(), It.IsAny<CancellationToken>()));
            keyEventArgs.Reserved.Should().Be(EventReservedValueMask.SuppressEvent);
        }

        [Test]
        public void GotoHideoutKeyCombinationShouldNotSendGotoHideoutCommandIfPathOfExileIsNotActiveWindow()
        {
            var keyEventArgs = new KeyboardHookEventArgs(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcF5 },
                Type = EventType.KeyPressed
            });

            this.keyPressed.OnNext(keyEventArgs);

            this.mediatorMock.Verify(x => x.Send(It.IsAny<GotoHideoutCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            keyEventArgs.Reserved.Should().BeNull();
        }

        [Test]
        public void OpenWikiKeyCombinationShouldSendOpenWikiCommandIfPathOfExileIsActiveWindow()
        {
            var keyEventArgs = new KeyboardHookEventArgs(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcW },
                Mask = ModifierMask.Alt,
                Type = EventType.KeyPressed
            });
            this.pathOfExileProcessHelperMock.Setup(x => x.IsPathOfExileActiveWindow())
                .Returns(true);

            this.keyPressed.OnNext(keyEventArgs);

            this.mediatorMock.Verify(x => x.Send(It.IsAny<OpenWikiCommand>(), It.IsAny<CancellationToken>()));
            keyEventArgs.Reserved.Should().Be(EventReservedValueMask.SuppressEvent);
        }

        [Test]
        public void OpenWikiKeyCombinationShouldNotSendOpenWikiCommandIfPathOfExileIsNotActiveWindow()
        {
            var keyEventArgs = new KeyboardHookEventArgs(new UioHookEvent
            {
                Keyboard = new KeyboardEventData { KeyCode = KeyCode.VcW },
                Mask = ModifierMask.Alt,
                Type = EventType.KeyPressed
            });

            this.keyPressed.OnNext(keyEventArgs);

            this.mediatorMock.Verify(x => x.Send(It.IsAny<OpenWikiCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            keyEventArgs.Reserved.Should().BeNull();
        }
    }
}