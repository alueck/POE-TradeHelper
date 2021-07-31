using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using POETradeHelper.Common;
using POETradeHelper.Common.UI;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Properties;
using POETradeHelper.PathOfExileTradeApi.Services;
using ReactiveUI;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public class ItemSearchSettingsViewModel : ReactiveObject, ISettingsViewModel
    {
        private bool isBusy;
        private IList<League> leagues;
        private League selectedLeague;
        private bool pricePredictionEnabled;
        private readonly ILeagueDataService leagueDataService;
        private readonly IWritableOptions<ItemSearchOptions> itemSearchOptions;

        public ItemSearchSettingsViewModel(ILeagueDataService leagueDataService, IWritableOptions<ItemSearchOptions> itemSearchOptions)
        {
            this.leagueDataService = leagueDataService;
            this.itemSearchOptions = itemSearchOptions;
        }

        public bool IsBusy
        {
            get => isBusy;
            set => this.RaiseAndSetIfChanged(ref this.isBusy, value);
        }

        public IList<League> Leagues
        {
            get => leagues;
            set => this.RaiseAndSetIfChanged(ref this.leagues, value);
        }

        public League SelectedLeague
        {
            get => selectedLeague;
            set => this.RaiseAndSetIfChanged(ref this.selectedLeague, value);
        }

        public bool PricePredictionEnabled
        {
            get => pricePredictionEnabled;
            set => this.RaiseAndSetIfChanged(ref pricePredictionEnabled, value);
        }

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
            SaveSettingsPrivate(true);
        }

        private void SaveSettingsPrivate(bool resetIsBusy)
        {
            try
            {
                this.IsBusy = true;

                this.itemSearchOptions.Update(o =>
                {
                    o.League = this.SelectedLeague;
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