using System;
using System.Net;

namespace POETradeHelper.PathOfExileTradeApi.Exceptions
{
    public class PoeTradeApiCommunicationException : Exception
    {
        public PoeTradeApiCommunicationException()
        {
        }

        public PoeTradeApiCommunicationException(string message) : base(message)
        {
        }

        public PoeTradeApiCommunicationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PoeTradeApiCommunicationException(string endpoint, HttpStatusCode httpStatusCode) : base($"Query to '{endpoint}' returned status code {httpStatusCode}")
        {
        }

        public PoeTradeApiCommunicationException(string endpoint, string content, HttpStatusCode httpStatusCode) : base($"Query to '{endpoint}' returned status code {httpStatusCode}. Content: {content}")
        {
        }
    }
}