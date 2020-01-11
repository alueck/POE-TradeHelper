using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services;
using POETradeHelper.ItemSearch.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace POETradeHelper.ItemSearch.Services
{
    public class ItemParserAggregator : IItemParserAggregator
    {
        public const string PropertyGroupSeparator = "--------";

        private readonly IEnumerable<IItemParser> parsers;

        public ItemParserAggregator(IEnumerable<IItemParser> parsers)
        {
            this.parsers = parsers;
        }

        public bool CanParse(string itemString)
        {
            return parsers.Count(x => x.CanParse(itemString)) == 1;
        }

        public Item Parse(string itemString)
        {
            IList<IItemParser> parsers = this.parsers.Where(x => x.CanParse(itemString)).ToList();

            VerifyMatchingParsers(itemString, parsers);

            Item item = parsers.First().Parse(itemString);

            return item;
        }

        private static void VerifyMatchingParsers(string itemString, IList<IItemParser> parsers)
        {
            if (parsers.Count == 0)
            {
                throw new NoMatchingParserFoundException(itemString);
            }
            else if (parsers.Count > 1)
            {
                throw new MultipleMatchingParsersFoundException(itemString, parsers);
            }
        }
    }
}