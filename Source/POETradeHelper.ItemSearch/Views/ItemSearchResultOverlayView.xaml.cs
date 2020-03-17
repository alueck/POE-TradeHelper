using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using POETradeHelper.ItemSearch.Attributes;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reflection;

namespace POETradeHelper.ItemSearch.Views
{
    public class ItemSearchResultOverlayView : Window, IItemSearchResultOverlayView
    {
        public ItemSearchResultOverlayView()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            var dataGrid = this.Get<DataGrid>("ListingsGrid");

            dataGrid.AutoGeneratingColumn += OnDataGridAutoGeneratingColumn;

            var expander = this.Get<Expander>("expander");
            expander.ContentTransition = null;

            expander.ObservableForProperty(x => x.IsExpanded).Subscribe(new AnonymousObserver<IObservedChange<Expander, bool>>(x => OnExpandedChanged(x.Value)));
        }

        private void OnExpandedChanged(bool isExpanded)
        {
            var expander = this.Get<Expander>("expander");

            if (isExpanded)
            {
                if (expander.Content is ILayoutable layoutable)
                {
                    layoutable.Measure(Size.Infinity);

                    expander.Measure(Size.Infinity);
                    this.Height = this.Height + Math.Max(layoutable.DesiredSize.Height, expander.DesiredSize.Height);
                    this.Width = Math.Max(this.Width, Math.Max(layoutable.DesiredSize.Width, expander.DesiredSize.Width));
                }
            }
            else
            {
                this.Height = 220;
                this.Width = 400;
            }
            this.SizeToContent = SizeToContent.Width;
        }

        private void OnDataGridAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;

            var itemsEnumerator = dataGrid.Items.GetEnumerator();

            if (itemsEnumerator.MoveNext())
            {
                Type itemType = itemsEnumerator.Current.GetType();

                var styleClassAttribute = itemType.GetProperty(e.PropertyName).GetCustomAttribute<StyleClassesAttribute>();
                if (styleClassAttribute != null)
                {
                    e.Column.CellStyleClasses.AddRange(styleClassAttribute.StyleClasses);
                }
            }
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            BeginMoveDrag(e);
            base.OnPointerPressed(e);
        }
    }
}