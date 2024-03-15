using System.Globalization;
using System.Text.RegularExpressions;

using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Services.Parsers.ItemParsers
{
    public partial class EquippableItemParser : ItemWithStatsParserBase
    {
        private const int NameLineIndex = 2;

        private static readonly IReadOnlyDictionary<string, Action<ArmourValues, int>> ArmourMappings = new Dictionary<string, Action<ArmourValues, int>>
        {
            [Resources.ArmourDescriptor] = (armourValues, value) => armourValues.Armour = value,
            [Resources.BlockChanceDescriptor] = (armourValues, value) => armourValues.BlockChance = value,
            [Resources.EnergyShieldDescriptor] = (armourValues, value) => armourValues.EnergyShield = value,
            [Resources.EvasionRatingDescriptor] = (armourValues, value) => armourValues.EvasionRating = value,
            [Resources.WardDescriptor] = (armourValues, value) => armourValues.Ward = value,
        };

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

            if (equippableItem.Category is EquippableItemCategory.Armour or EquippableItemCategory.Unknown)
            {
                equippableItem.ArmourValues = ParseArmourValues(itemStringLines);
            }

            if (equippableItem.Category is EquippableItemCategory.Weapons or EquippableItemCategory.Unknown)
            {
                equippableItem.WeaponValues = ParseWeaponValues(itemStringLines);
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

        private static ArmourValues ParseArmourValues(string[] itemStringLines)
        {
            ArmourValues result = new();

            foreach (string line in GetArmourOrWeaponValuesLines(itemStringLines))
            {
                Match match = ArmourValuesRegex().Match(line);

                if (match.Success && ArmourMappings.TryGetValue(match.Groups["Descriptor"].Value, out Action<ArmourValues, int>? setter))
                {
                    setter(result, int.Parse(match.Groups["Value"].Value));
                }
            }

            return result;
        }

        private static WeaponValues ParseWeaponValues(string[] itemStringLines)
        {
            WeaponValues result = new();

            foreach (string line in GetArmourOrWeaponValuesLines(itemStringLines))
            {
                Match match = WeaponValuesRegex().Match(line);

                if (!match.Success)
                {
                    continue;
                }

                string descriptor = match.Groups["Descriptor"].Value;

                if (descriptor == Resources.PhysicalDamageDescriptor)
                {
                    result.PhysicalDamage = GetMinMaxValue(match.Groups["Min"].Value, match.Groups["Max"].Value);
                }
                else if (descriptor == Resources.ElementalDamageDescriptor)
                {
                    result.ElementalDamage = match.Groups["Min"].Captures.Select(x => x.Value)
                        .Zip(match.Groups["Max"].Captures.Select(x => x.Value))
                        .Select(x => GetMinMaxValue(x.First, x.Second))
                        .ToArray();
                }
                else if (descriptor == Resources.AttacksPerSecondDescriptor)
                {
                    result.AttacksPerSecond = decimal.Parse(match.Groups["Values"].Value, CultureInfo.InvariantCulture);
                }
                else if (descriptor == Resources.CriticalStrikeChanceDescriptor)
                {
                    result.CriticalStrikeChance = decimal.Parse(match.Groups["Values"].Value, CultureInfo.InvariantCulture);
                }
            }

            return result;
        }

        private static MinMaxValue GetMinMaxValue(string min, string max) =>
            new()
            {
                Min = int.Parse(min),
                Max = int.Parse(max),
            };

        private static IEnumerable<string> GetArmourOrWeaponValuesLines(string[] itemStringLines)
        {
            int index = Array.IndexOf(itemStringLines, ParserConstants.PropertyGroupSeparator);

            return itemStringLines[(index + 1)..].TakeWhile(x => x != ParserConstants.PropertyGroupSeparator);
        }

        [GeneratedRegex(@"(?<Descriptor>[^:]*:)[\D]*(?<Value>\d+)")]
        private static partial Regex ArmourValuesRegex();

        [GeneratedRegex(@"^(?<Descriptor>[^:\r\n]*:) (?:(?<Values>(?:(?<Min>\d+)-(?<Max>\d+))|(?:\d+(?:\.\d+)?))[^,\r\n]*(?:, )?)+$")]
        private static partial Regex WeaponValuesRegex();
    }
}