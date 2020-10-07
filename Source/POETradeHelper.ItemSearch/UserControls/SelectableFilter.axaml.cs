﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace POETradeHelper.ItemSearch.UserControls
{
    public class SelectableFilter : UserControl
    {
        public SelectableFilter()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public object Filter
        {
            get => GetValue(FilterProperty);
            set => SetValue(FilterProperty, value);
        }

        public static AvaloniaProperty<object> FilterProperty = AvaloniaProperty.Register<SelectableFilter, object>(nameof(Filter));
    }
}