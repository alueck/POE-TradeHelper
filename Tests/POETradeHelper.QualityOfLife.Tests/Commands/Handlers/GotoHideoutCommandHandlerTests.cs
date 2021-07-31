using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Contract.Commands;
using POETradeHelper.QualityOfLife.Commands.Handlers;

namespace POETradeHelper.QualityOfLife.Tests.Commands.Handlers
{
    public class GotoHideoutCommandHandlerTests
    {
        private Mock<INativeKeyboard> nativeKeyboardMock;
        private GotoHideoutCommandHandler goToHideoutCommandHandler;

        [SetUp]
        public void Setup()
        {
            this.nativeKeyboardMock = new Mock<INativeKeyboard>();
            this.goToHideoutCommandHandler = new GotoHideoutCommandHandler(this.nativeKeyboardMock.Object);
        }

        [Test]
        public async Task HandleShouldCallSendGotoHideoutCommandToNativeKeyboard()
        {
            await this.goToHideoutCommandHandler.Handle(new GotoHideoutCommand(), default);

            this.nativeKeyboardMock.Verify(x => x.SendGotoHideoutCommand());
        }
    }
}