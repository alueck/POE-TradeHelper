using System;
using System.Diagnostics;

using Avalonia;
using Avalonia.Controls;

namespace POETradeHelper.Common.UI.UserControls
{
    public partial class HyperlinkButton : UserControl
    {
        public static readonly AvaloniaProperty<IHideable?> HideableWindowProperty = AvaloniaProperty.Register<HyperlinkButton, IHideable?>(nameof(HideableWindow));
        public static readonly AvaloniaProperty<Uri> UriProperty = AvaloniaProperty.Register<HyperlinkButton, Uri>(nameof(Uri));

        public HyperlinkButton()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a hideable window that should be hidden if the button is clicked.
        /// </summary>
        public IHideable? HideableWindow
        {
            get => this.GetValue<IHideable?>(HideableWindowProperty);
            set => this.SetValue(HideableWindowProperty, value);
        }

        public Uri Uri
        {
            get => this.GetValue<Uri>(UriProperty);
            set => this.SetValue(UriProperty, value);
        }

        public void OpenHyperlink()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = this.Uri.ToString(),
                UseShellExecute = true,
            });

            this.HideableWindow?.Hide();
        }
    }
}