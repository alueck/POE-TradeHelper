using System.Collections;

using Avalonia;
using Avalonia.Controls;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Views
{
    public partial class FiltersControl : UserControl
    {
        public static readonly AvaloniaProperty<ICollection> ItemsProperty = AvaloniaProperty.Register<FiltersControl, ICollection>(nameof(Items));
        public static readonly AvaloniaProperty<string> HeaderProperty = AvaloniaProperty.Register<FiltersControl, string>(nameof(Header));

        public FiltersControl()
        {
            this.InitializeComponent();
        }

        public ICollection Items
        {
            get => this.GetValue<ICollection>(ItemsProperty);
            set => this.SetValue(ItemsProperty, value);
        }

        public string Header
        {
            get => this.GetValue<string>(HeaderProperty);
            set => this.SetValue(HeaderProperty, value);
        }
    }
}