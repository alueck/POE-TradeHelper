using System.Threading.Tasks;

namespace POETradeHelper.Common.Contract
{
    public interface ICopyCommand
    {
        Task<string> ExecuteAsync(System.Threading.CancellationToken cancellationToken = default);
    }
}