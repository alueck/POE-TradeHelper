using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services;
using POETradeHelper.ItemSearch.Exceptions;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class ItemParserAggregatorTests
    {
        private ItemParserAggregator itemParserAggregator;
        private ItemStringBuilder itemStringBuilder;

        private Mock<IItemParser> gemItemParserMock;
        private Mock<IItemParser> currencyItemParserMock;

        [SetUp]
        public void Setup()
        {
            this.gemItemParserMock = new Mock<IItemParser>();
            this.currencyItemParserMock = new Mock<IItemParser>();
            this.itemParserAggregator = new ItemParserAggregator(new[] { this.gemItemParserMock.Object, this.currencyItemParserMock.Object });
            this.itemStringBuilder = new ItemStringBuilder();
        }

        [Test]
        public void CanParseShouldReturnFalseIfNoMatchingParserIsFound()
        {
            bool result = this.itemParserAggregator.CanParse("");

            Assert.IsFalse(result);
        }

        [Test]
        public void CanParseShouldReturnTrueIfMatchingParserIsFound()
        {
            this.currencyItemParserMock.Setup(x => x.CanParse(It.IsAny<string[]>()))
                .Returns(true);

            bool result = this.itemParserAggregator.CanParse("");

            Assert.IsTrue(result);
        }

        [Test]
        public void CanParseShouldReturnFalseIfMoreThanOneMatchingParserIsFound()
        {
            this.gemItemParserMock.Setup(x => x.CanParse(It.IsAny<string[]>()))
                .Returns(true);
            this.currencyItemParserMock.Setup(x => x.CanParse(It.IsAny<string[]>()))
                .Returns(true);

            bool result = this.itemParserAggregator.CanParse("");

            Assert.IsFalse(result);
        }

        [Test]
        public void ParseShouldCallCanParseOnAllItemParser()
        {
            var itemString = this.itemStringBuilder.Build();
            var expectedItemStringLines = this.itemStringBuilder.BuildLines();

            this.gemItemParserMock.Setup(x => x.CanParse(It.IsAny<string[]>()))
                .Returns(true);

            this.itemParserAggregator.Parse(itemString);

            this.gemItemParserMock.Verify(x => x.CanParse(expectedItemStringLines));
            this.currencyItemParserMock.Verify(x => x.CanParse(expectedItemStringLines));
        }

        [Test]
        public void ParseShouldThrowExceptionIfMoreThanOneMatchingParserIsFound()
        {
            this.gemItemParserMock.Setup(x => x.CanParse(It.IsAny<string[]>()))
                .Returns(true);
            this.currencyItemParserMock.Setup(x => x.CanParse(It.IsAny<string[]>()))
                .Returns(true);

            TestDelegate testDelegate = () => this.itemParserAggregator.Parse("");

            Assert.Throws<MultipleMatchingParsersFoundException>(testDelegate);
        }

        [Test]
        public void ParseShouldThrowExceptionIfNoMatchingParserIsFound()
        {
            TestDelegate testDelegate = () => this.itemParserAggregator.Parse("");

            Assert.Throws<NoMatchingParserFoundException>(testDelegate);
        }

        [Test]
        public void ParseShouldCallParseOnMatchingParser()
        {
            var itemString = this.itemStringBuilder.WithRarity("Currency").Build();
            var expectedItemStringLines = this.itemStringBuilder.BuildLines();

            this.currencyItemParserMock.Setup(x => x.CanParse(It.IsAny<string[]>()))
                .Returns(true);

            this.itemParserAggregator.Parse(itemString);

            this.currencyItemParserMock.Verify(x => x.Parse(expectedItemStringLines));
        }

        [Test]
        public void ParseShouldReturnItemFromMatchingParser()
        {
            var item = new CurrencyItem { Name = "Scroll of Wisdom" };

            this.currencyItemParserMock.Setup(x => x.CanParse(It.IsAny<string[]>()))
                .Returns(true);
            this.currencyItemParserMock.Setup(x => x.Parse(It.IsAny<string[]>()))
                .Returns(item);

            Item result = this.itemParserAggregator.Parse("");

            Assert.That(result, Is.EqualTo(item));
        }
    }
}