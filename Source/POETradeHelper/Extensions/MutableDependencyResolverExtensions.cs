using System;

using Microsoft.Extensions.Logging;

using Splat;
using Splat.Microsoft.Extensions.Logging;

namespace POETradeHelper.Extensions;

public static class MutableDependencyResolverExtensions
{
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