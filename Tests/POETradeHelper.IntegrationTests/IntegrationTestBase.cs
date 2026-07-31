using System;

using Autofac;

using NUnit.Framework;

using Splat.Autofac;

namespace POETradeHelper.IntegrationTests;

public abstract class IntegrationTestBase
{
    private static IContainer? Container;

    [OneTimeTearDown]
    public static void OneTimeTearDown()
    {
        Bootstrapper.Shutdown();
        Container?.Dispose();
    }

    protected static IContainer Setup(Action<ContainerBuilder>? customRegistration = null)
    {
        ContainerBuilder builder = new();
        var autofacDependencyResolver = builder.UseAutofacDependencyResolver();
        builder.RegisterInstance(autofacDependencyResolver);

        Bootstrapper.Configure(builder);
        customRegistration?.Invoke(builder);
        var container = Container = builder.Build();
        autofacDependencyResolver.SetLifetimeScope(container);

        return container;
    }
}