using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

namespace POETradeHelper.Common.Commands.Handlers
{
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
}