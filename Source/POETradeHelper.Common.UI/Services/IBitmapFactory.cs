using System.IO;

using Avalonia.Media.Imaging;

namespace POETradeHelper.Common.UI.Services
{
    public interface IBitmapFactory
    {
        IBitmap Create(Stream stream);
    }
}