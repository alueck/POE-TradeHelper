using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

using MediatR;

using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Contract.Commands;

using SharpHook;
using SharpHook.Native;
using SharpHook.Reactive;

namespace POETradeHelper.Common
{
    public class UserInputEventProvider : IUserInputEventProvider
    {
        private readonly IReactiveGlobalHook hook;
        private readonly IPathOfExileProcessHelper pathOfExileProcessHelper;
        private readonly IMediator mediator;
        private IDisposable subscription;

        public UserInputEventProvider(IReactiveGlobalHook hook, IPathOfExileProcessHelper pathOfExileProcessHelper, IMediator mediator)
        {
            this.hook = hook;
            this.pathOfExileProcessHelper = pathOfExileProcessHelper;
            this.mediator = mediator;
        }

        public Task OnInitAsync()
        {
            this.subscription = this.hook.KeyPressed
                .Select(args => Observable.FromAsync(() => this.OnKeyPressed(args)))
                .Concat()
                .Subscribe();

            return Task.CompletedTask;
        }

        private async Task OnKeyPressed(KeyboardHookEventArgs eventArgs)
        {
            if (eventArgs.Data.KeyCode == KeyCode.VcEscape)
            {
                void OnHandled() => eventArgs.Reserved = EventReservedValueMask.SuppressEvent;
                await this.mediator.Send(new HideOverlayCommand(OnHandled)).ConfigureAwait(false);
            }
            else if (this.pathOfExileProcessHelper.IsPathOfExileActiveWindow())
            {
                if (TryGetRequest(eventArgs, out var request))
                {
                    eventArgs.Reserved = EventReservedValueMask.SuppressEvent;
                    await this.mediator.Send(request).ConfigureAwait(false);
                }
            }
        }

        private static bool TryGetRequest(KeyboardHookEventArgs eventArgs, out IRequest request)
        {
            request = null;
            if (IsModifierPressed(eventArgs, ModifierMask.Ctrl) && eventArgs.Data.KeyCode == KeyCode.VcD)
            {
                request = new SearchItemCommand();
            }
            else if (eventArgs.Data.KeyCode == KeyCode.VcF5)
            {
                request = new GotoHideoutCommand();
            }
            else if (IsModifierPressed(eventArgs, ModifierMask.Alt) && eventArgs.Data.KeyCode == KeyCode.VcW)
            {
                request = new OpenWikiCommand();
            }

            return request != null;
        }

        private static bool IsModifierPressed(HookEventArgs eventArgs, ModifierMask modifier)
        {
            return (eventArgs.RawEvent.Mask & modifier) != ModifierMask.None;
        }

        public void Dispose()
        {
            this.subscription.Dispose();
        }
    }
}