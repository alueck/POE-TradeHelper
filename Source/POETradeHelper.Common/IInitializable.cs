using System.Threading.Tasks;

namespace POETradeHelper.Common
{
    public interface IInitializable
    {
        Task OnInitAsync();
    }
}