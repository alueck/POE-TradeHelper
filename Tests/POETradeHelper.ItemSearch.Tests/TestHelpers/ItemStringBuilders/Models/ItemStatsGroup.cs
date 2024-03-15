using System.Globalization;
using System.Text;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders.Models;

public class ItemStatsGroup : ItemStatsGroupBase
{
    private readonly List<string> armourValues = [];
    private readonly List<string> weaponValues = [];

    public ItemStatsGroup WithEvasionRating(int value)
    {
        this.armourValues.Add($"{Resources.EvasionRatingDescriptor} {value}");
        return this;
    }

    public ItemStatsGroup WithEnergyShield(int value)
    {
        this.armourValues.Add($"{Resources.EnergyShieldDescriptor} {value}");
        return this;
    }

    public ItemStatsGroup WithArmour(int value)
    {
        this.armourValues.Add($"{Resources.ArmourDescriptor} {value}");
        return this;
    }

    public ItemStatsGroup WithBlockChance(int value)
    {
        this.armourValues.Add($"{Resources.BlockChanceDescriptor} {value}%");
        return this;
    }

    public ItemStatsGroup WithWard(int value)
    {
        this.armourValues.Add($"{Resources.WardDescriptor} {value}");
        return this;
    }

    public ItemStatsGroup WithPhysicalDamage(MinMaxValue value)
    {
        this.weaponValues.Add($"{Resources.PhysicalDamageDescriptor} {value.Min}-{value.Max}");
        return this;
    }

    public ItemStatsGroup WithElementalDamage(IEnumerable<MinMaxValue> values)
    {
        this.weaponValues.Add($"{Resources.ElementalDamageDescriptor} {string.Join(", ", values.Select(v => $"{v.Min}-{v.Max}"))}");
        return this;
    }

    public ItemStatsGroup WithAttacksPerSecond(decimal value)
    {
        this.weaponValues.Add($"{Resources.AttacksPerSecondDescriptor} {value.ToString("N2", CultureInfo.InvariantCulture)}");
        return this;
    }

    public ItemStatsGroup WithCriticalStrikeChance(decimal value)
    {
        this.weaponValues.Add($"{Resources.CriticalStrikeChanceDescriptor} {value.ToString("N2", CultureInfo.InvariantCulture)}");
        return this;
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new();

        this.armourValues.ForEach(x => stringBuilder.AppendLine(x));
        this.weaponValues.ForEach(x => stringBuilder.AppendLine(x));

        stringBuilder
            .AppendLine($"{Resources.QualityDescriptor} +{this.Quality}% {Resources.AugmentedDescriptor}", () => this.Quality > 0);

        return stringBuilder.ToString();
    }
}