using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders.Models;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders
{
    public abstract class ItemStringBuilderBase<T, TNameAndRarity>
        where T : ItemStringBuilderBase<T, TNameAndRarity>
        where TNameAndRarity: NameAndRarityGroupBase, new()
    {
        protected TNameAndRarity NameAndRarityGroup { get; } = new() { Rarity = "Normal" };

        protected bool IsCorrupted { get; private set; }

        protected bool IsIdentified { get; private set; } = true;

        protected bool IsSynthesised { get; private set; }

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

        public T WithCorrupted(bool isCorrupted = true)
        {
            this.IsCorrupted = isCorrupted;
            return (T)this;
        }

        public T WithUnidentified()
        {
            this.IsIdentified = false;
            return (T)this;
        }

        public T WithIdentified(bool isIdentified)
        {
            this.IsIdentified = isIdentified;
            return (T)this;
        }

        public T WithSynthesised(bool synthesised = true)
        {
            this.IsSynthesised = synthesised;
            return (T)this;
        }

        public string[] BuildLines() => this.Build().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        public abstract string Build();
    }
}