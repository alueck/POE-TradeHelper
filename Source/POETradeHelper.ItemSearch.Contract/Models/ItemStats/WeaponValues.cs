using System;
using System.Collections.Generic;
using System.Linq;

namespace POETradeHelper.ItemSearch.Contract.Models;

public sealed record WeaponValues
{
    public MinMaxValue? PhysicalDamage { get; set; }

    public IReadOnlyList<MinMaxValue> ElementalDamage { get; set; } = Array.Empty<MinMaxValue>();

    public decimal? AttacksPerSecond { get; set; }

    public decimal? CriticalStrikeChance { get; set; }

    public decimal? AverageDamage => (this.PhysicalDamage?.Average ?? 0) + this.ElementalDamage.Sum(x => x.Average);
}