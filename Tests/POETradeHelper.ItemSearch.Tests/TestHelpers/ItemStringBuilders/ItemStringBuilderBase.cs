using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders.Models;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders
{
    public abstract class ItemStringBuilderBase<T>
        where T : ItemStringBuilderBase<T>
    {
        public NameAndRarityGroup NameAndRarityGroup { get; } = new() { Rarity = "Normal", Type = "TestType" };

        public bool IsCorrupted { get; protected set; }

        public bool IsIdentified { get; protected set; } = true;

        public bool IsSynthesised { get; protected set; }

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