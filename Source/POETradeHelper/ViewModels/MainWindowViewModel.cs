using DynamicData;
using Microsoft.Extensions.Options;
using POETradeHelper.Common.UI;
using POETradeHelper.Common.UI.Models;
using POETradeHelper.Properties;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;

namespace POETradeHelper.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private bool isBusy;
        private readonly IOptions<AppSettings> appSettings;
        private IEnumerable<ISettingsViewModel> settingsViewModels;
        private ObservableAsPropertyHelper<Message> saveSettingsMessage;

        public MainWindowViewModel(IEnumerable<ISettingsViewModel> settingsViewModels, IOptions<AppSettings> appSettings)
        {
            this.settingsViewModels = settingsViewModels;
            this.appSettings = appSettings;

            this.SaveSettingsCommand = ReactiveCommand.Create(SaveSettings);

            this.settingsViewModels
                .AsObservableChangeSet()
                .AutoRefreshOnObservable(x => x.ObservableForProperty(x => x.IsBusy))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(new AnonymousObserver<IChangeSet<ISettingsViewModel>>(changeSet => this.IsBusy = changeSet.Any(settingsViewModelChange => settingsViewModelChange.Item.Current.IsBusy)));

            ConfigureSaveSettingsCommand();
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
            get => isBusy;
            set => this.RaiseAndSetIfChanged(ref isBusy, value);
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
    }
}