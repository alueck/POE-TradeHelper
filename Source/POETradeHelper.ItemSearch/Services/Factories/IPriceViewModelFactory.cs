using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using System.Threading.Tasks;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public interface IPriceViewModelFactory
    {
        Task<PriceViewModel> CreateAsync(Price price);
    }
}