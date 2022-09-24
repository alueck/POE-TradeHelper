using Avalonia.Media.Imaging;

using System.IO;

namespace POETradeHelper.Common.UI.Services
{
    public interface IBitmapFactory
    {
        IBitmap Create(Stream stream);
    }
}