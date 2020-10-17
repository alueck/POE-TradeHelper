using System;
using System.ComponentModel;
using POETradeHelper.Common.Contract.Attributes;

namespace POETradeHelper.Common.Contract
{
    [Singleton]
    public interface IUserInputEventProvider : IDisposable
    {
        event EventHandler<HandledEventArgs> SearchItem;

        event EventHandler<HandledEventArgs> HideOverlay;

        event EventHandler<HandledEventArgs> GoToHidehout;
    }
}