using System.Threading.Tasks;
using POETradeHelper.Common.Contract.Attributes;

namespace POETradeHelper.Common
{
    [Singleton]
    public interface IInitializable
    {
        Task OnInitAsync();
    }
}