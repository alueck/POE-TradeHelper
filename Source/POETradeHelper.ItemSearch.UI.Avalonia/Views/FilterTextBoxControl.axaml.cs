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
        public static readonly AvaloniaProperty<decimal?> ValueProperty =
            AvaloniaProperty.Register<FilterTextBoxControl, decimal?>(nameof(Value), defaultBindingMode: BindingMode.TwoWay);

        public static readonly AvaloniaProperty<string?> WatermarkProperty =
            AvaloniaProperty.Register<FilterTextBoxControl, string?>(nameof(Watermark));

        private readonly TextBox textBox;

        public FilterTextBoxControl()
        {
            this.InitializeComponent();
            this.textBox = this.Get<TextBox>("textBox");
            this.textBox.AddHandler(TextInputEvent, this.TextBox_OnTextInput, RoutingStrategies.Tunnel);
        }

        public decimal? Value
        {
            get => this.GetValue<decimal?>(ValueProperty);
            set => this.SetValue(ValueProperty, value);
        }

        public string? Watermark
        {
            get => this.GetValue<string?>(WatermarkProperty);
            set => this.SetValue(WatermarkProperty, value);
        }

        public void TextBox_GotFocus(object sender, GotFocusEventArgs eventArgs)
        {
            if (this.textBox.Text != null)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    this.textBox.SelectionStart = 0;
                    this.textBox.SelectionEnd = this.textBox.Text.Length;
                });
            }
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        private void TextBox_OnTextInput(object? sender, TextInputEventArgs e)
        {
            string numberRegex = $@"^[\+\-]?\d+[\.{Regex.Escape(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)}]?\d*$";
            string newText = this.textBox.Text.Insert(this.textBox.SelectionStart, e.Text ?? string.Empty);
            bool isNumericText = Regex.IsMatch(newText, numberRegex);
            e.Handled = !isNumericText;
        }
    }
}