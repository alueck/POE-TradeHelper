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

        public PoeTradeApiCommunicationException(string endpoint, HttpStatusCode statusCode) : base($"Query to '{endpoint}' returned status code {statusCode}")
        {
            this.StatusCode = statusCode;
        }

        public PoeTradeApiCommunicationException(string endpoint, HttpStatusCode statusCode, string response)
            : base($"Query to '{endpoint}' returned status code {statusCode}.")
        {
            this.StatusCode = statusCode;
            this.Response = response;
        }

        public PoeTradeApiCommunicationException(string endpoint, HttpStatusCode statusCode, string request, string response)
            : base($"Query to '{endpoint}' returned status code {statusCode}.")
        {
            this.StatusCode = statusCode;
            this.Request = request;
            this.Response = response;
        }

        public HttpStatusCode StatusCode { get; }

        public string? Request { get; }

        public string? Response { get; }
    }
}