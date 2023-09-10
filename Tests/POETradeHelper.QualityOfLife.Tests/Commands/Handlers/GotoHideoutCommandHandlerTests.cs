using System.Threading.Tasks;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Contract.Commands;
using POETradeHelper.QualityOfLife.Commands.Handlers;

namespace POETradeHelper.QualityOfLife.Tests.Commands.Handlers
{
    public class GotoHideoutCommandHandlerTests
    {
        private IUserInputSimulator userInputSimulatorMock;
        private GotoHideoutCommandHandler goToHideoutCommandHandler;

        [SetUp]
        public void Setup()
        {
            this.userInputSimulatorMock = Substitute.For<IUserInputSimulator>();
            this.goToHideoutCommandHandler = new GotoHideoutCommandHandler(this.userInputSimulatorMock);
        }

        [Test]
        public async Task HandleShouldCallSendGotoHideoutCommandOnUserInputSimulator()
        {
            await this.goToHideoutCommandHandler.Handle(new GotoHideoutCommand(), default);

            await this.userInputSimulatorMock
                .Received()
                .SendGotoHideoutCommand();
        }
    }
}