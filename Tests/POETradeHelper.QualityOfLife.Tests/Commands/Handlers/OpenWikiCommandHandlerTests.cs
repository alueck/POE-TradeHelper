using System;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using MediatR;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using NUnit.Framework;

using POETradeHelper.Common.Commands;
using POETradeHelper.Common.Contract.Commands;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Queries;
using POETradeHelper.QualityOfLife.Commands.Handlers;
using POETradeHelper.QualityOfLife.Models;
using POETradeHelper.QualityOfLife.Services;

namespace POETradeHelper.QualityOfLife.Tests.Commands.Handlers
{
    public class OpenWikiCommandHandlerTests
    {
        private IMediator mediatorMock;
        private IWikiUrlProvider[] wikiUrlProviderMocks;
        private IOptionsMonitor<WikiOptions> wikiOptionsMock;
        private OpenWikiCommandHandler handler;

        [SetUp]
        public void Setup()
        {
            this.mediatorMock = Substitute.For<IMediator>();
            this.wikiOptionsMock = Substitute.For<IOptionsMonitor<WikiOptions>>();
            this.wikiOptionsMock.CurrentValue
                .Returns(new WikiOptions());
            this.SetupWikiUrlProviderMocks();

            this.handler = new OpenWikiCommandHandler(
                this.mediatorMock,
                this.wikiOptionsMock,
                this.wikiUrlProviderMocks,
                Substitute.For<ILogger<OpenWikiCommandHandler>>());
        }

        [Test]
        public async Task HandleShouldSendGetItemFromCursorQuery()
        {
            var cts = new CancellationTokenSource();

            await this.handler.Handle(new OpenWikiCommand(), cts.Token);

            await this.mediatorMock
                .Received()
                .Send(Arg.Any<GetItemFromCursorQuery>(), cts.Token);
        }

        [TestCase(WikiType.PoeWiki, 0)]
        [TestCase(WikiType.PoeDb, 1)]
        public async Task HandleShouldCallGetUrlOnMatchingWikiUrlProvider(WikiType wikiType, int urlProviderIndex)
        {
            // arrange
            var item = new EquippableItem(ItemRarity.Rare);

            this.wikiOptionsMock
                .CurrentValue
                .Returns(new WikiOptions { Wiki = wikiType });

            this.mediatorMock
                .Send(Arg.Any<GetItemFromCursorQuery>(), Arg.Any<CancellationToken>())
                .Returns(item);

            this.wikiUrlProviderMocks[urlProviderIndex]
                .GetUrl(Arg.Any<Item>())
                .Returns(new Uri("https://www.google.com"));

            // act
            await this.handler.Handle(new OpenWikiCommand(), default);

            // assert
            this.wikiUrlProviderMocks[urlProviderIndex]
                .Received()
                .GetUrl(item);
        }

        [Test]
        public async Task HandleShouldSendOpenUrlInBrowserCommand()
        {
            // arrange
            Uri expectedUrl = new("https://pathofexile.gamepedia.com");
            var item = new EquippableItem(ItemRarity.Rare);

            this.wikiOptionsMock
                .CurrentValue
                .Returns(new WikiOptions { Wiki = WikiType.PoeWiki });

            this.mediatorMock
                .Send(Arg.Any<GetItemFromCursorQuery>(), Arg.Any<CancellationToken>())
                .Returns(item);

            this.wikiUrlProviderMocks[0]
                .GetUrl(Arg.Any<Item>())
                .Returns(expectedUrl);

            // act
            await this.handler.Handle(new OpenWikiCommand(), default);

            // assert
            await this.mediatorMock
                .Received()
                .Send(
                    Arg.Is<OpenUrlInBrowserCommand>(c => c.Url == expectedUrl),
                    Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task HandleShouldCatchExceptions()
        {
            this.mediatorMock
                .Send(Arg.Any<GetItemFromCursorQuery>(), Arg.Any<CancellationToken>())
                .Throws<Exception>();

            Func<Task> action = () => this.handler.Handle(new OpenWikiCommand(), default);

            await action.Should().NotThrowAsync();
        }

        private void SetupWikiUrlProviderMocks()
        {
            var poeWikiWikiUrlProviderMock = Substitute.For<IWikiUrlProvider>();
            poeWikiWikiUrlProviderMock
                .HandledWikiType
                .Returns(WikiType.PoeWiki);

            var poeDbWikiUrlProviderMock = Substitute.For<IWikiUrlProvider>();
            poeDbWikiUrlProviderMock
                .HandledWikiType
                .Returns(WikiType.PoeDb);

            this.wikiUrlProviderMocks = new[]
            {
                poeWikiWikiUrlProviderMock,
                poeDbWikiUrlProviderMock,
            };
        }
    }
}