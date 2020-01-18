using Gma.System.MouseKeyHook;
using POETradeHelper.Common.Contract;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace POETradeHelper
{
    public class UserInputEventProvider : IUserInputEventProvider
    {
        private readonly IKeyboardMouseEvents globalInputEventsHook;

        public event EventHandler<HandledEventArgs> SearchItem;

        public event EventHandler<HandledEventArgs> HideOverlay;

        public UserInputEventProvider(IKeyboardMouseEvents globalInputEventsHook)
        {
            this.globalInputEventsHook = globalInputEventsHook;

            this.globalInputEventsHook.KeyDown += GlobalEventsHook_KeyDown;
        }

        private void GlobalEventsHook_KeyDown(object sender, KeyEventArgs e)
        {
            var handledEventArgs = new HandledEventArgs();

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D)
            {
                SearchItem.Invoke(this, handledEventArgs);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                HideOverlay.Invoke(this, handledEventArgs);
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