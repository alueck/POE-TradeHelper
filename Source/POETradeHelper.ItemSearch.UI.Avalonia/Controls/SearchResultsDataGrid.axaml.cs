using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.Templates;

using POETradeHelper.ItemSearch.UI.Avalonia.Attributes;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Controls;

public partial class SearchResultsDataGrid : UserControl
{
    public static readonly DirectProperty<SearchResultsDataGrid, IEnumerable> ItemsProperty =
        AvaloniaProperty.RegisterDirect<SearchResultsDataGrid, IEnumerable>(
            nameof(Items),
            o => o.Items,
            (o, v) => o.Items = v);

    private IEnumerable items = new AvaloniaList<object>();

    public SearchResultsDataGrid()
    {
        this.InitializeComponent();
        this.ListingsGrid.AutoGeneratingColumn += this.OnDataGridAutoGeneratingColumn;
    }

    public IEnumerable Items
    {
        get => this.items;
        set => this.SetAndRaise(ItemsProperty, ref this.items, value);
    }

    private void OnDataGridAutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        DataGrid dataGrid = (DataGrid)sender!;

        IEnumerator itemsEnumerator = dataGrid.ItemsSource.GetEnumerator();

        if (itemsEnumerator.MoveNext() && itemsEnumerator.Current != null)
        {
            Type itemType = itemsEnumerator.Current.GetType();
            PropertyInfo property = itemType.GetProperty(e.PropertyName)!;

            if (property.PropertyType == typeof(PriceViewModel))
            {
                e.Column = new DataGridTemplateColumn
                {
                    Header = property.GetCustomAttribute<DisplayAttribute>()?.GetShortName() ?? property.Name,
                    CellTemplate = dataGrid.DataTemplates.OfType<DataTemplate>()
                        .Single(d => d.DataType == typeof(PriceViewModel)),
                };
            }

            StyleClassesAttribute? styleClassAttribute = property.GetCustomAttribute<StyleClassesAttribute>();
            if (styleClassAttribute != null)
            {
                e.Column.CellStyleClasses.AddRange(styleClassAttribute.StyleClasses);
            }
        }
    }
}