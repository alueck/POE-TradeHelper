using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.ItemSearch.Services;

namespace POETradeHelper.ItemSearch.Tests.Services
{
    public class SearchItemProviderTests
    {
        private Mock<ICopyCommand> copyCommandMock;
        private Mock<IItemParserAggregator> itemParserAggregatorMock;
        private SearchItemProvider searchItemProvider;

        [SetUp]
        public void Setup()
        {
            this.copyCommandMock = new Mock<ICopyCommand>();
            this.itemParserAggregatorMock = new Mock<IItemParserAggregator>();
            this.searchItemProvider = new SearchItemProvider(this.copyCommandMock.Object, this.itemParserAggregatorMock.Object);
        }

        [Test]
        public async Task GetItemFromUnderCursorAsyncShouldCallExecuteAsyncOnCopyCommand()
        {
            var cancellationToken = new CancellationToken();

            await searchItemProvider.GetItemFromUnderCursorAsync(cancellationToken);

            copyCommandMock.Verify(x => x.ExecuteAsync(cancellationToken));
        }

        [Test]
        public async Task GetItemFromUnderCursorAsyncShouldCallIsParseableOnItemParser()
        {
            string stringToParse = "item string to parse";
            this.copyCommandMock.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(stringToParse);

            await this.searchItemProvider.GetItemFromUnderCursorAsync();

            this.itemParserAggregatorMock.Verify(x => x.IsParseable(stringToParse));
        }

        [Test]
        public async Task GetItemFromUnderCursorAsyncShouldCallParseOnItemParserIfIsParseableReturnsTrue()

        {
            string stringToParse = "item string to parse";
            this.copyCommandMock.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(stringToParse);
            this.itemParserAggregatorMock.Setup(x => x.IsParseable(It.IsAny<string>()))
                .Returns(true);

            await searchItemProvider.GetItemFromUnderCursorAsync();

            this.itemParserAggregatorMock.Verify(x => x.Parse(stringToParse));
        }

        [Test]
        public async Task GetItemFromUnderCursorAsyncShouldNotCallParseOnItemParserIfIsParseableReturnsFalse()

        {
            await searchItemProvider.GetItemFromUnderCursorAsync();

            this.itemParserAggregatorMock.Verify(x => x.Parse(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task GetItemFromUnderCursorAsyncShouldReturnParseResult()
        {
            var expected = new EquippableItem(ItemRarity.Normal) { Name = "TestItem" };
            this.itemParserAggregatorMock.Setup(x => x.IsParseable(It.IsAny<string>()))
                .Returns(true);
            this.itemParserAggregatorMock.Setup(x => x.Parse(It.IsAny<string>()))
                .Returns(expected);

            Item result = await searchItemProvider.GetItemFromUnderCursorAsync();

            Assert.AreEqual(expected, result);
        }
    }
}