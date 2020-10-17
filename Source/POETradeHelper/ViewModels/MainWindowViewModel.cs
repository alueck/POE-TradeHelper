using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
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
        private IDisposable settingsViewModelsInitializationSubscription;

        public MainWindowViewModel(IEnumerable<ISettingsViewModel> settingsViewModels, IEnumerable<IInitializable> initializables)
        {
            this.settingsViewModels = settingsViewModels;

            ConfigureIsBusySubscription(initializables);

            this.SaveSettingsCommand = ReactiveCommand.Create(SaveSettings);
            ConfigureSaveSettingsCommand();
        }

        private void ConfigureIsBusySubscription(IEnumerable<IInitializable> initializables)
        {
            IObservable<bool> settingsViewModelsBusyObservable = this.settingsViewModels
                                                                        .AsObservableChangeSet()
                                                                        .AutoRefreshOnObservable(x => x.ObservableForProperty(x => x.IsBusy))
                                                                        .ObserveOn(RxApp.MainThreadScheduler)
                                                                        .Select(changeSet => changeSet.Any(settingsViewModelChange => settingsViewModelChange.Item.Current.IsBusy));

            IObservable<bool> initializablesBusyObservable = Observable.Return(true).Concat(initializables
                .Select(x => x.OnInitAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        this.Log().Error(t.Exception);
                        this.ErrorMessage = new Message
                        {
                            Text = Resources.ProblemCommunicatingWithPoeApi,
                            Type = MessageType.Error
                        };
                    }
                    return Observable.Empty<Unit>();
                }))
                .ToObservable()
                .Switch()

                .IsEmpty());

            this.settingsViewModelsInitializationSubscription = initializablesBusyObservable
                .Subscribe(async initializing =>
                {
                    if (initializing)
                    {
                        this.IsBusyText = Resources.RetrievingDataText;
                    }
                    else
                    {
                        this.IsBusyText = null;
                        await this.InitializeSettingsViewModelsAsync();
                    }
                });

            settingsViewModelsBusyObservable
                .CombineLatest(initializablesBusyObservable, (settingsViewModelBusy, initializableBusy) => settingsViewModelBusy || initializableBusy)
                .Subscribe(busy => this.IsBusy = busy);
        }

        private async Task InitializeSettingsViewModelsAsync()
        {
            foreach (var settingsViewModel in this.settingsViewModels)
            {
                await settingsViewModel.InitializeAsync();
            }

            this.settingsViewModelsInitializationSubscription.Dispose();
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