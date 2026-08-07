using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.DependencyInjection;

using POETradeHelper.Common.Contract;

using SharpHook;

namespace POETradeHelper.Common
{
    [ExcludeFromCodeCoverage]
    internal class CommonModule : IModule, IDisposable
    {
        private readonly SimpleGlobalHook hook = new();

        public void RegisterServices(IServiceCollection serviceCollection)
        {
            this.hook.RunAsync();
            serviceCollection.AddSingleton<IGlobalHook>(this.hook);
            serviceCollection.AddSingleton<IEventSimulator, EventSimulator>();
        }

        public void Dispose()
        {
            this.hook.Dispose();
        }
    }
}
