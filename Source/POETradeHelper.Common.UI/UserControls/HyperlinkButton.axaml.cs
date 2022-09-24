using System;
using System.Diagnostics;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace POETradeHelper.Common.UI.UserControls
{
    public class HyperlinkButton : UserControl
    {
        public HyperlinkButton()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Allows to set a hideable window that should be hidden if the button is clicked.
        /// </summary>
        public IHideable? HideableWindow
        {
            get => this.GetValue<IHideable?>(HideableWindowProperty);
            set => this.SetValue(HideableWindowProperty, value);
        }

        public static AvaloniaProperty<IHideable?> HideableWindowProperty = AvaloniaProperty.Register<HyperlinkButton, IHideable?>(nameof(HideableWindow));

        public Uri Uri
        {
            get => this.GetValue<Uri>(UriProperty);
            set => SetValue(UriProperty, value);
        }

        public static AvaloniaProperty<Uri> UriProperty = AvaloniaProperty.Register<HyperlinkButton, Uri>(nameof(Uri));

        public void OpenHyperlink()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = this.Uri.ToString(),
                UseShellExecute = true
            });

            this.HideableWindow?.Hide();
        }
    }
}
