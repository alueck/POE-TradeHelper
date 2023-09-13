using System;

using Avalonia.ReactiveUI;

using Microsoft.Extensions.Logging;

using ReactiveUI;

using Splat;
using Splat.Microsoft.Extensions.Logging;

namespace POETradeHelper.Extensions;

public static class MutableDependencyResolverExtensions
{
    public static void InitializeAvalonia(this IMutableDependencyResolver resolver)
    {
        resolver.RegisterConstant(new AvaloniaActivationForViewFetcher(), typeof(IActivationForViewFetcher));
        resolver.RegisterConstant(new AutoDataTemplateBindingHook(), typeof(IPropertyBindingHook));
        RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
    }

    public static void UseMicrosoftExtensionsLoggingWithWrappingFullLogger(
        this IMutableDependencyResolver instance,
        Func<ILoggerFactory> loggerFactoryGetter)
    {
        var funcLogManager = new FuncLogManager(type =>
        {
            var actualLogger = loggerFactoryGetter().CreateLogger(type.ToString());
            var miniLoggingWrapper = new MicrosoftExtensionsLoggingLogger(actualLogger);
            return new WrappingFullLogger(miniLoggingWrapper);
        });

        instance.RegisterConstant(funcLogManager, typeof(ILogManager));
    }
}