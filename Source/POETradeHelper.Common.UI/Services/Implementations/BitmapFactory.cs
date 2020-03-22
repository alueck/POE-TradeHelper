using Avalonia.Media.Imaging;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace POETradeHelper.Common.UI.Services
{
    [ExcludeFromCodeCoverage]
    public class BitmapFactory : IBitmapFactory
    {
        public IBitmap Create(Stream stream)
        {
            return new Bitmap(stream);
        }
    }
}