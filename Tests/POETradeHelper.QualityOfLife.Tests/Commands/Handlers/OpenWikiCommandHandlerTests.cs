using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using POETradeHelper.Common.Commands;
using POETradeHelper.Common.Contract.Commands;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Queries;
using POETradeHelper.QualityOfLife.Commands;
using POETradeHelper.QualityOfLife.Commands.Handlers;
using POETradeHelper.QualityOfLife.Models;
using POETradeHelper.QualityOfLife.Services;

namespace POETradeHelper.QualityOfLife.Tests.Commands.Handlers
{
    public class OpenWikiCommandHandlerTests
    {
        private Mock<IMediator> mediatorMock;
        private Mock<IWikiUrlProvider>[] wikiUrlProviderMocks;
        private Mock<IOptionsMonitor<WikiOptions>> wikiOptionsMock;
        private OpenWikiCommandHandler handler;

        [SetUp]
        public void Setup()
        {
            this.mediatorMock = new Mock<IMediator>();
            this.wikiOptionsMock = new Mock<IOptionsMonitor<WikiOptions>>();
            this.wikiOptionsMock.Setup(x => x.CurrentValue)
                .Returns(new WikiOptions());
            SetupWikiUrlProviderMocks();
            
            this.handler = new OpenWikiCommandHandler(
                this.mediatorMock.Object,
                this.wikiOptionsMock.Object,
                this.wikiUrlProviderMocks.Select(x => x.Object),
                Mock.Of<ILogger<OpenWikiCommandHandler>>());
        }

        private void SetupWikiUrlProviderMocks()
        {
            var poeWikiWikiUrlProviderMock = new Mock<IWikiUrlProvider>();
            poeWikiWikiUrlProviderMock
                .SetupGet(x => x.HandledWikiType)
                .Returns(WikiType.PoeWiki);

            var poeDbWikiUrlProviderMock = new Mock<IWikiUrlProvider>();
            poeDbWikiUrlProviderMock
                .SetupGet(x => x.HandledWikiType)
                .Returns(WikiType.PoeDb);

            this.wikiUrlProviderMocks = new[]
            {
                poeWikiWikiUrlProviderMock,
                poeDbWikiUrlProviderMock
            };
        }

        [Test]
        public async Task HandleShouldSendGetItemFromCursorQuery()
        {
            var cts = new CancellationTokenSource();
            
            await this.handler.Handle(new OpenWikiCommand(), cts.Token);
            
            this.mediatorMock.Verify(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), cts.Token));
        }

        [TestCase(WikiType.PoeWiki, 0)]
        [TestCase(WikiType.PoeDb, 1)]
        public async Task HandleShouldCallGetUrlOnMatchingWikiUrlProvider(WikiType wikiType, int urlProviderIndex)
        {
            // arrange
            var item = new EquippableItem(ItemRarity.Rare);

            this.wikiOptionsMock
                .Setup(x => x.CurrentValue)
                .Returns(new WikiOptions { Wiki = wikiType });
            
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            this.wikiUrlProviderMocks[urlProviderIndex]
                .Setup(x => x.GetUrl(It.IsAny<Item>()))
                .Returns(new Uri("https://www.google.com"));
            
            // act
            await this.handler.Handle(new OpenWikiCommand(), default);
            
            // assert
            this.wikiUrlProviderMocks[urlProviderIndex].Verify(x => x.GetUrl(item));
        }

        [Test]
        public async Task HandleShouldNotCallGetUrlIfItemIsNull()
        {
            await this.handler.Handle(new OpenWikiCommand(), default);
            
            this.wikiUrlProviderMocks[0].Verify(x => x.GetUrl(It.IsAny<Item>()), Times.Never);
            this.wikiUrlProviderMocks[1].Verify(x => x.GetUrl(It.IsAny<Item>()), Times.Never);
        }

        [Test]
        public async Task HandleShouldSendOpenUrlInBrowserCommand()
        {
            // arrange
            Uri expectedUrl = new Uri("https://pathofexile.gamepedia.com");
            var item = new EquippableItem(ItemRarity.Rare);

            this.wikiOptionsMock
                .Setup(x => x.CurrentValue)
                .Returns(new WikiOptions { Wiki = WikiType.PoeWiki });
            
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            this.wikiUrlProviderMocks[0]
                .Setup(x => x.GetUrl(It.IsAny<Item>()))
                .Returns(expectedUrl);
            
            // act
            await this.handler.Handle(new OpenWikiCommand(), default);
            
            // assert
            this.mediatorMock.Verify(x => x.Send(
                It.Is<OpenUrlInBrowserCommand>(c => c.Url == expectedUrl),
                It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task HandleShouldCatchExceptions()
        {
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .Throws<Exception>();
            
            await this.handler.Handle(new OpenWikiCommand(), default);
        }
    }
}