using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace POETradeHelper.ItemSearch.Views
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

        public object Filter
        {
            get => GetValue(FilterProperty);
            set => SetValue(FilterProperty, value);
        }

        public static AvaloniaProperty<object> FilterProperty = AvaloniaProperty.Register<SelectableFilterControl, object>(nameof(Filter));

        public bool IsThreeState
        {
            get => (bool)GetValue(IsThreeStateProperty);
            set => SetValue(IsThreeStateProperty, value);
        }

        public static AvaloniaProperty<bool> IsThreeStateProperty = AvaloniaProperty.Register<SelectableFilterControl, bool>(nameof(IsThreeState));
    }
}