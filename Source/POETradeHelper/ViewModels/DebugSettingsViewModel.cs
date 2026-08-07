using System.Threading.Tasks;

using Mediator;

using POETradeHelper.Common.Contract.Commands;
using POETradeHelper.Common.UI;
using POETradeHelper.Properties;

using ReactiveUI;
using ReactiveUI.Primitives;

namespace POETradeHelper.ViewModels
{
    public class DebugSettingsViewModel : ReactiveObject, ISettingsViewModel
    {
        public DebugSettingsViewModel(IMediator mediator)
        {
            this.SearchItemFromClipboardCommand = ReactiveCommand.CreateFromTask(async () => await mediator.Send(new SearchItemCommand()));
            this.OpenWikiCommand = ReactiveCommand.CreateFromTask(async () => await mediator.Send(new OpenWikiCommand()));
        }

        public ReactiveCommand<RxVoid, Unit> SearchItemFromClipboardCommand { get; }

        public ReactiveCommand<RxVoid, Unit> OpenWikiCommand { get; }

        public string Title => Resources.DebugSettingsHeader;

        public bool IsBusy => false;

        public Task InitializeAsync() => Task.CompletedTask;

        public void SaveSettings()
        {
        }
    }
}