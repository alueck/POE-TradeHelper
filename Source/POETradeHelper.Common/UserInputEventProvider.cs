using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
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
    public sealed class UserInputEventProvider : IUserInputEventProvider
    {
        private readonly IReactiveGlobalHook hook;
        private readonly IPathOfExileProcessHelper pathOfExileProcessHelper;
        private readonly IMediator mediator;
        private readonly CompositeDisposable disposables = new();

        public UserInputEventProvider(IReactiveGlobalHook hook, IPathOfExileProcessHelper pathOfExileProcessHelper, IMediator mediator)
        {
            this.hook = hook;
            this.pathOfExileProcessHelper = pathOfExileProcessHelper;
            this.mediator = mediator;
        }

        public Task OnInitAsync()
        {
            var subscription = this.hook.KeyPressed
                .Select(args => Observable.FromAsync(() => this.OnKeyPressed(args)))
                .Concat()
                .Subscribe();
            this.disposables.Add(subscription);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this.disposables.Dispose();
        }

        private async Task OnKeyPressed(KeyboardHookEventArgs eventArgs)
        {
            if (eventArgs.Data.KeyCode == KeyCode.VcEscape)
            {
                void OnHandled() => eventArgs.SuppressEvent = true;
                await this.mediator.Send(new HideOverlayCommand(OnHandled)).ConfigureAwait(false);
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
    }
}