using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;

using POETradeHelper.Common.Contract;

namespace POETradeHelper
{
    [ExcludeFromCodeCoverage]
    public class ClipboardHelper : IClipboardHelper
    {
        private readonly IClipboard clipboard = ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!)!.MainWindow!.Clipboard!;

        public Task ClearAsync() => this.clipboard.ClearAsync();

        public Task<string?> GetTextAsync() => this.clipboard.GetTextAsync();

        public Task SetTextAsync(string? text) => this.clipboard.SetTextAsync(text);
    }
}