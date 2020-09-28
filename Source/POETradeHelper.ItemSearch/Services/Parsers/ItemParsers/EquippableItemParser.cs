using System.Linq;
using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class EquippableItemParser : ItemWithStatsParserBase
    {
        private const int NameLineIndex = 1;
        private ISocketsParser socketsParser;
        private readonly IItemDataService itemDataService;

        public EquippableItemParser(ISocketsParser socketsParser, IItemDataService itemDataService, IItemStatsParser<ItemWithStats> itemStatsParser) : base(itemStatsParser)
        {
            this.socketsParser = socketsParser;
            this.itemDataService = itemDataService;
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
                IsIdentified = this.IsIdentified(itemStringLines),
                IsCorrupted = this.IsCorrupted(itemStringLines),
                ItemLevel = this.GetIntegerFromFirstStringContaining(itemStringLines, Contract.Properties.Resources.ItemLevelDescriptor),
                Quality = this.GetIntegerFromFirstStringContaining(itemStringLines, Contract.Properties.Resources.QualityDescriptor),
                Influence = GetInfluenceType(itemStringLines),
                Sockets = this.GetSockets(itemStringLines)
            };

            this.SetNameAndType(equippableItem, itemStringLines);

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

        private void SetNameAndType(EquippableItem equippableItem, string[] itemStringLines)
        {
            int typeLineIndex = HasName(equippableItem) ? NameLineIndex + 1 : NameLineIndex;

            equippableItem.Name = itemStringLines[NameLineIndex];
            equippableItem.Type = this.GetTypeWithoutPrefixes(equippableItem, itemStringLines[typeLineIndex]);
        }

        private static bool HasName(EquippableItem equippableItem)
        {
            return equippableItem.IsIdentified && equippableItem.Rarity > ItemRarity.Magic;
        }

        private string GetTypeWithoutPrefixes(EquippableItem item, string type)
        {
            if (item.IsIdentified && item.Rarity == ItemRarity.Magic)
            {
                type = this.itemDataService.GetType(type);
            }
            else
            {
                type = type
                        .Replace(Contract.Properties.Resources.SuperiorPrefix, "")
                        .Replace(Contract.Properties.Resources.SynthesisedKeyword, "")
                        .Trim();
            }

            return type;
        }
    }
}