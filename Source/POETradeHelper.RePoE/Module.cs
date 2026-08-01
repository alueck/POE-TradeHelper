using Microsoft.Extensions.DependencyInjection;

using POETradeHelper.Common.Contract;
using POETradeHelper.RePoE.Services;

namespace POETradeHelper.RePoE;

public class Module : IModule
{
    public void RegisterServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient<IAlternativeStatTextsService, AlternativeStatTextsService>(c => c.BaseAddress = new Uri("https://repoe-fork.github.io/"));
    }
}