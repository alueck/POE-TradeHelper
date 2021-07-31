using System.Threading;
using WindowsHook;
using MediatR;
using Moq;
using NUnit.Framework;
using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Contract.Commands;
using POETradeHelper.Common.Contract.Queries;
using POETradeHelper.QualityOfLife.Commands;

namespace POETradeHelper.Win32.Tests
{
    public class UserInputEventProviderTests
    {
        private Mock<IKeyboardMouseEvents> keyboardMouseEventsMock;
        private Mock<IPathOfExileProcessHelper> pathOfExileProcessHelperMock;
        private Mock<IMediator> mediatorMock;
        private IUserInputEventProvider userInputEventProvider;

        [SetUp]
        public void Setup()
        {
            this.keyboardMouseEventsMock = new Mock<IKeyboardMouseEvents>();
            this.pathOfExileProcessHelperMock = new Mock<IPathOfExileProcessHelper>();
            this.mediatorMock = new Mock<IMediator>();
            this.userInputEventProvider = new UserInputEventProvider(
                this.keyboardMouseEventsMock.Object,
                this.pathOfExileProcessHelperMock.Object,
                this.mediatorMock.Object);
        }

        [Test]
        public void SearchItemKeyCombinationShouldSendSearchItemCommandIfPathOfExileIsActiveWindow()
        {
            var keyEventArgs = new KeyEventArgs(Keys.Control | Keys.D);
            this.pathOfExileProcessHelperMock.Setup(x => x.IsPathOfExileActiveWindow())
                .Returns(true);
            
            this.keyboardMouseEventsMock.Raise(x => x.KeyDown += null, keyEventArgs);

            this.mediatorMock.Verify(x => x.Send(It.IsAny<SearchItemCommand>(), It.IsAny<CancellationToken>()));
            Assert.That(keyEventArgs.Handled, Is.True);
        }

        [Test]
        public void SearchItemKeyCombinationShouldNotSendSearchItemCommandIfPathOfExileIsNotActiveWindow()
        {
            var keyEventArgs = new KeyEventArgs(Keys.Control | Keys.D);
            this.keyboardMouseEventsMock.Raise(x => x.KeyDown += null, keyEventArgs);

            this.mediatorMock.Verify(x => x.Send(It.IsAny<SearchItemCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.That(keyEventArgs.Handled, Is.False);
        }

        [Test]
        public void HideOverlayKeyCombinationShouldSendHideOverlayQuery()
        {
            this.keyboardMouseEventsMock.Raise(x => x.KeyDown += null, new KeyEventArgs(Keys.Escape));

            this.mediatorMock.Verify(x => x.Send(It.IsAny<HideOverlayQuery>(), It.IsAny<CancellationToken>()));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ShouldSetEventArgsHandledFromHideOverlayQuery(bool expected)
        {
            var keyEventArgs = new KeyEventArgs(Keys.Escape);
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<HideOverlayQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HideOverlayResponse(expected));
            
            this.keyboardMouseEventsMock.Raise(x => x.KeyDown += null, keyEventArgs);

            Assert.That(keyEventArgs.Handled, Is.EqualTo(expected));
        }

        [Test]
        public void GotoHideoutKeyCombinationShouldSendGotoHideoutCommandIfPathOfExileIsActiveWindow()
        {
            var keyEventArgs = new KeyEventArgs(Keys.F5);
            this.pathOfExileProcessHelperMock.Setup(x => x.IsPathOfExileActiveWindow())
                .Returns(true);

            this.keyboardMouseEventsMock.Raise(x => x.KeyDown += null, keyEventArgs);

            this.mediatorMock.Verify(x => x.Send(It.IsAny<GotoHideoutCommand>(), It.IsAny<CancellationToken>()));
            Assert.That(keyEventArgs.Handled, Is.True);
        }

        [Test]
        public void GotoHideoutKeyCombinationShouldNotSendGotoHideoutCommandIfPathOfExileIsNotActiveWindow()
        {
            var keyEventArgs = new KeyEventArgs(Keys.F5);
            this.keyboardMouseEventsMock.Raise(x => x.KeyDown += null, keyEventArgs);

            this.mediatorMock.Verify(x => x.Send(It.IsAny<GotoHideoutCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.That(keyEventArgs.Handled, Is.False);
        }

        [Test]
        public void OpenWikiKeyCombinationShouldSendOpenWikiCommandIfPathOfExileIsActiveWindow()
        {
            var keyEventArgs = new KeyEventArgs(Keys.Alt | Keys.W);
            this.pathOfExileProcessHelperMock.Setup(x => x.IsPathOfExileActiveWindow())
                .Returns(true);

            this.keyboardMouseEventsMock.Raise(x => x.KeyDown += null, keyEventArgs);
            
            this.mediatorMock.Verify(x => x.Send(It.IsAny<OpenWikiCommand>(), It.IsAny<CancellationToken>()));
            Assert.That(keyEventArgs.Handled, Is.True);
        }
        
        [Test]
        public void OpenWikiKeyCombinationShouldNotSendOpenWikiCommandIfPathOfExileIsNotActiveWindow()
        {
            var keyEventArgs = new KeyEventArgs(Keys.Alt | Keys.W);
            this.keyboardMouseEventsMock.Raise(x => x.KeyDown += null, keyEventArgs);
            
            this.mediatorMock.Verify(x => x.Send(It.IsAny<OpenWikiCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.That(keyEventArgs.Handled, Is.False);
        }
    }
}