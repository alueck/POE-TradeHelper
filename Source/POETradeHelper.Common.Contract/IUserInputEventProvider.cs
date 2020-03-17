using System;
using System.ComponentModel;

namespace POETradeHelper.Common.Contract
{
    public interface IUserInputEventProvider : IDisposable
    {
        event EventHandler<HandledEventArgs> SearchItem;

        event EventHandler<HandledEventArgs> HideOverlay;
    }
}