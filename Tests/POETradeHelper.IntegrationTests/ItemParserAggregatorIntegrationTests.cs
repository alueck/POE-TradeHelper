using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Autofac;

using AwesomeAssertions;
using AwesomeAssertions.Execution;

using NUnit.Framework;

using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.IntegrationTests;

[Category("Integration")]
public class ItemParserAggregatorIntegrationTests : IntegrationTestBase
{
    private static IPoeTradeApiClient PoeTradeApiClient;
    private static IItemParserAggregator Sut;

    [OneTimeSetUp]
    public static async Task OneTimeSetup()
    {
        var container = Setup(builder => builder.RegisterType<TestPoeTradeApiJsonSerializer>().AsImplementedInterfaces());
        PoeTradeApiClient = container.Resolve<IPoeTradeApiClient>();
        Sut = container.Resolve<IItemParserAggregator>();

        foreach (var initializable in container.Resolve<IEnumerable<IInitializable>>())
        {
            await initializable.OnInitAsync();
        }
    }

    [Test]
    public async Task Parse_MapItem()
    {
        SearchQueryRequest request = new()
        {
            League = "Standard",
            PageSize = 1,
            Query =
            {
                Filters =
                {
                    TypeFilters =
                    {
                        Category = new OptionFilter { Option = "map" },
                        Rarity = new OptionFilter { Option = nameof(ItemRarity.Rare).ToLower() },
                    },
                    MapFilters =
                    {
                        MapTier = new MinMaxFilter { Min = 1 }, // don't want to fetch scarabs
                    },
                    MiscFilters =
                    {
                        Identified = new BoolOptionFilter { Option = true },
                    },
                },
            },
        };

        // excluding monster level explicit stat
        await AssertItemWithStatsParsedCorrectly<MapItem>(request, stat => stat.Id != "explicit.stat_284496119");
    }

    [Test]
    public async Task Parse_JewelItem()
    {
        SearchQueryRequest request = new()
        {
            League = "Standard",
            PageSize = 1,
            Query =
            {
                Filters =
                {
                    TypeFilters =
                    {
                        Category = new OptionFilter { Option = "jewel" },
                        Rarity = new OptionFilter { Option = nameof(ItemRarity.Rare).ToLower() },
                    },
                    MiscFilters =
                    {
                        Identified = new BoolOptionFilter { Option = true },
                    },
                },
            },
        };

        await AssertItemWithStatsParsedCorrectly<JewelItem>(request);
    }

    [Test]
    public async Task Parse_FlaskItem()
    {
        SearchQueryRequest request = new()
        {
            League = "Standard",
            PageSize = 1,
            Query =
            {
                Filters =
                {
                    TypeFilters =
                    {
                        Category = new OptionFilter { Option = "flask" },
                    },
                },
            },
        };

        await AssertItemWithStatsParsedCorrectly<FlaskItem>(request);
    }

    [Test]
    public async Task Parse_Weapon()
    {
        SearchQueryRequest request = new()
        {
            League = "Standard",
            PageSize = 1,
            Query =
            {
                Filters =
                {
                    TypeFilters =
                    {
                        Category = new OptionFilter { Option = "weapon" },
                        Rarity = new OptionFilter { Option = nameof(ItemRarity.Rare).ToLower() },
                    },
                    MiscFilters =
                    {
                        Identified = new BoolOptionFilter { Option = true },
                    },
                },
            },
        };

        await AssertItemWithStatsParsedCorrectly<EquippableItem>(request, additionalAssertions: Assert);

        static void Assert(EquippableItem item)
        {
            item.Category.Should().Be(EquippableItemCategory.Weapon);
        }
    }

    [Test]
    public async Task Parse_Armour()
    {
        SearchQueryRequest request = new()
        {
            League = "Standard",
            PageSize = 1,
            Query =
            {
                Filters =
                {
                    TypeFilters =
                    {
                        Category = new OptionFilter { Option = "armour" },
                        Rarity = new OptionFilter { Option = nameof(ItemRarity.Rare).ToLower() },
                    },
                    MiscFilters =
                    {
                        Identified = new BoolOptionFilter { Option = true },
                    },
                },
            },
        };

        await AssertItemWithStatsParsedCorrectly<EquippableItem>(request, additionalAssertions: Assert);

        static void Assert(EquippableItem item)
        {
            item.Category.Should().Be(EquippableItemCategory.Armour);
        }
    }

    [Test]
    public async Task Parse_Accessory()
    {
        SearchQueryRequest request = new()
        {
            League = "Standard",
            PageSize = 1,
            Query =
            {
                Filters =
                {
                    TypeFilters =
                    {
                        Category = new OptionFilter { Option = "accessory" },
                        Rarity = new OptionFilter { Option = nameof(ItemRarity.Rare).ToLower() },
                    },
                    MiscFilters =
                    {
                        Identified = new BoolOptionFilter { Option = true },
                    },
                },
            },
        };

        await AssertItemWithStatsParsedCorrectly<EquippableItem>(request, additionalAssertions: Assert);

        static void Assert(EquippableItem item)
        {
            item.Category.Should().Be(EquippableItemCategory.Accessory);
        }
    }

