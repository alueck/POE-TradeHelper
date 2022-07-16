using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using POETradeHelper.Common;
using POETradeHelper.Common.UI;
using POETradeHelper.Common.WritableOptions;
using POETradeHelper.QualityOfLife.Models;
using POETradeHelper.QualityOfLife.Properties;
using ReactiveUI;

namespace POETradeHelper.QualityOfLife.ViewModels
{
    public class WikiSettingsViewModel : ReactiveObject, ISettingsViewModel
    {
        private readonly IWritableOptions<WikiOptions> wikiOptions;

        public WikiSettingsViewModel(IWritableOptions<WikiOptions> wikiOptions)
        {
            this.wikiOptions = wikiOptions;
        }

        public string Title => Resources.WikiSettingsHeader;

        public bool IsBusy => false;

        private IEnumerable<WikiType> wikiTypes;

        public IEnumerable<WikiType> WikiTypes
        {
            get => wikiTypes;
            set => this.RaiseAndSetIfChanged(ref wikiTypes, value);
        }

        private WikiType? selectedWikiType;

        public WikiType SelectedWikiType
        {
            get => this.selectedWikiType.GetValueOrDefault();
            set => this.RaiseAndSetIfChanged(ref selectedWikiType, value);
        }

        public Task InitializeAsync()
        {
            this.WikiTypes = Enum.GetValues<WikiType>();
            this.SelectedWikiType = this.wikiOptions.Value.Wiki;
            return Task.CompletedTask;
        }

        public void SaveSettings()
        {
            this.wikiOptions.Update(options =>
            {
                options.Wiki = this.selectedWikiType.GetValueOrDefault();
            });
        }
    }
}