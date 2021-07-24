using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace POETradeHelper.ItemSearch.Views
{
    public class FilterTextBoxControl : UserControl
    {
        public FilterTextBoxControl()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public int? Value
        {
            get => (int?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static AvaloniaProperty<int?> ValueProperty = AvaloniaProperty.Register<FilterTextBoxControl, int?>(nameof(Value), defaultBindingMode: BindingMode.TwoWay);

        public string Watermark
        {
            get => (string)GetValue(WatermarkProperty);
            set => SetValue(WatermarkProperty, value);
        }

        public static AvaloniaProperty<string> WatermarkProperty = AvaloniaProperty.Register<FilterTextBoxControl, string>(nameof(Watermark));

        public void TextBox_GotFocus(object sender, GotFocusEventArgs eventArgs)
        {
            var textBox = (TextBox)sender;

            if (textBox?.Text != null)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    textBox.SelectionStart = 0;
                    textBox.SelectionEnd = textBox.Text.Length;
                });
            }
        }
    }
}