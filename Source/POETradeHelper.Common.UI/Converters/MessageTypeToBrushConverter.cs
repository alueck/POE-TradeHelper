using Avalonia.Data.Converters;
using Avalonia.Media;
using POETradeHelper.Common.UI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace POETradeHelper.Common.UI.Converters
{
    public class MessageTypeToBrushConverter : IValueConverter
    {
        private static readonly IDictionary<MessageType, Color> messageTypeColorMappings = new Dictionary<MessageType, Color>
        {
            [MessageType.Info] = Colors.Blue,
            [MessageType.Success] = Colors.Green,
            [MessageType.Warning] = Colors.Orange,
            [MessageType.Error] = Colors.Red
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush result = null;

            if (targetType.IsAssignableFrom(typeof(Brush)) && value is MessageType messageType)
            {
                Color color = messageTypeColorMappings[messageType];
                result = new SolidColorBrush(color);
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType.IsAssignableFrom(typeof(MessageType)) && value is Color color)
            {
                return messageTypeColorMappings.First(kvp => kvp.Value.Equals(color));
            }

            return null;
        }
    }
}