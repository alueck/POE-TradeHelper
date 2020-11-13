using Microsoft.Extensions.DependencyInjection;

namespace POETradeHelper.Common.Contract
{
    public interface IModule
    {
        void RegisterServices(IServiceCollection serviceCollection);
    }
}