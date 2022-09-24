using Avalonia.Data.Converters;

using System;
using System.Globalization;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Converters
{
    public class DoubleOffsetConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double doubleValue && parameter is string s && double.TryParse(s, out double d))
            {
                return doubleValue + d;
            }

            return value;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
