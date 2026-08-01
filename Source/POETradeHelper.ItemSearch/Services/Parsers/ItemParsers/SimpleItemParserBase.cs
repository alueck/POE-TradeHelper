using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Services.Parsers.ItemParsers
{
    public abstract class SimpleItemParserBase<TItemType> : ItemParserBase
        where TItemType : Item, new()
    {
        private const int NameLineIndex = 2;

        protected override Item ParseItem(string[] itemStringLines) =>
            new TItemType
            {
                Name = itemStringLines[NameLineIndex],
                Type = itemStringLines[NameLineIndex],
            };
    }
}