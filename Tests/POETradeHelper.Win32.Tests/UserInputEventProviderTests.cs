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
        private IUserInputEventProvider userInputEventProvider;

        [SetUp]
        public void Setup()
        {
            this.keyboardMouseEventsMock = new Mock<IKeyboardMouseEvents>();
            this.userInputEventProvider = new UserInputEventProvider(this.keyboardMouseEventsMock.Object);
        }

        [Test]
        public void SearchItemKeyCombinationShouldTriggerSearchItemEvent()
        {
            bool eventWasDispatched = false;
            this.userInputEventProvider.SearchItem += (sender, args) => eventWasDispatched = true;

            keyboardMouseEventsMock.Raise(x => x.KeyDown += null, new KeyEventArgs(Keys.Control | Keys.D));

            Assert.True(eventWasDispatched);
        }

        [Test]
        public void HideOverlayKeyCombinationShoudTriggerHideOverlayEvent()
        {
            bool eventWasDispatched = false;
            this.userInputEventProvider.HideOverlay += (sender, args) => eventWasDispatched = true;

            keyboardMouseEventsMock.Raise(x => x.KeyDown += null, new KeyEventArgs(Keys.Escape));

            Assert.True(eventWasDispatched);
        }
    }
}