using System;
using System.Linq;

using NUnit.Framework;

using POETradeHelper.Common.Contract;
using POETradeHelper.QualityOfLife.Models;
using POETradeHelper.QualityOfLife.Services;
using Splat;

namespace POETradeHelper.IntegrationTests
{
    public class BootstrapperTests
    {
        [SetUp]
        public void Setup()
        {
            Bootstrapper.Configure();
        }
        
        [Test]
        public void AllPoeWikiUrlProvidersRegistered()
        {
            var wikiUrlProviders = Locator.Current.GetServices<IWikiUrlProvider>();

            Assert.Multiple(() =>
            {
                foreach (var wikiType in Enum.GetValues<WikiType>())
                {
                    Assert.That(
                        wikiUrlProviders,
                        Has.One.Matches<IWikiUrlProvider>(x => x.HandledWikiType == wikiType),
                        () => $"No {nameof(IWikiUrlProvider)} registered/implemented for wiki type '{wikiType}'");
                }
            });
        }

        [TearDown]
        public void TearDown()
        {
            Bootstrapper.Shutdown();
        }
    }
}