using System.Threading.Tasks;

namespace POETradeHelper.ItemSearch.Contract.Services
{
    public interface ICopyCommand
    {
        Task<string> ExecuteAsync(System.Threading.CancellationToken cancellationToken = default);
    }
}