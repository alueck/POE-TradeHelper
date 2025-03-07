﻿using System.Collections.Generic;
using System.Linq;

namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class ItemStats
    {
        public IList<ItemStat> AllStats { get; } = [];

        public IReadOnlyList<ItemStat> ExplicitStats => this.AllStats.Where(s => s.StatCategory == StatCategory.Explicit).ToList();

        public IReadOnlyList<ItemStat> ImplicitStats => this.AllStats.Where(s => s.StatCategory == StatCategory.Implicit).ToList();

        public IReadOnlyList<ItemStat> CraftedStats => this.AllStats.Where(s => s.StatCategory == StatCategory.Crafted).ToList();

        public IReadOnlyList<ItemStat> EnchantedStats => this.AllStats.Where(s => s.StatCategory == StatCategory.Enchant).ToList();

        public IReadOnlyList<ItemStat> FracturedStats => this.AllStats.Where(s => s.StatCategory == StatCategory.Fractured).ToList();

        public IReadOnlyList<ItemStat> MonsterStats => this.AllStats.Where(s => s.StatCategory == StatCategory.Monster).ToList();

        public IReadOnlyList<ItemStat> PseudoStats => this.AllStats.Where(s => s.StatCategory == StatCategory.Pseudo).ToList();
    }
}