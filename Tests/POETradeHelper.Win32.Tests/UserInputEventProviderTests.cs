using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using POETradeHelper.Common.Contract;
using WindowsHook;

namespace POETradeHelper.Win32.Tests
{
    public class UserInputEventProviderTests
    {
        private Mock<IKeyboardMouseEvents> keyboardMouseEventsMock;
        private Mock<IPathOfExileProcessHelper> pathOfExileProcessHelperMock;
        private IUserInputEventProvider userInputEventProvider;

        [SetUp]
        public void Setup()
        {
            this.keyboardMouseEventsMock = new Mock<IKeyboardMouseEvents>();
            this.pathOfExileProcessHelperMock = new Mock<IPathOfExileProcessHelper>();
            this.userInputEventProvider = new UserInputEventProvider(this.keyboardMouseEventsMock.Object, this.pathOfExileProcessHelperMock.Object);
        }

        [Test]
        public void SearchItemKeyCombinationShouldTriggerSearchItemEventIfPathOfExileIsActiveWindow()
        {
            this.pathOfExileProcessHelperMock.Setup(x => x.IsPathOfExileActiveWindow())
                .Returns(true);

            bool eventWasDispatched = false;
            this.userInputEventProvider.SearchItem += (sender, args) => eventWasDispatched = true;

            keyboardMouseEventsMock.Raise(x => x.KeyDown += null, new KeyEventArgs(Keys.Control | Keys.D));

            Assert.True(eventWasDispatched);
        }

        [Test]
        public void SearchItemKeyCombinationShouldNotTriggerSearchItemEventIfPathOfExileIsNotActiveWindow()
        {
            bool eventWasDispatched = false;
            this.userInputEventProvider.SearchItem += (sender, args) => eventWasDispatched = true;

            keyboardMouseEventsMock.Raise(x => x.KeyDown += null, new KeyEventArgs(Keys.Control | Keys.D));

            Assert.False(eventWasDispatched);
        }

        [Test]
        public void HideOverlayKeyCombinationShoudTriggerHideOverlayEvent()
        {
            bool eventWasDispatched = false;
            this.userInputEventProvider.HideOverlay += (sender, args) => eventWasDispatched = true;

            keyboardMouseEventsMock.Raise(x => x.KeyDown += null, new KeyEventArgs(Keys.Escape));

            Assert.True(eventWasDispatched);
        }

        [Test]
        public void GoToHideoutKeyCombinationShoudTriggerGoToHideoutEventIfPathOfExileIsActiveWindow()
        {
            this.pathOfExileProcessHelperMock.Setup(x => x.IsPathOfExileActiveWindow())
                .Returns(true);

            bool eventWasDispatched = false;
            this.userInputEventProvider.GoToHidehout += (sender, args) => eventWasDispatched = true;

            keyboardMouseEventsMock.Raise(x => x.KeyDown += null, new KeyEventArgs(Keys.F5));

            Assert.True(eventWasDispatched);
        }

        [Test]
        public void GoToHideoutKeyCombinationShoudNotTriggerGoToHideoutEventIfPathOfExileIsNotActiveWindow()
        {
            bool eventWasDispatched = false;
            this.userInputEventProvider.GoToHidehout += (sender, args) => eventWasDispatched = true;

            keyboardMouseEventsMock.Raise(x => x.KeyDown += null, new KeyEventArgs(Keys.F5));

            Assert.False(eventWasDispatched);
        }
    }
}