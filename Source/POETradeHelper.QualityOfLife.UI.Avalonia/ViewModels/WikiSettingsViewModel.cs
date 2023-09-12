using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using POETradeHelper.Common.UI;
using POETradeHelper.Common.WritableOptions;
using POETradeHelper.QualityOfLife.Models;
using POETradeHelper.QualityOfLife.UI.Avalonia.Properties;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace POETradeHelper.QualityOfLife.UI.Avalonia.ViewModels
{
    public class WikiSettingsViewModel : ReactiveObject, ISettingsViewModel
    {
        private readonly IWritableOptions<WikiOptions> wikiOptions;

        public WikiSettingsViewModel(IWritableOptions<WikiOptions> wikiOptions)
        {
            this.wikiOptions = wikiOptions;
            this.WikiTypes = Enum.GetValues<WikiType>();
        }

        public string Title => Resources.WikiSettingsHeader;

        public bool IsBusy => false;

        public IEnumerable<WikiType> WikiTypes { get; }

        [Reactive]
        public WikiType SelectedWikiType { get; private set; }

        public Task InitializeAsync()
        {
            this.SelectedWikiType = this.wikiOptions.Value.Wiki;
            return Task.CompletedTask;
        }

        public void SaveSettings()
        {
            this.wikiOptions.Update(options => options.Wiki = this.SelectedWikiType);
        }
    }
}