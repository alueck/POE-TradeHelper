using System;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace POETradeHelper.Common.UI.UserControls
{
    public class BusyIndicator : UserControl
    {
        private readonly DispatcherTimer displayAfterTimer = new();

        public BusyIndicator()
        {
            displayAfterTimer.Tick += DisplayAfterTimerEnded;
            this.InitializeComponent();
        }

        private void DisplayAfterTimerEnded(object? sender, EventArgs e)
        {
            displayAfterTimer.Stop();
            IsBusyIndicatorVisible = true;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            IsBusyProperty.Changed.Subscribe(OnIsBusyChanged);
        }

        private void OnIsBusyChanged(AvaloniaPropertyChangedEventArgs eventArgs)
        {
            if (eventArgs.Sender != this)
            {
                return;
            }

            if (eventArgs.NewValue is bool newValue && newValue)
            {
                if (DisplayAfter == TimeSpan.Zero)
                {
                    IsBusyIndicatorVisible = true;
                }
                else
                {
                    displayAfterTimer.Interval = DisplayAfter;
                    displayAfterTimer.Start();
                }
            }
            else
            {
                displayAfterTimer.Stop();
                IsBusyIndicatorVisible = false;
            }
        }

        public TimeSpan DisplayAfter
        {
            get => this.GetValue<TimeSpan>(DisplayAfterProperty);
            set => this.SetValue(DisplayAfterProperty, value);
        }

        public static AvaloniaProperty<TimeSpan> DisplayAfterProperty = AvaloniaProperty.Register<BusyIndicator, TimeSpan>(nameof(DisplayAfter), defaultValue: TimeSpan.FromMilliseconds(100));

        public bool IsBusy
        {
            get => this.GetValue<bool>(IsBusyProperty);
            set => this.SetValue(IsBusyProperty, value);
        }

        public static AvaloniaProperty<bool> IsBusyProperty = AvaloniaProperty.Register<BusyIndicator, bool>(nameof(IsBusy));

        public string? Text
        {
            get => this.GetValue<string?>(TextProperty);
            set => this.SetValue(TextProperty, value);
        }

        public static AvaloniaProperty<string?> TextProperty = AvaloniaProperty.Register<BusyIndicator, string?>(nameof(Text));

        protected bool IsBusyIndicatorVisible
        {
            get => this.GetValue<bool>(IsBusyIndicatorVisibleProperty);
            set => this.SetValue(IsBusyIndicatorVisibleProperty, value);
        }

        protected static AvaloniaProperty<bool> IsBusyIndicatorVisibleProperty = AvaloniaProperty.Register<BusyIndicator, bool>(nameof(IsBusyIndicatorVisible));
    }
}
