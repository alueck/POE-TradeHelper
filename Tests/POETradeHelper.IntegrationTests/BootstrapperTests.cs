using System;
using System.Linq;

using AwesomeAssertions;
using AwesomeAssertions.Execution;

using NUnit.Framework;

using POETradeHelper.QualityOfLife.Models;
using POETradeHelper.QualityOfLife.Services;

using Splat;

namespace POETradeHelper.IntegrationTests
{
    public class BootstrapperTests : IntegrationTestBase
    {
        [OneTimeSetUp]
        public static void OneTimeSetup()
        {
            Setup();
        }

        [Test]
        public void AllPoeWikiUrlProvidersRegistered()
        {
            var wikiUrlProviders = Locator.Current.GetServices<IWikiUrlProvider>().ToArray();

            using AssertionScope scope = new();

            foreach (var wikiType in Enum.GetValues<WikiType>())
            {
                wikiUrlProviders.Should().Contain(x => x.HandledWikiType == wikiType);
            }
        }
    }
}