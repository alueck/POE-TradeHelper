using System;

using MediatR;

namespace POETradeHelper.Common.Contract.Commands;

public class HideOverlayCommand : IRequest
{
    public HideOverlayCommand(Action onHandled)
    {
        OnHandled = onHandled;
    }

    public Action OnHandled { get; }
}