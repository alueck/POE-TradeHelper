using MediatR;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using POETradeHelper.Common.Commands;
using POETradeHelper.Common.Contract.Commands;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Queries;
using POETradeHelper.QualityOfLife.Models;
using POETradeHelper.QualityOfLife.Services;

namespace POETradeHelper.QualityOfLife.Commands.Handlers
{
    public class OpenWikiCommandHandler : IRequestHandler<OpenWikiCommand>
    {
        private readonly IMediator mediator;
        private readonly IOptionsMonitor<WikiOptions> wikiOptions;
        private readonly IEnumerable<IWikiUrlProvider> wikiUrlProviders;
        private readonly ILogger logger;

        public OpenWikiCommandHandler(
            IMediator mediator,
            IOptionsMonitor<WikiOptions> wikiOptions,
            IEnumerable<IWikiUrlProvider> wikiUrlProviders,
            ILogger<OpenWikiCommandHandler> logger)
        {
            this.mediator = mediator;
            this.wikiOptions = wikiOptions;
            this.wikiUrlProviders = wikiUrlProviders;
            this.logger = logger;
        }

        public async Task<Unit> Handle(OpenWikiCommand request, CancellationToken cancellationToken)
        {
            Item? item = null;
            try
            {
                item = await this.mediator.Send(new GetItemFromCursorQuery(), cancellationToken).ConfigureAwait(false);

                IWikiUrlProvider wikiUrlProvider = this.wikiUrlProviders.Single(x => x.HandledWikiType == this.wikiOptions.CurrentValue.Wiki);
                Uri url = wikiUrlProvider.GetUrl(item);

                await this.mediator.Send(new OpenUrlInBrowserCommand(url), cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "Failed to open wiki for {@Item}", item);
            }

            return Unit.Value;
        }
    }
}