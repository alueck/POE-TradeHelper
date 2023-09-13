using Avalonia;
using Avalonia.Controls;

using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Views
{
    public partial class SelectableFilterControl : UserControl
    {
        public static readonly AvaloniaProperty<FilterViewModelBase> FilterProperty =
            AvaloniaProperty.Register<SelectableFilterControl, FilterViewModelBase>(nameof(Filter));

        public static readonly AvaloniaProperty<bool> IsThreeStateProperty =
            AvaloniaProperty.Register<SelectableFilterControl, bool>(nameof(IsThreeState));

        public SelectableFilterControl()
        {
            this.InitializeComponent();
        }

        public FilterViewModelBase Filter
        {
            get => this.GetValue<FilterViewModelBase>(FilterProperty);
            set => this.SetValue(FilterProperty, value);
        }

        public bool IsThreeState
        {
            get => this.GetValue<bool>(IsThreeStateProperty);
            set => this.SetValue(IsThreeStateProperty, value);
        }
    }
}