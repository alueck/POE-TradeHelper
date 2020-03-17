using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using System;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers
{
    public abstract class ItemStringBuilderBase<T>
        where T : ItemStringBuilderBase<T>
    {
        public NameAndRarityGroup NameAndRarityGroup { get; } = new NameAndRarityGroup { Rarity = "Normal", Type = "TestType" };

        public bool IsCorrupted { get; protected set; }
        public bool IsIdentified { get; protected set; } = true;

        public T WithRarity(string rarity)
        {
            this.NameAndRarityGroup.Rarity = rarity;
            return (T)this;
        }

        public T WithRarity(ItemRarity rarity)
        {
            this.NameAndRarityGroup.Rarity = rarity.GetDisplayName();
            return (T)this;
        }

        public T WithName(string name)
        {
            this.NameAndRarityGroup.Name = name;
            return (T)this;
        }

        public T WithType(string type)
        {
            this.NameAndRarityGroup.Type = type;
            return (T)this;
        }

        public T WithCorrupted()
        {
            this.IsCorrupted = true;
            return (T)this;
        }

        public T WithUnidentified()
        {
            this.IsIdentified = false;
            return (T)this;
        }

        public string[] BuildLines()
        {
            return this.Build().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        }

        public abstract string Build();
    }
}