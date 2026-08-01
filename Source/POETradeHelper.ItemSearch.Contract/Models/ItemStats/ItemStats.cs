using System.Collections.Generic;
using System.Linq;

namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class ItemStats
    {
        public IList<ItemStat> AllStats { get; } = [];

        public IReadOnlyList<ItemStat> ExplicitStats => this.AllStats.Where(s => s.StatCategory == StatCategory.Explicit).ToArray();

        public IReadOnlyList<ItemStat> ImplicitStats => this.AllStats.Where(s => s.StatCategory == StatCategory.Implicit).ToArray();

        public IReadOnlyList<ItemStat> CraftedStats => this.AllStats.Where(s => s.StatCategory == StatCategory.Crafted).ToArray();

        public IReadOnlyList<ItemStat> EnchantedStats => this.AllStats.Where(s => s.StatCategory == StatCategory.Enchant).ToArray();

        public IReadOnlyList<ItemStat> FracturedStats => this.AllStats.Where(s => s.StatCategory == StatCategory.Fractured).ToArray();

        public IReadOnlyList<ItemStat> CrucibleStats => this.AllStats.Where(s => s.StatCategory == StatCategory.Crucible).ToArray();

        public IReadOnlyList<ItemStat> ImbuedStats => this.AllStats.Where(s => s.StatCategory == StatCategory.Imbued).ToArray();

        public IReadOnlyList<ItemStat> MonsterStats => this.AllStats.Where(s => s.StatCategory == StatCategory.Monster).ToArray();

        public IReadOnlyList<ItemStat> PseudoStats => this.AllStats.Where(s => s.StatCategory == StatCategory.Pseudo).ToArray();

        public IReadOnlyList<ItemStat> OtherStats => this.AllStats
            .Except(this.ExplicitStats)
            .Except(this.ImplicitStats)
            .Except(this.CraftedStats)
            .Except(this.EnchantedStats)
            .Except(this.FracturedStats)
            .Except(this.CrucibleStats)
            .Except(this.ImbuedStats)
            .Except(this.MonsterStats)
            .Except(this.PseudoStats)
            .ToArray();
    }
}