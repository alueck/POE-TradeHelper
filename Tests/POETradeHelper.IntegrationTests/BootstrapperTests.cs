using System;
using System.Collections.Generic;
using System.Linq;

using Autofac;

using AwesomeAssertions;
using AwesomeAssertions.Execution;

using NUnit.Framework;

using POETradeHelper.QualityOfLife.Models;
using POETradeHelper.QualityOfLife.Services;

using Splat;
using Splat.Autofac;

namespace POETradeHelper.IntegrationTests
{
    public class BootstrapperTests : IDisposable
    {
        private readonly IContainer container;

        public BootstrapperTests()
        {
            ContainerBuilder builder = new();
            var autofacDependencyResolver = builder.UseAutofacDependencyResolver();
            builder.RegisterInstance(autofacDependencyResolver);
            Bootstrapper.Configure(builder);
            this.container = builder.Build();
            autofacDependencyResolver.SetLifetimeScope(this.container);
        }

        public void Dispose()
        {
            Bootstrapper.Shutdown();
            this.container?.Dispose();
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