using System.Threading;
using System.Threading.Tasks;
using MediatR;
using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Contract.Commands;

namespace POETradeHelper.QualityOfLife.Commands.Handlers
{
    public class GotoHideoutCommandHandler : IRequestHandler<GotoHideoutCommand>
    {
        private readonly INativeKeyboard nativeKeyboard;
        private bool isExecuting;

        public GotoHideoutCommandHandler(INativeKeyboard nativeKeyboard)
        {
            this.nativeKeyboard = nativeKeyboard;
        }

        public Task<Unit> Handle(GotoHideoutCommand request, CancellationToken cancellationToken)
        {
            if (!isExecuting)
            {
                try
                {
                    isExecuting = true;
                    this.nativeKeyboard.SendGotoHideoutCommand();
                }
                finally
                {
                    isExecuting = false;
                }
            }

            return Task.FromResult(Unit.Value);
        }
    }
}