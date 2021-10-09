using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Templates;
using POETradeHelper.ItemSearch.Attributes;
using POETradeHelper.ItemSearch.ViewModels;

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