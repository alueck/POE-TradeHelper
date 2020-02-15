using System;
using System.Net;

namespace POETradeHelper.PathOfExileTradeApi.Exceptions
{
    public class PoeTradeApiCommunicationException : Exception
    {
        public PoeTradeApiCommunicationException() : base()
        {
        }

        public PoeTradeApiCommunicationException(string message) : base(message)
        {
        }

        public PoeTradeApiCommunicationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PoeTradeApiCommunicationException(string requestUri, HttpStatusCode httpStatusCode) : base($"Query to '{requestUri}' returned status code {httpStatusCode}")
        {
        }

        public PoeTradeApiCommunicationException(string requestUri, string content, HttpStatusCode httpStatusCode) : base($"Query to '{requestUri}' returned status code {httpStatusCode}. Content: {content}")
        {
        }
    }
}