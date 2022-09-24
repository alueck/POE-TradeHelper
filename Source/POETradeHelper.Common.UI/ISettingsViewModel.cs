using System.Threading.Tasks;

namespace POETradeHelper.Common.UI
{
    public interface ISettingsViewModel
    {
        string Title { get; }

        bool IsBusy { get; }

        void SaveSettings();

        Task InitializeAsync();
    }
}