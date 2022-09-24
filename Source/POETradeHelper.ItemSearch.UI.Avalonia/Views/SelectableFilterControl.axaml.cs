using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Views
{
    public class SelectableFilterControl : UserControl
    {
        public SelectableFilterControl()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public FilterViewModelBase Filter
        {
            get => this.GetValue<FilterViewModelBase>(FilterProperty);
            set => this.SetValue(FilterProperty, value);
        }

        public static AvaloniaProperty<FilterViewModelBase> FilterProperty = AvaloniaProperty.Register<SelectableFilterControl, FilterViewModelBase>(nameof(Filter));

        public bool IsThreeState
        {
            get => this.GetValue<bool>(IsThreeStateProperty);
            set => this.SetValue(IsThreeStateProperty, value);
        }

        public static AvaloniaProperty<bool> IsThreeStateProperty = AvaloniaProperty.Register<SelectableFilterControl, bool>(nameof(IsThreeState));
    }
}
