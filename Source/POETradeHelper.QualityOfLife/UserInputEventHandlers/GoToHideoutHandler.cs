using System.Threading;
using System.Threading.Tasks;
using MediatR;
using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Contract.Commands;

namespace POETradeHelper.QualityOfLife.UserInputEventHandlers
{
    public class GoToHideoutHandler : IRequestHandler<GotoHideoutCommand>
    {
        private readonly INativeKeyboard nativeKeyboard;
        private bool isExecuting;

        public GoToHideoutHandler(INativeKeyboard nativeKeyboard)
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
                    this.nativeKeyboard.SendGoToHideoutCommand();
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