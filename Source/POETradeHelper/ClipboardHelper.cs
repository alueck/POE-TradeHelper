using Avalonia;
using Avalonia.Input.Platform;
using POETradeHelper.Common.Contract;
using System.Threading.Tasks;

namespace POETradeHelper
{
    public class ClipboardHelper : IClipboardHelper
    {
        private readonly IClipboard clipboard;

        public ClipboardHelper()
        {
            this.clipboard = AvaloniaLocator.Current.GetService<IClipboard>();
        }

        public Task ClearAsync()
        {
            return clipboard.ClearAsync();
        }

        public Task<string> GetTextAsync()
        {
            return clipboard.GetTextAsync();
        }

        public Task SetTextAsync(string text)
        {
            return clipboard.SetTextAsync(text);
        }
    }
}