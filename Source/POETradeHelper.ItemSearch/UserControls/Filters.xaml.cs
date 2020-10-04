using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

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

        public string Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static AvaloniaProperty<string> HeaderProperty = AvaloniaProperty.Register<Filters, string>(nameof(Header));

        private void TextBox_GotFocus(object sender, GotFocusEventArgs eventArgs)
        {
            var textBox = (TextBox)sender;

            if (textBox != null)
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