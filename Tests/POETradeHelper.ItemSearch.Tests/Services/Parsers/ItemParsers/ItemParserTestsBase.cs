using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Parsers.ItemParsers;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers.ItemParsers
{
    public abstract class ItemParserTestsBase
    {
        protected ItemParserBase ItemParser { get; set; } = null!;

        [Test]
        public void ParseShouldSetItemText()
        {
            // arrange
            string[] itemStringLines = this.GetValidItemStringLines();
            string expected = string.Join(Environment.NewLine, itemStringLines);

            // act
            Item result = this.ItemParser.Parse(itemStringLines);

            // assert
            Assert.That(result.ItemText, Is.EqualTo(expected));
        }

        protected abstract string[] GetValidItemStringLines();
    }
}