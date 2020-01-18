using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public abstract class SimpleItemParserBase<TItemType> : ItemParserBase
        where TItemType : Item, new()
    {
        private const int NameLineIndex = 1;

        public override abstract bool CanParse(string[] itemStringLines);

        public override Item Parse(string[] itemStringLines)
        {
            return new TItemType
            {
                Name = itemStringLines[NameLineIndex],
                Type = itemStringLines[NameLineIndex]
            };
        }
    }
}