using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using POETradeHelper.Common.Contract;
using WindowsHook;

namespace POETradeHelper.Win32
{
    [ExcludeFromCodeCoverage]
    internal class Win32Module : IModule
    {
        public void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(Hook.GlobalEvents());
        }
    }
}