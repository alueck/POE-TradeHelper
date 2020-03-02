﻿using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using System.Text.RegularExpressions;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class FlaskItemParser : ItemParserBase
    {
        private const int NameLineIndex = 1;
        private IFlaskItemStatsParser flaskItemStatsParser;

        public FlaskItemParser(IFlaskItemStatsParser flaskItemStatsParser)
        {
            this.flaskItemStatsParser = flaskItemStatsParser;
        }

        public override bool CanParse(string[] itemStringLines)
        {
            int typeLineIndex = this.GetTypeLineIndex(itemStringLines);
            return itemStringLines[typeLineIndex].Contains(Resources.FlaskKeyword);
        }

        public override Item Parse(string[] itemStringLines)
        {
            ItemRarity? rarity = this.GetRarity(itemStringLines);
            var flaskItem = new FlaskItem(rarity.Value)
            {
                Name = itemStringLines[NameLineIndex],
                Type = this.GetFlaskType(itemStringLines),
                Quality = this.GetIntegerFromFirstStringContaining(itemStringLines, Resources.QualityDescriptor),
                IsIdentified = this.IsIdentified(itemStringLines),
            };

            if (flaskItem.IsIdentified)
            {
                flaskItem.Stats = this.flaskItemStatsParser.Parse(itemStringLines);
            }

            return flaskItem;
        }

        private string GetFlaskType(string[] itemStringLines)
        {
            int typeLineIndex = this.GetTypeLineIndex(itemStringLines);

            string type = itemStringLines[typeLineIndex];

            Match match;
            if (type.Contains(Resources.LifeFlaskKeyword) || type.Contains(Resources.ManaFlaskKeyword))
            {
                match = Regex.Match(type, $@"\w+\s{{1}}\w+\s{{1}}{Resources.FlaskKeyword}");
            }
            else
            {
                match = Regex.Match(type, $@"\w+\s{{1}}{Resources.FlaskKeyword}");
            }

            return match.Value;
        }

        private int GetTypeLineIndex(string[] itemStringLines)
        {
            return this.HasRarity(itemStringLines, ItemRarity.Unique)
                ? NameLineIndex + 1
                : NameLineIndex;
        }
    }
}