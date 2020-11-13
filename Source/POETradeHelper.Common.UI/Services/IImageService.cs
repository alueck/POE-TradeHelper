using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;

namespace POETradeHelper.Common.UI.Services
{
    public interface IImageService
    {
        Task<IBitmap> GetImageAsync(Uri uri, CancellationToken cancellationToken = default);
    }
}