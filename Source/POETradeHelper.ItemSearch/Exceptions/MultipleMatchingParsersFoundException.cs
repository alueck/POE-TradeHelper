using POETradeHelper.ItemSearch.Contract.Services.Parsers;

namespace POETradeHelper.ItemSearch.Exceptions
{
    public class MultipleMatchingParsersFoundException : ParserException
    {
        private static readonly string ExceptionMessage = $"Multiple parsers ({{0}}) signaled a match for the following item string:{Environment.NewLine}{{1}}";

        public MultipleMatchingParsersFoundException(string itemString, IEnumerable<IItemParser> parsers, Exception? innerException = null) : base(itemString, FormatMessage(itemString, parsers), innerException)
        {
        }

        private static string FormatMessage(string itemString, IEnumerable<IItemParser> parsers)
        {
            return string.Format(ExceptionMessage, string.Join(", ", parsers.Select(p => p.GetType().Name)), itemString);
        }
    }
}