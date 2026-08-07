using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

using ReactiveUI;
using ReactiveUI.Primitives;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Behaviors;

[ExcludeFromCodeCoverage]
public sealed class DataGridInfiniteScrollBehavior : Behavior<DataGrid>
{
    public static readonly AvaloniaProperty<ReactiveCommand<RxVoid, RxVoid>> LoadNextPageCommandProperty =
        AvaloniaProperty.Register<DataGridInfiniteScrollBehavior, ReactiveCommand<RxVoid, RxVoid>>(nameof(LoadNextPageCommand));

    private bool isLoading;
    private IDisposable? scrollBarValueSubscription;
    private ScrollBar? verticalScrollBar;

    public ReactiveCommand<RxVoid, RxVoid>? LoadNextPageCommand
    {
        get => this.GetValue<ReactiveCommand<RxVoid, RxVoid>>(LoadNextPageCommandProperty);
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