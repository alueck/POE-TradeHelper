using System;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Media;

namespace POETradeHelper.Common.UI.Services
{
    public interface IImageService
    {
        Task<IImage?> GetImageAsync(Uri uri, CancellationToken cancellationToken = default);
    }
}
