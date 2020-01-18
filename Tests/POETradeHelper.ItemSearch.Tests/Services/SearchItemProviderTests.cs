using Moq;
using NUnit.Framework;
using POETradeHelper.Common.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services;
using POETradeHelper.ItemSearch.Services;
using System.Threading.Tasks;

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
            await searchItemProvider.GetItemFromUnderCursorAsync();

            copyCommandMock.Verify(x => x.ExecuteAsync());
        }

        [Test]
        public async Task GetItemFromUnderCursorAsyncShouldCallCanParseOnItemParser()
        {
            string stringToParse = "item string to parse";
            this.copyCommandMock.Setup(x => x.ExecuteAsync())
                .ReturnsAsync(stringToParse);

            await this.searchItemProvider.GetItemFromUnderCursorAsync();

            this.itemParserAggregatorMock.Verify(x => x.CanParse(stringToParse));
        }

        [Test]
        public async Task GetItemFromUnderCursorAsyncShouldCallParseOnItemParserIfCanParseReturnsTrue()

        {
            string stringToParse = "item string to parse";
            this.copyCommandMock.Setup(x => x.ExecuteAsync())
                .ReturnsAsync(stringToParse);
            this.itemParserAggregatorMock.Setup(x => x.CanParse(It.IsAny<string>()))
                .Returns(true);

            await searchItemProvider.GetItemFromUnderCursorAsync();

            this.itemParserAggregatorMock.Verify(x => x.Parse(stringToParse));
        }

        [Test]
        public async Task GetItemFromUnderCursorAsyncShouldNotCallParseOnItemParserIfCanParseReturnsFalse()

        {
            await searchItemProvider.GetItemFromUnderCursorAsync();

            this.itemParserAggregatorMock.Verify(x => x.Parse(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task GetItemFromUnderCursorAsyncShouldReturnParseResult()
        {
            var expected = new EquippableItem(ItemRarity.Normal) { Name = "TestItem" };
            this.itemParserAggregatorMock.Setup(x => x.CanParse(It.IsAny<string>()))
                .Returns(true);
            this.itemParserAggregatorMock.Setup(x => x.Parse(It.IsAny<string>()))
                .Returns(expected);

            Item result = await searchItemProvider.GetItemFromUnderCursorAsync();

            Assert.AreEqual(expected, result);
        }
    }
}