using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using POETradeHelper.PathOfExileTradeApi.Constants;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.PathOfExileTradeApi.Services.Implementations
{
    [ExcludeFromCodeCoverage]
    public class PseudoStatDataMappingService : IPseudoStatDataMappingService
    {
        private readonly IStatsDataService statsDataService;

        public PseudoStatDataMappingService(IStatsDataService statsDataService)
        {
            this.statsDataService = statsDataService;
        }

        public IEnumerable<StatData> GetPseudoStatData(string itemStatId)
        {
            IEnumerable<StatData?> result = Enumerable.Empty<StatData>();

            var indexOfDot = itemStatId.IndexOf('.');

            if (indexOfDot >= 0)
            {
                var key = itemStatId[(indexOfDot + 1)..];

                if (Mappings.TryGetValue(key, out IReadOnlyCollection<string>? pseudoStatIds))
                {
                    result = pseudoStatIds.Select(this.statsDataService.GetStatDataById);
                }
            }

            return result.OfType<StatData>();
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Readability")]
        private static readonly IReadOnlyDictionary<string, IReadOnlyCollection<string>> Mappings = new Dictionary<string, IReadOnlyCollection<string>>
        {
            [StatId.ColdResistance] = new[] { PseudoStatId.TotalColdResistance, PseudoStatId.TotalElementalResistance, PseudoStatId.TotalResistance },
            [StatId.FireResistance] = new[] { PseudoStatId.TotalFireResistance, PseudoStatId.TotalElementalResistance, PseudoStatId.TotalResistance },
            [StatId.LightningResistance] = new[] { PseudoStatId.TotalLightningResistance, PseudoStatId.TotalElementalResistance, PseudoStatId.TotalResistance },

            [StatId.FireAndColdResistance] = new[] { PseudoStatId.TotalFireResistance, PseudoStatId.TotalColdResistance, PseudoStatId.TotalElementalResistance, PseudoStatId.TotalResistance },
            [StatId.ColdAndLightningResistance] = new[] { PseudoStatId.TotalColdResistance, PseudoStatId.TotalAddsLightningDamage, PseudoStatId.TotalElementalResistance, PseudoStatId.TotalResistance },
            [StatId.FireAndLightningResistance] = new[] { PseudoStatId.TotalFireResistance, PseudoStatId.TotalLightningResistance, PseudoStatId.TotalElementalResistance, PseudoStatId.TotalResistance },

            [StatId.ChaosResistance] = new[] { PseudoStatId.TotalChaosResistance, PseudoStatId.TotalResistance },

            [StatId.MaximumLife] = new[] { PseudoStatId.TotalLife },
            [StatId.LifeRegen] = new[] { PseudoStatId.TotalLifeRegen },
            [StatId.LifeRegenPercent] = new[] { PseudoStatId.TotalPercentLifeRegen },
            [StatId.MaximumMana] = new[] { PseudoStatId.TotalMana },
            [StatId.ManaRegenerationRate] = new[] { PseudoStatId.TotalIncreasedManaRegen },
            [StatId.MaximumEnergyShield] = new[] { PseudoStatId.TotalEnergyShield },
            [StatId.MaximumEnergyShieldLocal] = new[] { PseudoStatId.TotalEnergyShield },
            [StatId.IncreasedEnergyShieldPercentLocal] = new[] { PseudoStatId.TotalIncreasedEnergyShield },

            [StatId.Strength] = new[] { PseudoStatId.TotalStrength },
            [StatId.StrengthAndIntelligence] = new[] { PseudoStatId.TotalStrength, PseudoStatId.TotalIntelligence },
            [StatId.StrengthAndDexterity] = new[] { PseudoStatId.TotalStrength, PseudoStatId.TotalDexterity },
            [StatId.Dexterity] = new[] { PseudoStatId.TotalDexterity },
            [StatId.DexterityAndIntelligence] = new[] { PseudoStatId.TotalDexterity, PseudoStatId.TotalIntelligence },
            [StatId.Intelligence] = new[] { PseudoStatId.TotalIntelligence },
            [StatId.AllAttributes] = new[] { PseudoStatId.TotalAllAttributes },

            [StatId.AttackSpeed] = new[] { PseudoStatId.TotalAttackSpeed },
            [StatId.AttackSpeedLocal] = new[] { PseudoStatId.TotalAttackSpeed },
            [StatId.CastSpeed] = new[] { PseudoStatId.TotalCastSpeed },
            [StatId.AttackAndCastSpeed] = new[] { PseudoStatId.TotalAttackSpeed, PseudoStatId.TotalCastSpeed },
            [StatId.MovementSpeed] = new[] { PseudoStatId.TotalMovementSpeed },

            [StatId.GlobalCriticalStrikeChance] = new[] { PseudoStatId.TotalGlobalCriticalStrikeChance },
            [StatId.CriticalStrikeChanceForSpells] = new[] { PseudoStatId.TotalCriticalStrikeChanceForSpells },
            [StatId.GlobalCriticalStrikeMultiplier] = new[] { PseudoStatId.TotalGlobalCriticalStrikeMultiplier },

            [StatId.AddsPhysicalDamageLocal] = new[] { PseudoStatId.TotalAddsPhysicalDamage, PseudoStatId.TotalAddsDamage },
            [StatId.AddsPhysicalDamage] = new[] { PseudoStatId.TotalAddsPhysicalDamage, PseudoStatId.TotalAddsDamage },
            [StatId.AddsPhysicalDamageToAttacks] = new[] { PseudoStatId.TotalAddsPhysicalDamageToAttacks, PseudoStatId.TotalAddsDamageToAttacks },
            [StatId.AddsPhysicalDamageToSpells] = new[] { PseudoStatId.TotalAddsPhysicalDamageToSpells, PseudoStatId.TotalAddsDamageToSpells },
            [StatId.PhysicalAttackDamageLeechedAsLife] = new[] { PseudoStatId.TotalPhysicalAttackDamageLeechedAsLife },
            [StatId.PhysicalAttackDamageLeechedAsMana] = new[] { PseudoStatId.TotalPhysicalAttackDamageLeechedAsMana },

            [StatId.AddsLightningDamageLocal] = new[] { PseudoStatId.TotalAddsLightningDamage, PseudoStatId.TotalAddsElementalDamage, PseudoStatId.TotalAddsDamage },
            [StatId.AddsLightningDamage] = new[] { PseudoStatId.TotalAddsLightningDamage, PseudoStatId.TotalAddsElementalDamage, PseudoStatId.TotalAddsDamage },
            [StatId.AddsLightningDamageToAttacks] = new[] { PseudoStatId.TotalAddsLightningDamageToAttacks, PseudoStatId.TotalAddsElementalDamageToAttacks, PseudoStatId.TotalAddsDamageToAttacks },
            [StatId.AddsLightningDamageToSpells] = new[] { PseudoStatId.TotalAddsLightningDamageToSpells, PseudoStatId.TotalAddsElementalDamageToSpells, PseudoStatId.TotalAddsDamageToSpells },

            [StatId.AddsColdDamageLocal] = new[] { PseudoStatId.TotalAddsColdDamage, PseudoStatId.TotalAddsElementalDamage, PseudoStatId.TotalAddsDamage },
            [StatId.AddsColdDamage] = new[] { PseudoStatId.TotalAddsColdDamage, PseudoStatId.TotalAddsElementalDamage, PseudoStatId.TotalAddsDamage },
            [StatId.AddsColdDamageToAttacks] = new[] { PseudoStatId.TotalAddsColdDamageToAttacks, PseudoStatId.TotalAddsElementalDamageToAttacks, PseudoStatId.TotalAddsDamageToAttacks },
            [StatId.AddsColdDamageToSpells] = new[] { PseudoStatId.TotalAddsColdDamageToSpells, PseudoStatId.TotalAddsElementalDamageToSpells, PseudoStatId.TotalAddsDamageToSpells },

            [StatId.AddsFireDamageLocal] = new[] { PseudoStatId.TotalAddsFireDamage, PseudoStatId.TotalAddsElementalDamage, PseudoStatId.TotalAddsDamage },
            [StatId.AddsFireDamage] = new[] { PseudoStatId.TotalAddsFireDamage, PseudoStatId.TotalAddsElementalDamage, PseudoStatId.TotalAddsDamage },
            [StatId.AddsFireDamageToAttacks] = new[] { PseudoStatId.TotalAddsFireDamageToAttacks, PseudoStatId.TotalAddsElementalDamageToAttacks, PseudoStatId.TotalAddsDamageToAttacks },
            [StatId.AddsFireDamageToSpells] = new[] { PseudoStatId.TotalAddsFireDamageToSpells, PseudoStatId.TotalAddsElementalDamageToSpells, PseudoStatId.TotalAddsDamageToSpells },

            [StatId.AddsChaosDamageLocal] = new[] { PseudoStatId.TotalAddsChaosDamage, PseudoStatId.TotalAddsDamage },
            [StatId.AddsChaosDamage] = new[] { PseudoStatId.TotalAddsChaosDamage, PseudoStatId.TotalAddsDamage },
            [StatId.AddsChaosDamageToAttacks] = new[] { PseudoStatId.TotalAddsChaosDamageToAttacks, PseudoStatId.TotalAddsDamageToAttacks },
            [StatId.AddsChaosDamageToSpells] = new[] { PseudoStatId.TotalAddsChaosDamageToSpells, PseudoStatId.TotalAddsDamageToSpells },

            [StatId.IncreasedElementalDamage] = new[] { PseudoStatId.TotalIncreasedElementalDamage },
            [StatId.IncreasedLightningDamage] = new[] { PseudoStatId.TotalIncreasedLightningDamage },
            [StatId.IncreasedColdDamage] = new[] { PseudoStatId.TotalIncreasedColdDamage },
            [StatId.IncreasedFireDamage] = new[] { PseudoStatId.TotalIncreasedFireDamage },
            [StatId.IncreasedSpellDamage] = new[] { PseudoStatId.TotalIncreasedSpellDamage },
            [StatId.IncreasedPhysicalDamage] = new[] { PseudoStatId.TotalIncreasedPhysicalDamage },
            [StatId.IncreasedBurningDamage] = new[] { PseudoStatId.TotalIncreasedBurningDamage },

            [StatId.IncreasedLightningDamageWithAttackSkills] = new[] { PseudoStatId.TotalIncreasedLightningDamageWithAttackSkills, PseudoStatId.TotalIncreasedElementalDamageWithAttackSkills },
            [StatId.IncreasedColdDamageWithAttackSkills] = new[] { PseudoStatId.TotalIncreasedColdDamageWithAttackSkills, PseudoStatId.TotalIncreasedElementalDamageWithAttackSkills },
            [StatId.IncreasedFireDamageWithAttackSkills] = new[] { PseudoStatId.TotalIncreasedFireDamageWithAttackSkills, PseudoStatId.TotalIncreasedElementalDamageWithAttackSkills },

            [StatId.IncreasedRarityOfItems] = new[] { PseudoStatId.TotalIncreasedRarity },
        };
    }
}