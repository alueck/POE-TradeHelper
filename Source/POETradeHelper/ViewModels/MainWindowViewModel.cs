using System;
using System.Collections.Generic;

using System.Threading.Tasks;

using POETradeHelper.Common.Contract;
using POETradeHelper.Common.UI;
using POETradeHelper.Common.UI.Models;
using POETradeHelper.Properties;

using ReactiveUI;
using ReactiveUI.Primitives;
using ReactiveUI.Primitives.Signals;
using ReactiveUI.SourceGenerators;

using Splat;

namespace POETradeHelper.ViewModels
{
    public partial class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel(IEnumerable<ISettingsViewModel> settingsViewModels, IEnumerable<IInitializable> initializables)
        {
            this.SettingsViewModels = settingsViewModels;

            this.InitializeCommand = ReactiveCommand.CreateFromTask(() => this.InitializeAsync(initializables));
            this.SaveSettingsCommand = ReactiveCommand.Create(this.SaveSettings);
            this._saveSettingsMessageHelper = LinqExtensions.SwitchSelect(this.SaveSettingsCommand, success =>
                {
                    if (success)
                    {
                        Message successMessage = new() { Type = MessageType.Success, Text = Resources.SavedMessageText };
                        return Signal.Return(successMessage).Concat(Signal.Return<Message?>(null).Delay(TimeSpan.FromSeconds(3), RxSchedulers.MainThreadScheduler));
                    }

                    Message failedMessage = new() { Type = MessageType.Error, Text = Resources.FailedToSaveSettingsMessageText };
                    return Signal.Return(failedMessage);
                })
                .ToProperty(this, x => x.SaveSettingsMessage);
        }

        public IEnumerable<ISettingsViewModel> SettingsViewModels { get; }

        public ReactiveCommand<RxVoid, RxVoid> InitializeCommand { get; }

        [Reactive]
        public partial bool IsBusy { get; private set; }

        [Reactive]
        public partial string IsBusyText { get; private set; } = string.Empty;

        [ObservableAsProperty]
        public partial Message? SaveSettingsMessage { get; }

        public ReactiveCommand<RxVoid, bool> SaveSettingsCommand { get; }

        [Reactive]
        public partial Message? ErrorMessage { get; private set; }

        private async Task InitializeAsync(IEnumerable<IInitializable> initializables)
        {
            bool success = await this.InitializeAsync(
                async () => await InitializeInitializablesAsync(initializables).ConfigureAwait(true),
                Resources.RetrievingDataText,
                Resources.ProblemCommunicatingWithPoeApi,
                true).ConfigureAwait(true);

            if (success)
            {
                await this.InitializeAsync(
                    this.InitializeSettingViewModelsAsync,
                    Resources.InitializingApplicationText,
                    string.Format(Resources.InitializationError, FileConfiguration.PoeTradeHelperAppDataFolder),
                    false).ConfigureAwait(true);
            }
        }

        private static async Task InitializeInitializablesAsync(IEnumerable<IInitializable> initializables)
        {
            foreach (IInitializable initializable in initializables)
            {
                await initializable.OnInitAsync().ConfigureAwait(true);
            }
        }

        private async Task InitializeSettingViewModelsAsync()
        {
            foreach (ISettingsViewModel settingsViewModel in this.SettingsViewModels)
            {
                await settingsViewModel.InitializeAsync().ConfigureAwait(true);
            }
        }

        private async Task<bool> InitializeAsync(Func<Task> initializationFunc, string isBusyText, string errorText, bool resetBusyTextOnly)
        {
            this.IsBusy = true;
            this.IsBusyText = isBusyText;

            try
            {
                await initializationFunc().ConfigureAwait(true);
            }
            catch (Exception exception)
            {
                this.Log().Error(exception);
                this.ErrorMessage = new Message
                {
                    Text = errorText,
                    Type = MessageType.Error,
                };

                this.ResetIsBusy(resetBusyTextOnly);
                return false;
            }

            this.ResetIsBusy(resetBusyTextOnly);
            return true;
        }

        private void ResetIsBusy(bool resetTextOnly)
        {
            this.IsBusy = resetTextOnly;
            this.IsBusyText = string.Empty;
        }

        private bool SaveSettings()
        {
            try
            {
                foreach (ISettingsViewModel settingsViewModel in this.SettingsViewModels)
                {
                    settingsViewModel.SaveSettings();
                }
            }
            catch (Exception exception)
            {
                this.Log().Error(exception, "Failed to save settings.");
                return false;
            }

            return true;
        }
    }
}