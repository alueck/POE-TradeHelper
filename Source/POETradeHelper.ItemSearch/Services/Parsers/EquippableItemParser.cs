using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using System.Linq;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class EquippableItemParser : ItemParserBase
    {
        private const int NameLineIndex = 1;
        private ISocketsParser socketsParser;

        public EquippableItemParser(ISocketsParser socketsParser)
        {
            this.socketsParser = socketsParser;
        }

        public override bool CanParse(string[] itemStringLines)
        {
            ItemRarity? itemRarity = this.GetRarity(itemStringLines);
            return itemRarity >= ItemRarity.Normal && itemRarity <= ItemRarity.Unique
                && HasItemLevel(itemStringLines)
                && !IsMapOrOrganItem(itemStringLines);
        }

        private static bool HasItemLevel(string[] itemStringLines)
        {
            return itemStringLines.Any(l => l.Contains(Contract.Properties.Resources.ItemLevelDescriptor));
        }

        private static bool IsMapOrOrganItem(string[] itemStringLines)
        {
            return itemStringLines.Any(l => l.Contains(Contract.Properties.Resources.MapTierDescriptor)
                                         || l.Contains(Contract.Properties.Resources.MetatmorphOrganDescriptor));
        }

        public override Item Parse(string[] itemStringLines)
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

            SetNameAndType(equippableItem, itemStringLines);

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

        private static void SetNameAndType(EquippableItem equippableItem, string[] itemStringLines)
        {
            int typeLineIndex = HasName(equippableItem) ? NameLineIndex + 1 : NameLineIndex;

            equippableItem.Name = itemStringLines[NameLineIndex];
            equippableItem.Type = GetTypeWithoutPrefixes(itemStringLines[typeLineIndex]);
        }

        private static bool HasName(EquippableItem equippableItem)
        {
            return equippableItem.IsIdentified && equippableItem.Rarity != ItemRarity.Normal;
        }

        private static string GetTypeWithoutPrefixes(string type)
        {
            return type.Replace(Contract.Properties.Resources.SuperiorPrefix, "").Trim();
        }
    }
}