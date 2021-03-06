﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using POETradeHelper.Common.UI.Models;

namespace POETradeHelper.Common.UI.UserControls
{
    public class MessageTextBlock : UserControl
    {
        public MessageTextBlock()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public Message Message
        {
            get => GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public static AvaloniaProperty<Message> MessageProperty = AvaloniaProperty.Register<MessageTextBlock, Message>(nameof(Message));
    }
}