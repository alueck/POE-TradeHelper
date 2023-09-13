using System.Diagnostics.CodeAnalysis;
using System.IO;

using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace POETradeHelper.Common.UI.Services
{
    [ExcludeFromCodeCoverage]
    public class BitmapFactory : IBitmapFactory
    {
        public IImage Create(Stream stream) => new Bitmap(stream);
    }
}