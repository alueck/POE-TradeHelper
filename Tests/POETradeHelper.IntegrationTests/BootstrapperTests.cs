using System;
using System.Linq;

using FluentAssertions;
using FluentAssertions.Execution;

using NUnit.Framework;

using POETradeHelper.QualityOfLife.Models;
using POETradeHelper.QualityOfLife.Services;

using Splat;

namespace POETradeHelper.IntegrationTests
{
    public class BootstrapperTests : IDisposable
    {
        public BootstrapperTests()
        {
            Bootstrapper.Configure();
        }

        public void Dispose()
        {
            Bootstrapper.Shutdown();
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