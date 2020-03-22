using Avalonia.Media.Imaging;
using System;
using System.Threading.Tasks;

namespace POETradeHelper.Common.UI
{
    public interface IImageService
    {
        Task<IBitmap> GetImageAsync(Uri uri);
    }
}