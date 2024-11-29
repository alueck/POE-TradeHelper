using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using POETradeHelper.Common.UI;
using POETradeHelper.Common.WritableOptions;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.UI.Avalonia.Properties;
using POETradeHelper.PathOfExileTradeApi.Services;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels
{
    public class ItemSearchSettingsViewModel : ReactiveObject, ISettingsViewModel
    {
        private readonly ILeagueDataService leagueDataService;
        private readonly IWritableOptions<ItemSearchOptions> itemSearchOptions;

        public ItemSearchSettingsViewModel(ILeagueDataService leagueDataService, IWritableOptions<ItemSearchOptions> itemSearchOptions)
        {
            this.leagueDataService = leagueDataService;
            this.itemSearchOptions = itemSearchOptions;
        }

        [Reactive]
        public bool IsBusy { get; private set; }

        [Reactive]
        public IList<League> Leagues { get; private set; } = [];

        [Reactive]
        public League? SelectedLeague { get; set; }

        [Reactive]
        public bool PricePredictionEnabled { get; set; }

        public string Title => Resources.ItemSearchSettingsHeader;

        public Task InitializeAsync()
        {
            this.IsBusy = true;
            this.Leagues = this.leagueDataService.GetLeaguesData().Select(l => new League { Id = l.Id, Text = l.Text }).ToList();
            this.SelectedLeague = this.Leagues.FirstOrDefault(league => string.Equals(this.itemSearchOptions.Value.League?.Id, league.Id, StringComparison.Ordinal)) ?? this.Leagues.FirstOrDefault();
            this.PricePredictionEnabled = this.itemSearchOptions.Value.PricePredictionEnabled;

            if (this.SelectedLeague != default)
            {
                this.SaveSettingsPrivate(false);
            }

            this.IsBusy = false;

            return Task.CompletedTask;
        }

        public void SaveSettings()
        {
            this.SaveSettingsPrivate(true);
        }

        private void SaveSettingsPrivate(bool resetIsBusy)
        {
            try
            {
                this.IsBusy = true;

                this.itemSearchOptions.Update(o =>
                {
                    o.League = this.SelectedLeague!;
                    o.PricePredictionEnabled = this.PricePredictionEnabled;
                });
            }
            finally
            {
                this.IsBusy = !resetIsBusy;
            }
        }
    }
}