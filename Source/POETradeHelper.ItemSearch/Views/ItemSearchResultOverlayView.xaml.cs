using POETradeHelper.ItemSearch.Attributes;
using POETradeHelper.ItemSearch.ViewModels;
using Avalonia.Controls;
using Avalonia;
using Avalonia.Markup.Xaml;
using System.Reactive;
using ReactiveUI;
using Avalonia.Layout;
using System;
using System.Reflection;
using Avalonia.Input;
using System.Linq;
using Avalonia.Markup.Xaml.Templates;
using System.ComponentModel.DataAnnotations;

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
                PropertyInfo property = itemType.GetProperty(e.PropertyName);

                if (e.PropertyName == nameof(SimpleListingViewModel.Price))
                {
                    e.Column = new DataGridTemplateColumn
                    {
                        Header = property.GetCustomAttribute<DisplayAttribute>()?.GetShortName() ?? property.Name,
                        CellTemplate = dataGrid.DataTemplates.OfType<DataTemplate>().Single(d => d.DataType == typeof(PriceViewModel)),
                    };
                }

                var styleClassAttribute = property.GetCustomAttribute<StyleClassesAttribute>();
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