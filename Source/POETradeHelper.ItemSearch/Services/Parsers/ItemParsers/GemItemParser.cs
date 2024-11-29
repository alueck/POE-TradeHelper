using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Services.Parsers.ItemParsers
{
    public class GemItemParser : ItemParserBase
    {
        private const int NameLineIndex = 2;
        private readonly IItemDataService itemDataService;

        public GemItemParser(IItemDataService itemDataService)
        {
            this.itemDataService = itemDataService;
        }

        public override bool CanParse(string[] itemStringLines) => this.HasRarity(itemStringLines, ItemRarity.Gem);

        protected override Item ParseItem(string[] itemStringLines)
        {
            string? vaalName = Array.Find(itemStringLines, l => l.StartsWith(Resources.VaalKeyword));
            string name = GetName(itemStringLines[NameLineIndex], vaalName);

            ItemType? itemType = this.itemDataService.GetType(name);

            GemItem gemItem = new()
            {
                Name = name,
                Type = itemType?.Type ?? string.Empty,
                TypeDiscriminator = itemType?.Discriminator,
                IsCorrupted = this.IsCorrupted(itemStringLines),
                Quality = GetIntegerFromFirstStringContaining(itemStringLines, Resources.QualityDescriptor),
                Level = GetIntegerFromFirstStringContaining(itemStringLines, Resources.LevelDescriptor),
                ExperiencePercent = GetExperiencePercent(itemStringLines),
                IsVaalVersion = !string.IsNullOrEmpty(vaalName),
            };

            return gemItem;
        }

        private static string GetName(string nameLine, string? vaalName)
        {
            string name;

            if (!string.IsNullOrEmpty(vaalName))
            {
                bool isTransfiguredVaalGem = nameLine.Length > vaalName.Length;
                name = isTransfiguredVaalGem ? $"{vaalName} ({nameLine})" : vaalName;
            }
            else
            {
                name = nameLine;
            }

            return name;
        }

        private static int GetExperiencePercent(string[] itemStringLines)
        {
            int experiencePercent = 0;
            string? experienceLine = Array.Find(itemStringLines, l => l.Contains(Resources.ExperienceDescriptor));

            if (experienceLine != null)
            {
                IEnumerable<decimal> experienceNumbers = experienceLine
                    .Replace(Resources.ExperienceDescriptor, string.Empty)
                    .Replace(".", string.Empty)
                    .Trim()
                    .Split('/')
                    .Select(decimal.Parse)
                    .ToArray();

                experiencePercent = GetIntegralPercent(experienceNumbers.First(), experienceNumbers.Last());
            }

            return experiencePercent;
        }

        private static int GetIntegralPercent(decimal currentOutOfTotal, decimal total)
        {
            decimal percent = currentOutOfTotal / total * 100;

            return (int)percent;
        }
    }
}