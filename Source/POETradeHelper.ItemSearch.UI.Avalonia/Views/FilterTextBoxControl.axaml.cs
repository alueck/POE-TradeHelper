using System.Globalization;
using System.Text.RegularExpressions;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Views
{
    public class FilterTextBoxControl : UserControl
    {
        public FilterTextBoxControl()
        {
            this.InitializeComponent();
            this.textBox = this.Get<TextBox>("textBox");
            this.textBox.AddHandler(TextInputEvent, TextBox_OnTextInput, RoutingStrategies.Tunnel);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public decimal? Value
        {
            get => this.GetValue<decimal?>(ValueProperty);
            set => this.SetValue(ValueProperty, value);
        }

        public static AvaloniaProperty<decimal?> ValueProperty = AvaloniaProperty.Register<FilterTextBoxControl, decimal?>(nameof(Value), defaultBindingMode: BindingMode.TwoWay);

        public string? Watermark
        {
            get => this.GetValue<string?>(WatermarkProperty);
            set => this.SetValue(WatermarkProperty, value);
        }

        public static AvaloniaProperty<string?> WatermarkProperty = AvaloniaProperty.Register<FilterTextBoxControl, string?>(nameof(Watermark));
        private readonly TextBox textBox;

        public void TextBox_GotFocus(object sender, GotFocusEventArgs eventArgs)
        {
            if (this.textBox.Text != null)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    this.textBox.SelectionStart = 0;
                    this.textBox.SelectionEnd = textBox.Text.Length;
                });
            }
        }

        private void TextBox_OnTextInput(object? sender, TextInputEventArgs e)
        {
            var numberRegex = $@"^[\+\-]?\d+[\.{Regex.Escape(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)}]?\d*$";
            var newText = this.textBox.Text.Insert(this.textBox.SelectionStart, e.Text ?? string.Empty);
            var isNumericText = Regex.IsMatch(newText, numberRegex);
            e.Handled = !isNumericText;
        }
    }
}
