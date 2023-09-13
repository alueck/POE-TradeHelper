using System.IO;

using Avalonia.Media;

namespace POETradeHelper.Common.UI.Services
{
    public interface IBitmapFactory
    {
        IImage Create(Stream stream);
    }
}