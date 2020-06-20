using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using POETradeHelper.Common.Contract;
using WindowsHook;

namespace POETradeHelper
{
    public class UserInputEventProvider : IUserInputEventProvider
    {
        private readonly IKeyboardMouseEvents globalInputEventsHook;
        private readonly IPathOfExileProcessHelper pathOfExileProcessHelper;

        public event EventHandler<HandledEventArgs> SearchItem;

        public event EventHandler<HandledEventArgs> HideOverlay;

        public event EventHandler<HandledEventArgs> GoToHidehout;

        public UserInputEventProvider(IKeyboardMouseEvents globalInputEventsHook, IPathOfExileProcessHelper pathOfExileProcessHelper)
        {
            this.globalInputEventsHook = globalInputEventsHook;

            this.globalInputEventsHook.KeyDown += GlobalEventsHook_KeyDown;
            this.pathOfExileProcessHelper = pathOfExileProcessHelper;
        }

        private void GlobalEventsHook_KeyDown(object sender, KeyEventArgs e)
        {
            var handledEventArgs = new HandledEventArgs();

            if (e.KeyCode == Keys.Escape)
            {
                HideOverlay.Invoke(this, handledEventArgs);
            }
            else if (this.pathOfExileProcessHelper.IsPathOfExileActiveWindow())
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D)
                {
                    SearchItem.Invoke(this, handledEventArgs);
                }
                else if (e.KeyCode == Keys.F5)
                {
                    GoToHidehout.Invoke(this, handledEventArgs);
                }
            }

            e.Handled = handledEventArgs.Handled;
        }

        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            this.globalInputEventsHook.KeyDown -= GlobalEventsHook_KeyDown;
        }
    }
}