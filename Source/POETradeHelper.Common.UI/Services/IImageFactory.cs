using Avalonia.Media.Imaging;
using System.IO;

namespace POETradeHelper.Common.UI.Services
{
    public interface IImageFactory
    {
        IBitmap Create(Stream stream);
    }
}