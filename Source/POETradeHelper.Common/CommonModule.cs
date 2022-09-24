using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.DependencyInjection;

using POETradeHelper.Common.Contract;

using SharpHook;
using SharpHook.Reactive;

namespace POETradeHelper.Common
{
    [ExcludeFromCodeCoverage]
    internal class CommonModule : IModule, IDisposable
    {
        private readonly SimpleReactiveGlobalHook hook = new();

        public void RegisterServices(IServiceCollection serviceCollection)
        {
            this.hook.RunAsync().Subscribe();
            serviceCollection.AddSingleton<IReactiveGlobalHook>(this.hook);
            serviceCollection.AddSingleton<IEventSimulator, EventSimulator>();
        }

        public void Dispose()
        {
            this.hook.Dispose();
        }
    }
}
