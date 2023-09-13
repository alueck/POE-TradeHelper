using System.Threading.Tasks;

using MediatR;

using POETradeHelper.Common.Contract.Commands;
using POETradeHelper.Common.UI;
using POETradeHelper.Properties;

using ReactiveUI;

using Unit = System.Reactive.Unit;

namespace POETradeHelper.ViewModels
{
    public class DebugSettingsViewModel : ReactiveObject, ISettingsViewModel
    {
        public DebugSettingsViewModel(IMediator mediator)
        {
            this.SearchItemFromClipboardCommand = ReactiveCommand.CreateFromTask(() => mediator.Send(new SearchItemCommand()));
        }

        public ReactiveCommand<Unit, Unit> SearchItemFromClipboardCommand { get; }

        public string Title => Resources.DebugSettingsHeader;

        public bool IsBusy => false;

        public Task InitializeAsync() => Task.CompletedTask;

        public void SaveSettings()
        {
        }
    }
}