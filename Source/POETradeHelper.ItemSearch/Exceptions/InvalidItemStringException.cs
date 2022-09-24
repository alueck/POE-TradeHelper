namespace POETradeHelper.ItemSearch.Exceptions
{
    public class InvalidItemStringException : Exception
    {
        public InvalidItemStringException(string itemString) : base($"'{itemString}' was not identified as a valid item string.")
        {
        }
    }
}