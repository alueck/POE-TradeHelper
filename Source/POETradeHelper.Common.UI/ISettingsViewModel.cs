using System.Threading.Tasks;

namespace POETradeHelper.Common.UI
{
    public interface ISettingsViewModel
    {
        string Title { get; }
        bool IsBusy { get; set; }

        void SaveSettings();

        Task InitializeAsync();
    }
}