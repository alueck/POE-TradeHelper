using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace POETradeHelper.Common.Commands.Handlers
{
    [ExcludeFromCodeCoverage]
    public class OpenUrlInBrowserCommandHandler : AsyncRequestHandler<OpenUrlInBrowserCommand>
    {
        protected override Task Handle(OpenUrlInBrowserCommand request, CancellationToken cancellationToken)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = request.Url.ToString(),
                UseShellExecute = true
            });
            
            return Task.CompletedTask;
        }
    }
}