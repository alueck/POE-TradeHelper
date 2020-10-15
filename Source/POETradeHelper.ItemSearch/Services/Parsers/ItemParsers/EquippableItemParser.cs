using System.Linq;
using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class EquippableItemParser : ItemWithStatsParserBase
    {
        private const int NameLineIndex = 1;
        private ISocketsParser socketsParser;
        private readonly IItemTypeParser itemTypeParser;

        public EquippableItemParser(ISocketsParser socketsParser, IItemTypeParser itemTypeParser, IItemStatsParser<ItemWithStats> itemStatsParser)
            : base(itemStatsParser)
        {
            this.socketsParser = socketsParser;
            this.itemTypeParser = itemTypeParser;
        }

        public override bool CanParse(string[] itemStringLines)
        {
            ItemRarity? itemRarity = this.GetRarity(itemStringLines);
            return itemRarity >= ItemRarity.Normal && itemRarity <= ItemRarity.Unique
                && HasItemLevel(itemStringLines)
                && !IsMapOrOrganItem(itemStringLines)
                && !TypeOrNameContains(itemStringLines, Resources.FlaskKeyword, Resources.JewelKeyword);
        }

        private static bool HasItemLevel(string[] itemStringLines)
        {
            return itemStringLines.Any(l => l.Contains(Contract.Properties.Resources.ItemLevelDescriptor));
        }

        private static bool IsMapOrOrganItem(string[] itemStringLines)
        {
            return itemStringLines.Any(l => l.Contains(Contract.Properties.Resources.MapTierDescriptor)
                                         || l.Contains(Contract.Properties.Resources.OrganItemDescriptor));
        }

        private bool TypeOrNameContains(string[] itemStringLines, params string[] keywords)
        {
            return itemStringLines.Skip(1).Take(2).Any(line => keywords.Any(line.Contains));
        }

        protected override ItemWithStats ParseItemWithoutStats(string[] itemStringLines)
        {
            ItemRarity? itemRarity = this.GetRarity(itemStringLines);
            var equippableItem = new EquippableItem(itemRarity.Value)
            {
                Name = itemStringLines[NameLineIndex],
                IsIdentified = this.IsIdentified(itemStringLines),
                IsCorrupted = this.IsCorrupted(itemStringLines),
                ItemLevel = this.GetIntegerFromFirstStringContaining(itemStringLines, Contract.Properties.Resources.ItemLevelDescriptor),
                Quality = this.GetIntegerFromFirstStringContaining(itemStringLines, Contract.Properties.Resources.QualityDescriptor),
                Influence = GetInfluenceType(itemStringLines),
                Sockets = this.GetSockets(itemStringLines)
            };

            equippableItem.Type = this.itemTypeParser.ParseType(itemStringLines, equippableItem.Rarity, equippableItem.IsIdentified);

            return equippableItem;
        }

        private ItemSockets GetSockets(string[] itemStringLines)
        {
            string socketsLine = itemStringLines.FirstOrDefault(l => l.Contains(Contract.Properties.Resources.SocketsDescriptor));
            string socketsString = socketsLine?.Replace(Contract.Properties.Resources.SocketsDescriptor, "").Trim();

            ItemSockets itemSockets = this.socketsParser.Parse(socketsString);

            return itemSockets;
        }

        private InfluenceType GetInfluenceType(string[] itemStringLines)
        {
            InfluenceType? influenceType = itemStringLines.Last().ParseToEnumByDisplayName<InfluenceType>();

            return influenceType ?? InfluenceType.None;
        }
    }
}