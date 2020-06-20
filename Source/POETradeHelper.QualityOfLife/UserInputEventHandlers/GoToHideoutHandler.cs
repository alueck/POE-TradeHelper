using POETradeHelper.Common;
using POETradeHelper.Common.Contract;

namespace POETradeHelper.QualityOfLife.UserInputEventHandlers
{
    public class GoToHideoutHandler : IUserInputEventHandler
    {
        private readonly IUserInputEventProvider userInputEventProvider;
        private readonly INativeKeyboard nativeKeyboard;
        private bool isExecuting;

        public GoToHideoutHandler(IUserInputEventProvider userInputEventProvider, INativeKeyboard nativeKeyboard)
        {
            this.userInputEventProvider = userInputEventProvider;
            this.nativeKeyboard = nativeKeyboard;

            this.userInputEventProvider.GoToHidehout += GoToHideout;
        }

        public void Dispose()
        {
            this.userInputEventProvider.GoToHidehout -= GoToHideout;
        }

        private void GoToHideout(object sender, System.ComponentModel.HandledEventArgs e)
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
        }
    }
}