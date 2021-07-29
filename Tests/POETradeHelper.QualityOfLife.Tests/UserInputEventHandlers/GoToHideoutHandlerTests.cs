using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Contract.Commands;

namespace POETradeHelper.QualityOfLife.UserInputEventHandlers.Tests
{
    public class GoToHideoutHandlerTests
    {
        private Mock<INativeKeyboard> nativeKeyboardMock;
        private GoToHideoutHandler goToHideoutHandler;

        [SetUp]
        public void Setup()
        {
            this.nativeKeyboardMock = new Mock<INativeKeyboard>();
            this.goToHideoutHandler = new GoToHideoutHandler(this.nativeKeyboardMock.Object);
        }

        [Test]
        public async Task HandleShouldCallSendGoToHideoutCommandToNativeKeyboard()
        {
            await this.goToHideoutHandler.Handle(new GotoHideoutCommand(), default);

            this.nativeKeyboardMock.Verify(x => x.SendGoToHideoutCommand());
        }
    }
}