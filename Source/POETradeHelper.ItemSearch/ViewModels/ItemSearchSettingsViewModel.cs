using Microsoft.Extensions.Options;
using POETradeHelper.Common.UI;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.PathOfExileTradeApi.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public class ItemSearchSettingsViewModel : ReactiveObject, ISettingsViewModel
    {
        private bool isBusy;
        private IList<League> leagues;
        private League selectedLeague;
        private readonly IPoeTradeApiClient poeTradeApiClient;
        private readonly IOptions<ItemSearchOptions> itemSearchOptions;

        public ItemSearchSettingsViewModel(IPoeTradeApiClient poeTradeApiClient, IOptions<ItemSearchOptions> itemSearchOptions)
        {
            this.poeTradeApiClient = poeTradeApiClient;
            this.itemSearchOptions = itemSearchOptions;

            Initialize();
        }

        public bool IsBusy
        {
            get { return isBusy; }
            set { this.RaiseAndSetIfChanged(ref this.isBusy, value); }
        }

        public IList<League> Leagues
        {
            get { return leagues; }
            set { this.RaiseAndSetIfChanged(ref this.leagues, value); }
        }

        public League SelectedLeague
        {
            get { return selectedLeague; }
            set { this.RaiseAndSetIfChanged(ref this.selectedLeague, value); }
        }

        public string Title => "Item search settings";

        private async Task Initialize()
        {
            this.IsBusy = true;
            this.Leagues = await this.poeTradeApiClient.GetLeaguesAsync();
            this.SelectedLeague = this.Leagues.FirstOrDefault(league => string.Equals(this.itemSearchOptions.Value.League?.Id, league.Id, StringComparison.Ordinal)) ?? this.Leagues.First();
            this.SaveSettings();
            this.IsBusy = false;
        }

        public void SaveSettings()
        {
            this.itemSearchOptions.Value.League = this.SelectedLeague;
        }
    }
}