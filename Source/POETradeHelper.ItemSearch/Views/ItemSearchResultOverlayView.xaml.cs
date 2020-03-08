using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using POETradeHelper.ItemSearch.Attributes;
using System;
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