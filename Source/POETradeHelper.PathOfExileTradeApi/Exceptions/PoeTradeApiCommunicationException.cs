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

        public PoeTradeApiCommunicationException(string endpoint, HttpStatusCode httpStatusCode) : base($"Query to endpoint {endpoint} of Path of Exile trade API returned status code {httpStatusCode}")
        {
        }
    }
}