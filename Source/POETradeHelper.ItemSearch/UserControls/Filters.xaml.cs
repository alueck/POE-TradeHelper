using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections;

namespace POETradeHelper.ItemSearch.UserControls
{
    public class Filters : UserControl
    {
        public Filters()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public ICollection Items
        {
            get => GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public static AvaloniaProperty<ICollection> ItemsProperty = AvaloniaProperty.Register<Filters, ICollection>(nameof(Items));
    }
}