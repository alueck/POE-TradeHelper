using System;
using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class ItemTypeParserTests
    {
        private ItemStringBuilder itemStringBuilder;
        private Mock<IItemDataService> itemDataServiceMock;
        private ItemTypeParser itemTypeParser;

        [SetUp]
        public void Setup()
        {
            this.itemStringBuilder = new ItemStringBuilder();
            this.itemDataServiceMock = new Mock<IItemDataService>();
            this.itemTypeParser = new ItemTypeParser(this.itemDataServiceMock.Object);
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

        [TestCase(ItemRarity.Rare)]
        [TestCase(ItemRarity.Unique)]
        public void ParseTypeShouldCallGetTypeOnItemDataServiceForIdentifiedRareOrUniqueItem(ItemRarity itemRarity)
        {
            const string Type = "Identified item type";
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Item name")
                .WithType(Type)
                .BuildLines();

            this.itemTypeParser.ParseType(itemStringLines, itemRarity, true);

            this.itemDataServiceMock.Verify(x => x.GetType(Type));
        }

        [Test]
        public void ParseTypeShouldCallGetTypeOnItemDataServiceForIdentifiedMagicItem()
        {
            const string Name = "Identified item name";
            string[] itemStringLines = this.itemStringBuilder
                .WithName(Name)
                .BuildLines();

            this.itemTypeParser.ParseType(itemStringLines, ItemRarity.Magic, true);

            this.itemDataServiceMock.Verify(x => x.GetType(Name));
        }

        [TestCase(ItemRarity.Normal)]
        [TestCase(ItemRarity.Magic)]
        [TestCase(ItemRarity.Rare)]
        [TestCase(ItemRarity.Unique)]
        public void ParseTypeShouldCallGetTypeOnItemDataServiceForUnidentifiedItem(ItemRarity itemRarity)
        {
            const string Type = "Unidentified item type";
            string[] itemStringLines = this.itemStringBuilder
                .WithType(Type)
                .BuildLines();

            this.itemTypeParser.ParseType(itemStringLines, itemRarity, false);

            this.itemDataServiceMock.Verify(x => x.GetType(Type));
        }

        [TestCase(ItemRarity.Magic)]
        [TestCase(ItemRarity.Rare)]
        [TestCase(ItemRarity.Unique)]
        public void ParseTypeShouldReturnResultFromItemDataServiceForIdentifiedItems(ItemRarity itemRarity)
        {
            const string expected = "Result from ItemDataService";
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Item name")
                .WithType("Item type")
                .BuildLines();

            this.itemDataServiceMock.Setup(x => x.GetType(It.IsAny<string>()))
                .Returns(expected);

            string result = this.itemTypeParser.ParseType(itemStringLines, itemRarity, true);

            Assert.That(result, Is.EqualTo(expected));
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

            this.itemDataServiceMock.Setup(x => x.GetType(It.IsAny<string>()))
                .Returns(expected);

            string result = this.itemTypeParser.ParseType(itemStringLines, itemRarity, false);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}