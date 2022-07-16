using System.Threading.Tasks;

using POETradeHelper.Common.Contract.Attributes;

namespace POETradeHelper.Common.Contract
{
    [Singleton]
    public interface IInitializable
    {
        Task OnInitAsync();
    }
}