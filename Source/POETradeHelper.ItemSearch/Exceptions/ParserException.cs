using System;

namespace POETradeHelper.ItemSearch.Exceptions
{
    public class ParserException : Exception
    {
        protected string ItemString { get; }

        public ParserException(string itemString) : this(itemString, null, null)
        {
        }

        public ParserException(string itemString, string message) : this(itemString, message, null)
        {
        }

        public ParserException(string itemString, string message, Exception innerException) : base(message, innerException)
        {
            this.ItemString = itemString;
        }
    }
}