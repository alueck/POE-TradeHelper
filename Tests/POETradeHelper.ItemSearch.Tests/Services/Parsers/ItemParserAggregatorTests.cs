using NSubstitute;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.ItemSearch.Exceptions;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class ItemParserAggregatorTests
    {
        private readonly ItemParserAggregator itemParserAggregator;
        private readonly ItemStringBuilder itemStringBuilder;

        private readonly IItemParser gemItemParserMock;
        private readonly IItemParser currencyItemParserMock;

        public ItemParserAggregatorTests()
        {
            this.gemItemParserMock = Substitute.For<IItemParser>();
            this.currencyItemParserMock = Substitute.For<IItemParser>();
            this.itemParserAggregator = new ItemParserAggregator(new[] { this.gemItemParserMock, this.currencyItemParserMock });
            this.itemStringBuilder = new ItemStringBuilder();
        }

        [TestCase("incorrect item string", false)]
        [TestCase("Rarity: Magic", true)]
        public void IsParseableShouldReturnResultBasedOnRarityLineExistence(string itemString, bool expected)
        {
            bool result = this.itemParserAggregator.IsParseable(itemString);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldCallCanParseOnAllItemParser()
        {
            var itemString = this.itemStringBuilder.Build();
            var expectedItemStringLines = this.itemStringBuilder.BuildLines();

            this.gemItemParserMock.CanParse(Arg.Any<string[]>())
                .Returns(true);

            this.itemParserAggregator.Parse(itemString);

            this.gemItemParserMock
                .Received()
                .CanParse(Arg.Is<string[]>(s => s.SequenceEqual(expectedItemStringLines)));
            this.currencyItemParserMock
                .Received()
                .CanParse(Arg.Is<string[]>(s => s.SequenceEqual(expectedItemStringLines)));
        }

        [Test]
        public void ParseShouldThrowExceptionIfMoreThanOneMatchingParserIsFound()
        {
            this.gemItemParserMock.CanParse(Arg.Any<string[]>())
                .Returns(true);
            this.currencyItemParserMock.CanParse(Arg.Any<string[]>())
                .Returns(true);

            TestDelegate testDelegate = () => this.itemParserAggregator.Parse(string.Empty);

            Assert.Throws<MultipleMatchingParsersFoundException>(testDelegate);
        }

        [Test]
        public void ParseShouldThrowExceptionIfNoMatchingParserIsFound()
        {
            TestDelegate testDelegate = () => this.itemParserAggregator.Parse(string.Empty);

            Assert.Throws<NoMatchingParserFoundException>(testDelegate);
        }

        [Test]
        public void ParseShouldCallParseOnMatchingParser()
        {
            var itemString = this.itemStringBuilder.WithRarity("Currency").Build();
            var expectedItemStringLines = this.itemStringBuilder.BuildLines();

            this.currencyItemParserMock.CanParse(Arg.Any<string[]>())
                .Returns(true);

            this.itemParserAggregator.Parse(itemString);

            this.currencyItemParserMock
                .Received()
                .Parse(Arg.Is<string[]>(s => s.SequenceEqual(expectedItemStringLines)));
        }

        [Test]
        public void ParseShouldReturnItemFromMatchingParser()
        {
            var item = new CurrencyItem { Name = "Scroll of Wisdom" };

            this.currencyItemParserMock.CanParse(Arg.Any<string[]>())
                .Returns(true);
            this.currencyItemParserMock.Parse(Arg.Any<string[]>())
                .Returns(item);

            Item result = this.itemParserAggregator.Parse(string.Empty);

            Assert.That(result, Is.EqualTo(item));
        }
    }
}