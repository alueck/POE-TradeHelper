using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public abstract class SimpleItemParserBase<TItemType> : ItemParserBase
        where TItemType : Item, new()
    {
        private const int NameLineIndex = 2;

        public override abstract bool CanParse(string[] itemStringLines);

        protected override Item ParseItem(string[] itemStringLines)
        {
            return new TItemType
            {
                Name = itemStringLines[NameLineIndex],
                Type = itemStringLines[NameLineIndex]
            };
        }
    }
}