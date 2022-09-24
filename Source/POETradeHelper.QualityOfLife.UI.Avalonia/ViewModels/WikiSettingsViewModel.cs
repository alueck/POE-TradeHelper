using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using POETradeHelper.Common.UI;
using POETradeHelper.Common.WritableOptions;
using POETradeHelper.QualityOfLife.Models;
using POETradeHelper.QualityOfLife.Properties;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

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

        [Reactive]
        public IEnumerable<WikiType> WikiTypes { get; private set; }

        [Reactive]
        public WikiType SelectedWikiType { get; private set; }

        public Task InitializeAsync()
        {
            this.WikiTypes = Enum.GetValues<WikiType>();
            this.SelectedWikiType = this.wikiOptions.Value.Wiki;
            return Task.CompletedTask;
        }

        public void SaveSettings()
        {
            this.wikiOptions.Update(options => options.Wiki = this.SelectedWikiType);
        }
    }
}
