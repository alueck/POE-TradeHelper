namespace POETradeHelper.ItemSearch.Views
{
    public interface IItemSearchResultOverlayView
    {
        bool IsVisible { get; set; }

        void Hide();

        void Show();
    }
}