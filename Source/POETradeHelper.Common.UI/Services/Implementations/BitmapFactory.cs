using System.Diagnostics.CodeAnalysis;
using System.IO;

using Avalonia.Media.Imaging;

namespace POETradeHelper.Common.UI.Services
{
    [ExcludeFromCodeCoverage]
    public class BitmapFactory : IBitmapFactory
    {
        public IBitmap Create(Stream stream) => new Bitmap(stream);
    }
}