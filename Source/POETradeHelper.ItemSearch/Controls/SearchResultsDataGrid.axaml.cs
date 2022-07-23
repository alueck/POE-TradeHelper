using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Templates;

using POETradeHelper.ItemSearch.Attributes;
using POETradeHelper.ItemSearch.ViewModels;

namespace POETradeHelper.ItemSearch.Controls;

public partial class SearchResultsDataGrid : UserControl
{
    public static readonly DirectProperty<SearchResultsDataGrid, IEnumerable> ItemsProperty =
        AvaloniaProperty.RegisterDirect<SearchResultsDataGrid, IEnumerable>(nameof(Items), o => o.Items, (o, v) => o.Items = v);

    private IEnumerable items = new AvaloniaList<object>();

    public IEnumerable Items
    {
        get => this.items;
        set => this.SetAndRaise(ItemsProperty, ref this.items, value);
    }
    
    public SearchResultsDataGrid()
    {
        this.InitializeComponent();
    }

    public DataGrid ListingsGrid => this.Get<DataGrid>("ListingsGrid");
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        this.ListingsGrid.AutoGeneratingColumn += this.OnDataGridAutoGeneratingColumn;
    }

    private void OnDataGridAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        DataGrid dataGrid = (DataGrid)sender;

        var itemsEnumerator = dataGrid.Items.GetEnumerator();

        if (itemsEnumerator.MoveNext())
        {
            Type itemType = itemsEnumerator.Current.GetType();
            PropertyInfo property = itemType.GetProperty(e.PropertyName);

            if (property.PropertyType == typeof(PriceViewModel))
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
}