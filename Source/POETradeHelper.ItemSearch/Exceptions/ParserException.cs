namespace POETradeHelper.ItemSearch.Exceptions
{
    public class ParserException : Exception
    {
        public ParserException(string itemString) : this(itemString, null, null)
        {
        }

        public ParserException(string itemString, string message) : this(itemString, message, null)
        {
        }

        public ParserException(string itemString, string? message, Exception? innerException) : base(message, innerException)
        {
            this.ItemString = itemString;
        }

        public ParserException(string[] itemStringLines) : this(itemStringLines, null, null)
        {
        }

        public ParserException(string[] itemStringLines, string message) : this(itemStringLines, message, null)
        {
        }

        public ParserException(string[] itemStringLines, string? message, Exception? innerException) : base(message, innerException)
        {
            this.ItemString = string.Join(Environment.NewLine, itemStringLines);
        }

        protected string ItemString { get; }
    }
}