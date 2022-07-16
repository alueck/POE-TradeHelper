using System.Threading.Tasks;

namespace POETradeHelper.Common.Contract
{
    public interface IUserInputSimulator
    {
        void SendCopyCommand();

        Task SendGotoHideoutCommand();
    }
}