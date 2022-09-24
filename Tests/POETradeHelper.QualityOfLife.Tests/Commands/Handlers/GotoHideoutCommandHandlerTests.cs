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
        private Mock<IUserInputSimulator> userInputSimulatorMock;
        private GotoHideoutCommandHandler goToHideoutCommandHandler;

        [SetUp]
        public void Setup()
        {
            this.userInputSimulatorMock = new Mock<IUserInputSimulator>();
            this.goToHideoutCommandHandler = new GotoHideoutCommandHandler(this.userInputSimulatorMock.Object);
        }

        [Test]
        public async Task HandleShouldCallSendGotoHideoutCommandOnUserInputSimulator()
        {
            await this.goToHideoutCommandHandler.Handle(new GotoHideoutCommand(), default);

            this.userInputSimulatorMock.Verify(x => x.SendGotoHideoutCommand());
        }
    }
}