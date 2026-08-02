using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;

using POETradeHelper.Common.Contract;
using POETradeHelper.Common.UI;

namespace POETradeHelper
{
    [ExcludeFromCodeCoverage]
    public class ClipboardHelper : IClipboardHelper
    {
        private readonly IUiThreadDispatcher uiThreadDispatcher;

        public ClipboardHelper(IUiThreadDispatcher uiThreadDispatcher)
        {
            this.uiThreadDispatcher = uiThreadDispatcher;
        }

        private readonly IClipboard clipboard = ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!)!.MainWindow!.Clipboard!;

        public async Task ClearAsync()
        {
            await this.uiThreadDispatcher.InvokeAsync(this.clipboard.ClearAsync);
        }

        public async Task<string?> GetTextAsync()
        {
            return await this.uiThreadDispatcher.InvokeAsync(this.clipboard.TryGetTextAsync);
        }

        public async Task SetTextAsync(string? text)
        {
            await this.uiThreadDispatcher.InvokeAsync(async () => await this.clipboard.SetTextAsync(text));
        }
    }
}