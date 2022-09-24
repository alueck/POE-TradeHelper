using System.Collections;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Views
{
    public class FiltersControl : UserControl
    {
        public FiltersControl()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public ICollection Items
        {
            get => (ICollection)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public static AvaloniaProperty<ICollection> ItemsProperty = AvaloniaProperty.Register<FiltersControl, ICollection>(nameof(Items));

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static AvaloniaProperty<string> HeaderProperty = AvaloniaProperty.Register<FiltersControl, string>(nameof(Header));
    }
}