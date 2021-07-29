using System.Threading;
using WindowsHook;
using MediatR;
using Moq;
using NUnit.Framework;
using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Contract.Commands;
using POETradeHelper.Common.Contract.Queries;

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
        public void SearchItemKeyCombinationShouldTriggerSearchItemEventIfPathOfExileIsActiveWindow()
        {
            this.pathOfExileProcessHelperMock.Setup(x => x.IsPathOfExileActiveWindow())
                .Returns(true);

            this.keyboardMouseEventsMock.Raise(x => x.KeyDown += null, new KeyEventArgs(Keys.Control | Keys.D));

            this.mediatorMock.Verify(x => x.Send(It.IsAny<SearchItemCommand>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public void SearchItemKeyCombinationShouldNotTriggerSearchItemEventIfPathOfExileIsNotActiveWindow()
        {
            this.keyboardMouseEventsMock.Raise(x => x.KeyDown += null, new KeyEventArgs(Keys.Control | Keys.D));

            this.mediatorMock.Verify(x => x.Send(It.IsAny<SearchItemCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public void HideOverlayKeyCombinationShouldTriggerHideOverlayEvent()
        {
            this.keyboardMouseEventsMock.Raise(x => x.KeyDown += null, new KeyEventArgs(Keys.Escape));

            this.mediatorMock.Verify(x => x.Send(It.IsAny<HideOverlayQuery>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public void GotoHideoutKeyCombinationShouldTriggerGoToHideoutEventIfPathOfExileIsActiveWindow()
        {
            this.pathOfExileProcessHelperMock.Setup(x => x.IsPathOfExileActiveWindow())
                .Returns(true);

            this.keyboardMouseEventsMock.Raise(x => x.KeyDown += null, new KeyEventArgs(Keys.F5));

            this.mediatorMock.Verify(x => x.Send(It.IsAny<GotoHideoutCommand>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public void GotoHideoutKeyCombinationShouldNotTriggerGoToHideoutEventIfPathOfExileIsNotActiveWindow()
        {
            this.keyboardMouseEventsMock.Raise(x => x.KeyDown += null, new KeyEventArgs(Keys.F5));

            this.mediatorMock.Verify(x => x.Send(It.IsAny<GotoHideoutCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}