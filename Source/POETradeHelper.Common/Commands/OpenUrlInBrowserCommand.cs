using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace POETradeHelper.Common.Commands;

public class OpenUrlInBrowserCommand : IRequest
{
    public OpenUrlInBrowserCommand(Uri url)
    {
        this.Url = url ?? throw new ArgumentNullException(nameof(url));
    }

    public Uri Url { get; }
}

[ExcludeFromCodeCoverage]
public class OpenUrlInBrowserCommandHandler : IRequestHandler<OpenUrlInBrowserCommand>
{
    public Task Handle(OpenUrlInBrowserCommand request, CancellationToken cancellationToken)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = request.Url.ToString(),
            UseShellExecute = true,
        });

        return Task.CompletedTask;
    }
}