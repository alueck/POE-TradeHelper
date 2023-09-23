using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Services.Parsers.ItemParsers
{
    public class EquippableItemParser : ItemWithStatsParserBase
    {
        private const int NameLineIndex = 2;
        private readonly ISocketsParser socketsParser;
        private readonly IItemTypeParser itemTypeParser;
        private readonly IItemDataService itemDataService;

        public EquippableItemParser(
            ISocketsParser socketsParser,
            IItemTypeParser itemTypeParser,
            IItemStatsParser<ItemWithStats> itemStatsParser,
            IItemDataService itemDataService)
            : base(itemStatsParser)
        {
            this.socketsParser = socketsParser;
            this.itemTypeParser = itemTypeParser;
            this.itemDataService = itemDataService;
        }

        public override bool CanParse(string[] itemStringLines)
        {
            ItemRarity? itemRarity = GetRarity(itemStringLines);
            return itemRarity is >= ItemRarity.Normal and <= ItemRarity.Unique
                   && HasItemLevel(itemStringLines)
                   && !IsMapOrOrganItem(itemStringLines)
                   && !TypeOrNameContains(itemStringLines, Resources.FlaskKeyword, Resources.JewelKeyword);
        }

        protected override ItemWithStats ParseItemWithoutStats(string[] itemStringLines)
        {
            ItemRarity? itemRarity = GetRarity(itemStringLines);
            EquippableItem equippableItem = new(itemRarity!.Value)
            {
                Name = itemStringLines[NameLineIndex],
                IsIdentified = this.IsIdentified(itemStringLines),
                IsCorrupted = this.IsCorrupted(itemStringLines),
                ItemLevel = GetIntegerFromFirstStringContaining(itemStringLines, Resources.ItemLevelDescriptor),
                Quality = GetIntegerFromFirstStringContaining(itemStringLines, Resources.QualityDescriptor),
                Influence = GetInfluenceType(itemStringLines),
                Sockets = this.GetSockets(itemStringLines),
            };

            equippableItem.Type =
                this.itemTypeParser.ParseType(itemStringLines, equippableItem.Rarity, equippableItem.IsIdentified);
            if (!string.IsNullOrEmpty(equippableItem.Type))
            {
                equippableItem.Category =
                    this.itemDataService.GetCategory(equippableItem.Type)
                        .ParseToEnumByDisplayName<EquippableItemCategory>(StringComparison.OrdinalIgnoreCase) ??
                    EquippableItemCategory.Unknown;
            }

            return equippableItem;
        }

        private static bool HasItemLevel(string[] itemStringLines) =>
            itemStringLines.Any(l => l.Contains(Resources.ItemLevelDescriptor));

        private static bool IsMapOrOrganItem(string[] itemStringLines) =>
            itemStringLines.Any(l => l.Contains(Resources.MapTierDescriptor)
                                     || l.Contains(Resources.OrganItemDescriptor));

        private static bool TypeOrNameContains(string[] itemStringLines, params string[] keywords) =>
            itemStringLines.Skip(2).Take(2).Any(line => keywords.Any(line.Contains));

        private ItemSockets GetSockets(string[] itemStringLines)
        {
            string? socketsLine = itemStringLines.FirstOrDefault(l => l.Contains(Resources.SocketsDescriptor));
            string? socketsString = socketsLine?.Replace(Resources.SocketsDescriptor, string.Empty).Trim();

            return this.socketsParser.Parse(socketsString);
        }

        private static InfluenceType GetInfluenceType(string[] itemStringLines)
        {
            InfluenceType? influenceType = itemStringLines
                .Reverse()
                .Take(4)
                .Select(x => x.ParseToEnumByDisplayName<InfluenceType>())
                .FirstOrDefault(x => x.HasValue);

            return influenceType ?? InfluenceType.None;
        }
    }
}