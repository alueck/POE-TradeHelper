using System.Threading.Tasks;

namespace POETradeHelper.Common.Contract
{
    public interface IClipboardHelper
    {
        Task<string> GetTextAsync();

        Task SetTextAsync(string text);

        Task ClearAsync();
    }
}