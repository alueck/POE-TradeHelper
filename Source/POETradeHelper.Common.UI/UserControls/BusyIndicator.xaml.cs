using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System;

namespace POETradeHelper.Common.UI.UserControls
{
    public class BusyIndicator : UserControl
    {
        private readonly DispatcherTimer displayAfterTimer = new DispatcherTimer();

        public BusyIndicator()
        {
            displayAfterTimer.Tick += DisplayAfterTimerEnded;
            this.InitializeComponent();
        }

        private void DisplayAfterTimerEnded(object sender, EventArgs e)
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
            if ((bool)eventArgs.NewValue)
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
            get => this.GetValue(DisplayAfterProperty);
            set => this.SetValue(DisplayAfterProperty, value);
        }

        public static AvaloniaProperty<TimeSpan> DisplayAfterProperty = AvaloniaProperty.Register<BusyIndicator, TimeSpan>(nameof(DisplayAfter), defaultValue: TimeSpan.FromMilliseconds(100));

        public bool IsBusy
        {
            get => this.GetValue(IsBusyProperty);
            set => this.SetValue(IsBusyProperty, value);
        }

        public static AvaloniaProperty<bool> IsBusyProperty = AvaloniaProperty.Register<BusyIndicator, bool>(nameof(IsBusy));

        protected bool IsBusyIndicatorVisible
        {
            get => this.GetValue(IsBusyIndicatorVisibleProperty);
            set => this.SetValue(IsBusyIndicatorVisibleProperty, value);
        }

        protected static AvaloniaProperty<bool> IsBusyIndicatorVisibleProperty = AvaloniaProperty.Register<BusyIndicator, bool>(nameof(IsBusyIndicatorVisible));
    }
}