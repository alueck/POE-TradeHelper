using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using POETradeHelper.Common.UI.Models;

namespace POETradeHelper.Common.UI.UserControls
{
    public class MessageTextBlock : UserControl
    {
        public static readonly AvaloniaProperty<Message?> MessageProperty = AvaloniaProperty.Register<MessageTextBlock, Message?>(nameof(Message));

        public MessageTextBlock()
        {
            this.InitializeComponent();
        }

        public Message? Message
        {
            get => this.GetValue<Message?>(MessageProperty);
            set => this.SetValue(MessageProperty, value);
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}