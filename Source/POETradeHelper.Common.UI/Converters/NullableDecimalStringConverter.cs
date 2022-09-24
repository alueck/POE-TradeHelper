using System;
using System.Globalization;

using Avalonia.Data;
using Avalonia.Data.Converters;

namespace POETradeHelper.Common.UI.Converters
{
    public class NullableDecimalStringConverter : IValueConverter
    {
        private const NumberStyles NumberStyle = NumberStyles.AllowLeadingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((decimal?)value)?.ToString(culture) ?? "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value is string stringValue)
            {
                if (string.IsNullOrEmpty(stringValue))
                {
                    return null;
                }

                if (decimal.TryParse(stringValue, NumberStyle, culture, out var decimalValue)
                    || decimal.TryParse(stringValue, NumberStyle, CultureInfo.InvariantCulture, out decimalValue))
                {
                    return decimalValue;
                }
            }

            return new BindingNotification(new InvalidCastException($"'{value}' is not a valid number."), BindingErrorType.Error);
        }
    }
}