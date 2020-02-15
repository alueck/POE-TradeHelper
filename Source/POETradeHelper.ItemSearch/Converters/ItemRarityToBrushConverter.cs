using Avalonia.Data.Converters;
using Avalonia.Media;
using POETradeHelper.ItemSearch.Contract.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace POETradeHelper.ItemSearch.Converters
{
    public class ItemRarityToBrushConverter : IValueConverter
    {
        private static readonly IDictionary<ItemRarity, IBrush> itemRarityColorMappings = new Dictionary<ItemRarity, IBrush>
        {
            [ItemRarity.Normal] = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
            [ItemRarity.Magic] = new SolidColorBrush(Color.FromRgb(136, 136, 255)),
            [ItemRarity.Rare] = new SolidColorBrush(Color.FromRgb(255, 255, 119)),
            [ItemRarity.Unique] = new SolidColorBrush(Color.FromRgb(175, 96, 37))
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IBrush result = null;

            if (typeof(IBrush).IsAssignableFrom(targetType) && value is ItemRarity itemRarity)
            {
                if (!itemRarityColorMappings.TryGetValue(itemRarity, out result))
                {
                    result = new SolidColorBrush(Color.FromRgb(170, 158, 130));
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}