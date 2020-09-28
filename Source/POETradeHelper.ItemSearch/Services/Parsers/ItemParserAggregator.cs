using System;
using System.Collections.Generic;
using System.Linq;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.ItemSearch.Exceptions;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class ItemParserAggregator : IItemParserAggregator
    {
        private readonly IEnumerable<IItemParser> parsers;

        public ItemParserAggregator(IEnumerable<IItemParser> parsers)
        {
            this.parsers = parsers;
        }

        public bool IsParseable(string itemString)
        {
            var itemStringLines = this.GetLines(itemString);

            return itemStringLines.Any(l => l.StartsWith(Resources.RarityDescriptor));
        }

        public Item Parse(string itemString)
        {
            var itemStringLines = this.GetLines(itemString);

            IList<IItemParser> parsers = this.parsers.Where(x => x.CanParse(itemStringLines)).ToList();

            VerifyMatchingParsers(itemString, parsers);

            Item item = parsers.First().Parse(itemStringLines);

            return item;
        }

        private string[] GetLines(string itemString)
        {
            return itemString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
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