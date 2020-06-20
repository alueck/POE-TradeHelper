using System.ComponentModel;
using Moq;
using NUnit.Framework;
using POETradeHelper.Common.Contract;

namespace POETradeHelper.QualityOfLife.UserInputEventHandlers.Tests
{
    public class GoToHideoutHandlerTests
    {
        private Mock<IUserInputEventProvider> userInputEventProviderMock;
        private Mock<INativeKeyboard> nativeKeyboardMock;
        private GoToHideoutHandler goToHideoutHandler;

        [SetUp]
        public void Setup()
        {
            this.userInputEventProviderMock = new Mock<IUserInputEventProvider>();
            this.nativeKeyboardMock = new Mock<INativeKeyboard>();
            this.goToHideoutHandler = new GoToHideoutHandler(this.userInputEventProviderMock.Object, this.nativeKeyboardMock.Object);
        }

        [Test]
        public void GoToHideoutHandlerShouldCallSendGoToHideoutCommandToNativeKeyboard()
        {
            this.userInputEventProviderMock.Raise(x => x.GoToHidehout += null, new HandledEventArgs());

            this.nativeKeyboardMock.Verify(x => x.SendGoToHideoutCommand());
        }
    }
}