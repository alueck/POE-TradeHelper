using System;

using MediatR;

namespace POETradeHelper.Common.Contract.Commands;

public class HideOverlayCommand : IRequest
{
    public HideOverlayCommand(Action onHandled)
    {
        this.OnHandled = onHandled;
    }

    public Action OnHandled { get; }
}