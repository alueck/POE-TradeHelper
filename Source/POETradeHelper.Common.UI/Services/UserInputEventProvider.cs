using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Mediator;

using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Contract.Commands;

using ReactiveUI.Primitives.Extensions;
using ReactiveUI.Primitives.Signals;

using SharpHook;
using SharpHook.Data;

namespace POETradeHelper.Common.UI.Services
{
    public sealed class UserInputEventProvider : IUserInputEventProvider
    {
        private readonly IGlobalHook hook;
        private readonly IPathOfExileProcessHelper pathOfExileProcessHelper;
        private readonly IMediator mediator;
        private readonly IOverlayStatusProvider overlayStatusProvider;
        private IDisposable? eventSubscription;

        public UserInputEventProvider(
            IGlobalHook hook,
            IPathOfExileProcessHelper pathOfExileProcessHelper,
            IMediator mediator,
            IOverlayStatusProvider overlayStatusProvider)
        {
            this.hook = hook;
            this.pathOfExileProcessHelper = pathOfExileProcessHelper;
            this.mediator = mediator;
            this.overlayStatusProvider = overlayStatusProvider;
        }

        public Task OnInitAsync()
        {
            this.eventSubscription = Signal.FromEventPattern<KeyboardHookEventArgs>(
                handler => this.hook.KeyPressed += handler,
                handler =>  this.hook.KeyPressed -= handler)
                .SubscribeAsync(async x => await this.OnKeyPressed(x.EventArgs));

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this.eventSubscription?.Dispose();
        }

        private async Task OnKeyPressed(KeyboardHookEventArgs eventArgs)
        {
            if (eventArgs.Data.KeyCode == KeyCode.VcEscape)
            {
                eventArgs.SuppressEvent = this.overlayStatusProvider.IsVisible;
                await this.mediator.Send(new HideOverlayCommand()).ConfigureAwait(false);
            }
            else if (this.pathOfExileProcessHelper.IsPathOfExileActiveWindow() && TryGetRequest(eventArgs, out var request))
            {
                eventArgs.SuppressEvent = true;
                await this.mediator.Send(request).ConfigureAwait(false);
            }
        }

        private static bool TryGetRequest(KeyboardHookEventArgs eventArgs, [NotNullWhen(true)] out IRequest? request)
        {
            request = null;
            if (IsModifierPressed(eventArgs, EventMask.Alt) && eventArgs.Data.KeyCode == KeyCode.VcD)
            {
                request = new SearchItemCommand();
            }
            else if (eventArgs.Data.KeyCode == KeyCode.VcF5)
            {
                request = new GotoHideoutCommand();
            }
            else if (IsModifierPressed(eventArgs, EventMask.Alt) && eventArgs.Data.KeyCode == KeyCode.VcW)
            {
                request = new OpenWikiCommand();
            }

            return request != null;
        }

        private static bool IsModifierPressed(HookEventArgs eventArgs, EventMask modifier)
        {
            return (eventArgs.RawEvent.Mask & modifier) != EventMask.None;
        }
    }
}