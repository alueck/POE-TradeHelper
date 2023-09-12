namespace POETradeHelper.ItemSearch.Exceptions
{
    public class NoMatchingParserFoundException : ParserException
    {
        private static readonly string ExceptionMessage = $"No parser signaled a match for the following item string:{Environment.NewLine}{{0}}";

        public NoMatchingParserFoundException(string itemString, Exception? innerException = null) : base(itemString, string.Format(ExceptionMessage, itemString), innerException)
        {
        }
    }
}