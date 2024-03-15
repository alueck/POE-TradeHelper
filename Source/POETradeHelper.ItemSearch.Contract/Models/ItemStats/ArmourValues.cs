namespace POETradeHelper.ItemSearch.Contract.Models;

public sealed record ArmourValues
{
    public int? EvasionRating { get; set; }

    public int? EnergyShield { get; set; }

    public int? Armour { get; set; }

    public int? BlockChance { get; set; }

    public int? Ward { get; set; }
}