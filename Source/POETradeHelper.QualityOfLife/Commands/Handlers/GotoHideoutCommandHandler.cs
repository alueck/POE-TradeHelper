using MediatR;

using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Contract.Commands;

namespace POETradeHelper.QualityOfLife.Commands.Handlers
{
    public class GotoHideoutCommandHandler : IRequestHandler<GotoHideoutCommand>
    {
        private readonly IUserInputSimulator userInputSimulator;
        private bool isExecuting;

        public GotoHideoutCommandHandler(IUserInputSimulator userInputSimulator)
        {
            this.userInputSimulator = userInputSimulator;
        }

        public async Task<Unit> Handle(GotoHideoutCommand request, CancellationToken cancellationToken)
        {
            if (!this.isExecuting)
            {
                try
                {
                    this.isExecuting = true;
                    await this.userInputSimulator.SendGotoHideoutCommand();
                }
                finally
                {
                    this.isExecuting = false;
                }
            }

            return Unit.Value;
        }
    }
}