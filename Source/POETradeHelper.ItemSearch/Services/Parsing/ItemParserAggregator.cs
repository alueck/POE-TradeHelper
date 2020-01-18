﻿using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services;
using POETradeHelper.ItemSearch.Exceptions;
using System;
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
            var itemStringLines = this.GetLines(itemString);
            return parsers.Count(x => x.CanParse(itemStringLines)) == 1;
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