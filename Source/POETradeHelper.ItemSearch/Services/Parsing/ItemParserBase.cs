using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services;
using System;
using System.Linq;

namespace POETradeHelper.ItemSearch.Services
{
    public abstract class ItemParserBase : IItemParser
    {
        protected bool HasRarity(string itemString, ItemRarity itemRarity)
        {
            string rarityLine = GetLines(itemString).FirstOrDefault(line => line.Contains(Resources.RarityDescriptor));

            string rarity = rarityLine?.Replace(Resources.RarityDescriptor, "").Trim();

            return rarity == itemRarity.GetDisplayName();
        }

        protected string[] GetLines(string itemString)
        {
            return itemString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }

        public abstract Item Parse(string itemString);

        public abstract bool CanParse(string itemString);
    }
}