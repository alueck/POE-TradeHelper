using POETradeHelper.ItemSearch.Contract.Models;
using System.Threading.Tasks;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public interface IStaticItemDataService
    {
        string GetId(Item item);
    }
}