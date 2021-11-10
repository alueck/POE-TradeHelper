using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using POETradeHelper.Common;
using POETradeHelper.Common.UI;
using POETradeHelper.Common.UI.Models;
using POETradeHelper.Properties;
using ReactiveUI;
using Splat;

namespace POETradeHelper.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private bool isBusy;
        private string isBusyText;
        private Message errorMessage;
        private IEnumerable<ISettingsViewModel> settingsViewModels;
        private ObservableAsPropertyHelper<Message> saveSettingsMessage;

        public MainWindowViewModel(IEnumerable<ISettingsViewModel> settingsViewModels, IEnumerable<IInitializable> initializables)
        {
            this.settingsViewModels = settingsViewModels;

            InitializeAsync(initializables);

            this.SaveSettingsCommand = ReactiveCommand.Create(SaveSettings);
            ConfigureSaveSettingsCommand();
        }

        private async void InitializeAsync(IEnumerable<IInitializable> initializables)
        {
            bool success = await this.InitializeAsync(
                async () => await InitializeInitializablesAsync(initializables).ConfigureAwait(true),
                Resources.RetrievingDataText,
                Resources.ProblemCommunicatingWithPoeApi,
                resetBusyTextOnly: true).ConfigureAwait(true);

            if (success)
            {
                await this.InitializeAsync(
                    this.InitializeSettingViewModelsAsync,
                    Resources.InitializingApplicationText,
                    string.Format(Resources.InitializationError, FileConfiguration.PoeTradeHelperAppDataFolder),
                    resetBusyTextOnly: false).ConfigureAwait(true);
            }
        }

        private static async Task InitializeInitializablesAsync(IEnumerable<IInitializable> initializables)
        {
            foreach (var initializable in initializables)
            {
                await initializable.OnInitAsync().ConfigureAwait(true);
            }
        }

        private async Task InitializeSettingViewModelsAsync()
        {
            foreach (var settingsViewModel in this.settingsViewModels)
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
                    Type = MessageType.Error
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
            this.IsBusyText = null;
        }

        private void ConfigureSaveSettingsCommand()
        {
            this.SaveSettingsCommand
                .Select(success =>
                {
                    if (success)
                    {
                        var successMessage = new Message { Type = MessageType.Success, Text = Resources.SavedMessageText };
                        return Observable.Return(successMessage).Concat(Observable.Return((Message)null).Delay(TimeSpan.FromSeconds(3), RxApp.MainThreadScheduler));
                    }
                    else
                    {
                        var failedMessage = new Message { Type = MessageType.Error, Text = Resources.FailedToSaveSettingsMessageText };
                        return Observable.Return(failedMessage);
                    }
                })
                .Switch()
                .ToProperty(this, x => x.SaveSettingsMessage, out saveSettingsMessage);
        }

        private bool SaveSettings()
        {
            try
            {
                foreach (var settingsViewModel in settingsViewModels)
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

        public bool IsBusy
        {
            get => this.isBusy;
            set => this.RaiseAndSetIfChanged(ref this.isBusy, value);
        }

        public string IsBusyText
        {
            get => this.isBusyText;
            set => this.RaiseAndSetIfChanged(ref this.isBusyText, value);
        }

        public Message SaveSettingsMessage
        {
            get => saveSettingsMessage.Value;
        }

        public IEnumerable<ISettingsViewModel> SettingsViewModels
        {
            get => this.settingsViewModels;
            set => this.RaiseAndSetIfChanged(ref this.settingsViewModels, value);
        }

        public ReactiveCommand<Unit, bool> SaveSettingsCommand { get; }

        public Message ErrorMessage
        {
            get => this.errorMessage;
            set => this.RaiseAndSetIfChanged(ref this.errorMessage, value);
        }
    }
}