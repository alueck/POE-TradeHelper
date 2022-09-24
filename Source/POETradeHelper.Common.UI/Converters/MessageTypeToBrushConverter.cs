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
        private static readonly IDictionary<MessageType, IBrush> messageTypeColorMappings = new Dictionary<MessageType, IBrush>
        {
            [MessageType.Info] = Brushes.Blue,
            [MessageType.Success] = Brushes.Green,
            [MessageType.Warning] = Brushes.Orange,
            [MessageType.Error] = Brushes.Red
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IBrush result = null;

            if (typeof(IBrush).IsAssignableFrom(targetType) && value is MessageType messageType)
            {
                result = messageTypeColorMappings[messageType];
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