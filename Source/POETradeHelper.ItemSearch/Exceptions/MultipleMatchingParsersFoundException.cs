using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace POETradeHelper.ItemSearch.Exceptions
{
    public class MultipleMatchingParsersFoundException : ParserException
    {
        private static readonly string exceptionMessage = $"Multiple parsers ({{0}}) signaled a match for the following item string:{Environment.NewLine}{{1}}";

        public MultipleMatchingParsersFoundException(string itemString, IEnumerable<IItemParser> parsers) : this(itemString, parsers, null)
        {
        }

        public MultipleMatchingParsersFoundException(string itemString, IEnumerable<IItemParser> parsers, Exception innerException) : base(itemString, FormatMessage(itemString, parsers), innerException)
        {
        }

        private static string FormatMessage(string itemString, IEnumerable<IItemParser> parsers)
        {
            return string.Format(exceptionMessage, string.Join(", ", parsers.Select(p => p.GetType().Name)), itemString);
        }
    }
}