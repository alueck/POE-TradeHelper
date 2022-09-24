﻿using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

using POETradeHelper.Common.Contract;
using POETradeHelper.Common.UI;
using POETradeHelper.Common.UI.Models;
using POETradeHelper.Properties;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

namespace POETradeHelper.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel(IEnumerable<ISettingsViewModel> settingsViewModels, IEnumerable<IInitializable> initializables)
        {
            this.SettingsViewModels = settingsViewModels;

            this.InitializeAsync(initializables);

            this.SaveSettingsCommand = ReactiveCommand.Create(this.SaveSettings);
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
                .ToPropertyEx(this, x => x.SaveSettingsMessage);
        }

        public IEnumerable<ISettingsViewModel> SettingsViewModels { get; }

        [Reactive]
        public bool IsBusy { get; private set; }

        [Reactive]
        public string IsBusyText { get; private set; }

        [ObservableAsProperty]
        public Message SaveSettingsMessage { get; }

        public ReactiveCommand<Unit, bool> SaveSettingsCommand { get; }

        [Reactive]
        public Message ErrorMessage { get; private set; }

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
            foreach (var settingsViewModel in this.SettingsViewModels)
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

        private bool SaveSettings()
        {
            try
            {
                foreach (var settingsViewModel in this.SettingsViewModels)
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
