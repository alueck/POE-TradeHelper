using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using ReactiveUI;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Behaviors;

[ExcludeFromCodeCoverage]
public sealed class DataGridInfiniteScrollBehavior : Behavior<DataGrid>
{
    public static readonly AvaloniaProperty<ReactiveCommand<Unit, Unit>> LoadNextPageCommandProperty =
        AvaloniaProperty.Register<DataGridInfiniteScrollBehavior, ReactiveCommand<Unit, Unit>>(nameof(LoadNextPageCommand));

    private bool isLoading;
    private IDisposable? scrollBarValueSubscription;
    private ScrollBar? verticalScrollBar;

    public ReactiveCommand<Unit, Unit>? LoadNextPageCommand
    {
        get => this.GetValue<ReactiveCommand<Unit, Unit>>(LoadNextPageCommandProperty);
        set => this.SetValue(LoadNextPageCommandProperty, value);
    }

    protected override void OnAttached() => this.AssociatedObject!.Loaded += this.OnLoaded;

    protected override void OnDetaching()
    {
        this.AssociatedObject!.Loaded -= this.OnLoaded;
        this.scrollBarValueSubscription?.Dispose();
        this.verticalScrollBar = null;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        this.verticalScrollBar = this.AssociatedObject!.GetVisualDescendants().OfType<ScrollBar>()
            .FirstOrDefault(x => x.Orientation == Orientation.Vertical);

        if (this.verticalScrollBar != null)
        {
            this.scrollBarValueSubscription = this.verticalScrollBar
                .GetObservable(RangeBase.ValueProperty)
                .Skip(1)
                .Subscribe(this.HandleScrollBarValueChanged);
        }
    }

    private void HandleScrollBarValueChanged(double scrollBarValue)
    {
        if (this.LoadNextPageCommand != null
            && !this.isLoading
            && this.verticalScrollBar?.Maximum - scrollBarValue < 60)
        {
            try
            {
                this.isLoading = true;
                this.LoadNextPageCommand.Execute().Subscribe(_ => this.isLoading = false);
            }
            catch
            {
                this.isLoading = false;
            }
        }
    }
}