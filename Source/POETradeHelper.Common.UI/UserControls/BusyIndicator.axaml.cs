using System;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace POETradeHelper.Common.UI.UserControls
{
    public partial class BusyIndicator : UserControl
    {
        public static readonly AvaloniaProperty<TimeSpan> DisplayAfterProperty = AvaloniaProperty.Register<BusyIndicator, TimeSpan>(nameof(DisplayAfter), TimeSpan.FromMilliseconds(100));
        public static readonly AvaloniaProperty<bool> IsBusyProperty = AvaloniaProperty.Register<BusyIndicator, bool>(nameof(IsBusy));
        public static readonly AvaloniaProperty<string?> TextProperty = AvaloniaProperty.Register<BusyIndicator, string?>(nameof(Text));
        protected static readonly AvaloniaProperty<bool> IsBusyIndicatorVisibleProperty = AvaloniaProperty.Register<BusyIndicator, bool>(nameof(IsBusyIndicatorVisible));

        private readonly DispatcherTimer displayAfterTimer = new();

        public BusyIndicator()
        {
            this.displayAfterTimer.Tick += this.DisplayAfterTimerEnded;
            IsBusyProperty.Changed.Subscribe(this.OnIsBusyChanged);
            this.InitializeComponent();
        }

        public TimeSpan DisplayAfter
        {
            get => this.GetValue<TimeSpan>(DisplayAfterProperty);
            set => this.SetValue(DisplayAfterProperty, value);
        }

        public bool IsBusy
        {
            get => this.GetValue<bool>(IsBusyProperty);
            set => this.SetValue(IsBusyProperty, value);
        }

        public string? Text
        {
            get => this.GetValue<string?>(TextProperty);
            set => this.SetValue(TextProperty, value);
        }

        protected bool IsBusyIndicatorVisible
        {
            get => this.GetValue<bool>(IsBusyIndicatorVisibleProperty);
            set => this.SetValue(IsBusyIndicatorVisibleProperty, value);
        }

        private void DisplayAfterTimerEnded(object? sender, EventArgs e)
        {
            this.displayAfterTimer.Stop();
            this.IsBusyIndicatorVisible = true;
        }

        private void OnIsBusyChanged(AvaloniaPropertyChangedEventArgs eventArgs)
        {
            if (eventArgs.Sender != this)
            {
                return;
            }

            if (eventArgs.NewValue is bool newValue && newValue)
            {
                if (this.DisplayAfter == TimeSpan.Zero)
                {
                    this.IsBusyIndicatorVisible = true;
                }
                else
                {
                    this.displayAfterTimer.Interval = this.DisplayAfter;
                    this.displayAfterTimer.Start();
                }
            }
            else
            {
                this.displayAfterTimer.Stop();
                this.IsBusyIndicatorVisible = false;
            }
        }
    }
}