    [Test]
    public async Task Parse_GemItem()
    {
        // Don't want to hit the rate limit
        await Task.Delay(2500);

        // Arrange
        SearchQueryRequest request = new()
        {
            League = "Standard",
            PageSize = 1,
            Query =
            {
                Filters =
                {
                    TypeFilters =
                    {
                        Category = new OptionFilter { Option = "gem" },
                    },
                },
            },
        };

        var itemData = await GetItem(request, getStats: false);

        // Act
        Item item = Sut.Parse(itemData.ItemText);

        // Assert
        using var scope = new AssertionScope();
        scope.AppendTracing($"Query URL: {itemData.QueryUrl}\n");
        scope.AppendTracing($"{itemData.ItemText}\n");
        item.Should().BeOfType<GemItem>();
    }

    private static async Task AssertItemWithStatsParsedCorrectly<TItem>(
        SearchQueryRequest request,
        Predicate<ItemStat>? statFilter = null,
        Action<TItem>? additionalAssertions = null)
        where TItem : ItemWithStats
    {
        // Don't want to hit the rate limit
        await Task.Delay(2500);

        var itemData = await GetItem(request, getStats: true);

        // Act
        Item item = Sut.Parse(itemData.ItemText);

        // Assert
        using var scope = new AssertionScope();
        scope.AppendTracing($"Query URL: {itemData.QueryUrl}\n");
        scope.AppendTracing($"{itemData.ItemText}\n");
        var itemWithStats = item.Should().BeOfType<TItem>().Subject;
        itemWithStats.Stats.Should().NotBeNull();
        scope.AppendTracing($"Parsed stats:\n{string.Join('\n', itemWithStats.Stats.AllStats.Select(stat => $"{stat.Id}: {stat.Text}"))}\n");

        itemWithStats.Stats.AllStats
            .Where(stat => stat.StatCategory != StatCategory.Pseudo && (statFilter?.Invoke(stat) ?? true))
            .Select(stat => stat.Id)
            .Order()
            .Should().BeEquivalentTo(itemData.Stats.Select(stat => stat.Id).Order(), cfg => cfg.WithStrictOrdering());

        additionalAssertions?.Invoke(itemWithStats);
    }

    private static async Task<ItemData> GetItem(SearchQueryRequest request, bool getStats)
    {
        var listings = await PoeTradeApiClient.GetListingsAsync(request);

        ItemListing? item = listings.Result.FirstOrDefault()?.Item;

        string? itemText = null;
        if (item?.AdditionalData?.TryGetValue("extended", out var extended) == true
            && extended.TryGetProperty("text", out var text))
        {
            itemText = Encoding.UTF8.GetString(text.GetBytesFromBase64());
        }

        if (item == null || string.IsNullOrWhiteSpace(itemText))
        {
            AssertionChain.GetOrCreate().FailWith("Failed to find item or item text");
        }

        var stats = getStats ? GetStats(item!) : [];
        foreach (var stat in stats.Where(stat => string.Equals(stat.Type, nameof(StatCategory.Implicit), StringComparison.OrdinalIgnoreCase)))
        {
            var index = itemText!.IndexOf(stat.Text, StringComparison.InvariantCulture);
            if (index >= 0)
            {
                itemText = itemText.Insert(index + stat.Text.Length, " (implicit)");
            }
        }

        return new ItemData
        {
            Item = item!,
            ItemText = itemText!,
            Stats = stats,
            QueryUrl = listings.Uri,
        };
    }

    private static IReadOnlyCollection<(string Id, string Text, string Type)> GetStats(ItemListing itemListing)
    {
        var explicitStats = itemListing.AdditionalData.TryGetValue("explicitMods", out var explicitMods)
            ? GetStats(explicitMods.EnumerateArray())
            : [];
        var implicitStats = itemListing.AdditionalData.TryGetValue("implicitMods", out var implicitMods)
            ? GetStats(implicitMods.EnumerateArray())
            : [];
        var enchantStats = itemListing.AdditionalData.TryGetValue("enchantMods", out var enchantMods)
            ? GetStats(enchantMods.EnumerateArray())
            : [];

        return [..explicitStats, ..implicitStats, ..enchantStats];

        static IEnumerable<(string Id, string Text, string Type)> GetStats(JsonElement.ArrayEnumerator enumerator)
        {
            return enumerator.Select(x => (GetId(x), GetText(x), GetType(x)));

            static string GetId(JsonElement jsonElement) => jsonElement.GetProperty("hash")!.GetString()!.Replace("stat.", "");

            static string GetText(JsonElement jsonElement) => jsonElement.GetProperty("description")!.GetString()!;

            static string GetType(JsonElement jsonElement) => jsonElement.GetProperty("domain")!.GetString()!;
        }
    }

    private sealed record ItemData
    {
        public required ItemListing Item { get; init; }

        public required string ItemText { get; init; }

        public required Uri? QueryUrl { get; init; }

        public required IReadOnlyCollection<(string Id, string Text, string Type)> Stats { get; init; }
    }

    private class TestPoeTradeApiJsonSerializer : PoeTradeApiJsonSerializer
    {
        private static readonly JsonSerializerOptions CamelCaseJsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public TestPoeTradeApiJsonSerializer(IJsonSerializerWrapper jsonSerializer) : base(jsonSerializer)
        {
        }

        public override T? Deserialize<T>(string json) where T : default
        {
            return this.JsonSerializer.Deserialize<T>(json, CamelCaseJsonSerializerOptions);
        }
    }
}