namespace POETradeHelper.ItemSearch.Exceptions
{
    public class NoMatchingParserFoundException : ParserException
    {
        private static readonly string exceptionMessage = $"No parser signaled a match for the following item string:{Environment.NewLine}{{0}}";

        public NoMatchingParserFoundException(string itemString, Exception? innerException = null) : base(itemString, string.Format(exceptionMessage, itemString), innerException)
        {
        }
    }
}