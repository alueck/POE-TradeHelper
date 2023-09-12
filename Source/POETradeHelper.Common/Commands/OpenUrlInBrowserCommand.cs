using System;

using MediatR;

namespace POETradeHelper.Common.Commands
{
    public class OpenUrlInBrowserCommand : IRequest
    {
        public OpenUrlInBrowserCommand(Uri url)
        {
            this.Url = url ?? throw new ArgumentNullException(nameof(url));
        }

        public Uri Url { get; }
    }
}