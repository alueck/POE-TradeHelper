using NSubstitute;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class ItemTypeParserTests
    {
        private readonly ItemStringBuilder itemStringBuilder;
        private readonly IItemDataService itemDataServiceMock;
        private readonly ItemTypeParser itemTypeParser;

        public ItemTypeParserTests()
        {
            this.itemStringBuilder = new ItemStringBuilder();
            this.itemDataServiceMock = Substitute.For<IItemDataService>();
            this.itemTypeParser = new ItemTypeParser(this.itemDataServiceMock);
        }

        [TestCase(ItemRarity.Currency)]
        [TestCase(ItemRarity.DivinationCard)]
        [TestCase(ItemRarity.Gem)]
        public void ParseTypeShouldThrowArgumentExceptionForUnsupportedItemRarity(ItemRarity itemRarity)
        {
            // act
            TestDelegate action = () => this.itemTypeParser.ParseType(Array.Empty<string>(), itemRarity, false);

            // assert
            Assert.Throws<ArgumentException>(action);
        }

        [Test]
        public void ParseTypeShouldCallGetTypeOnItemDataServiceForIdentifiedMagicItem()
        {
            const string Name = "Identified item name";
            string[] itemStringLines = this.itemStringBuilder
                .WithName(Name)
                .BuildLines();

            this.itemTypeParser.ParseType(itemStringLines, ItemRarity.Magic, true);

            this.itemDataServiceMock
                .Received()
                .GetType(Name);
        }

        [TestCase(ItemRarity.Normal)]
        [TestCase(ItemRarity.Magic)]
        [TestCase(ItemRarity.Rare)]
        public void ParseTypeShouldCallGetTypeOnItemDataServiceForUnidentifiedItem(ItemRarity itemRarity)
        {
            const string Type = "Unidentified item type";
            string[] itemStringLines = this.itemStringBuilder
                .WithType(Type)
                .BuildLines();

            this.itemTypeParser.ParseType(itemStringLines, itemRarity, false);

            this.itemDataServiceMock
                .Received()
                .GetType(Type);
        }

        [TestCase(ItemRarity.Rare)]
        [TestCase(ItemRarity.Unique)]
        public void ParseTypeShouldReturnTypeForIdentifiedRareOrUniqueItem(ItemRarity itemRarity)
        {
            const string expected = "Item type";
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Item name")
                .WithType(expected)
                .BuildLines();

            string result = this.itemTypeParser.ParseType(itemStringLines, itemRarity, true);

            Assert.That(result, Is.EqualTo(expected));
            this.itemDataServiceMock
                .DidNotReceive()
                .GetType(Arg.Any<string>());
        }

        [TestCase(ItemRarity.Normal)]
        [TestCase(ItemRarity.Magic)]
        [TestCase(ItemRarity.Rare)]
        [TestCase(ItemRarity.Unique)]
        public void ParseTypeShouldReturnResultFromItemDataServiceForUnidentifiedItems(ItemRarity itemRarity)
        {
            const string expected = "Result from ItemDataService";
            string[] itemStringLines = this.itemStringBuilder
                .WithType("Item type")
                .BuildLines();

            this.itemDataServiceMock.GetType(Arg.Any<string>())
                .Returns(new ItemType(expected));

            string result = this.itemTypeParser.ParseType(itemStringLines, itemRarity, false);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
