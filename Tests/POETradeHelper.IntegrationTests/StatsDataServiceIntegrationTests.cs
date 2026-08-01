using System.Threading.Tasks;

using Autofac;

using AwesomeAssertions;

using NUnit.Framework;

using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.IntegrationTests;

[Category("Integration")]
public class StatsDataServiceIntegrationTests : IntegrationTestBase
{
    private static IStatsDataService Sut;

    [OneTimeSetUp]
    public static async Task OneTimeSetup()
    {
        var container = Setup();
        Sut = container.Resolve<IStatsDataService>();
        await Sut.OnInitAsync();
    }

    [Test]
    public void TryGetStatData_ShouldFindStat()
    {
        IStatData? statData = Sut.TryGetStatData(["Monsters gain an Endurance Charge on Hit"], false, "explicit");

        statData.Should().NotBeNull();
    }
}