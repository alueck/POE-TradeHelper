using System.Diagnostics.CodeAnalysis;
using WindowsHook;
using MediatR;
using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Contract.Commands;
using POETradeHelper.Common.Contract.Queries;
using POETradeHelper.QualityOfLife.Commands;

namespace POETradeHelper.Win32
{
    public class UserInputEventProvider : IUserInputEventProvider
    {
        private readonly IKeyboardMouseEvents globalInputEventsHook;
        private readonly IPathOfExileProcessHelper pathOfExileProcessHelper;
        private readonly IMediator mediator;

        public UserInputEventProvider(IKeyboardMouseEvents globalInputEventsHook, IPathOfExileProcessHelper pathOfExileProcessHelper, IMediator mediator)
        {
            this.globalInputEventsHook = globalInputEventsHook;

            this.globalInputEventsHook.KeyDown += GlobalEventsHook_KeyDown;
            this.pathOfExileProcessHelper = pathOfExileProcessHelper;
            this.mediator = mediator;
        }

        private async void GlobalEventsHook_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                var response = await this.mediator.Send(new HideOverlayQuery()).ConfigureAwait(false);
                e.Handled = response.Handled;
            }
            else if (this.pathOfExileProcessHelper.IsPathOfExileActiveWindow())
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D)
                {
                    e.Handled = true;
                    await this.mediator.Send(new SearchItemCommand()).ConfigureAwait(false);
                }
                else if (e.KeyCode == Keys.F5)
                {
                    e.Handled = true;
                    await this.mediator.Send(new GotoHideoutCommand()).ConfigureAwait(false);
                }
                else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.W)
                {
                    e.Handled = true;
                    await this.mediator.Send(new OpenWikiCommand()).ConfigureAwait(false);
                }
            }
        }

        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            this.globalInputEventsHook.KeyDown -= GlobalEventsHook_KeyDown;
        }
    }
}