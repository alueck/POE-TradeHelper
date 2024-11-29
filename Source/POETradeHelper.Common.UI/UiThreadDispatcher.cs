using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Avalonia.Threading;

namespace POETradeHelper.Common.UI;

[ExcludeFromCodeCoverage]
public class UiThreadDispatcher : IUiThreadDispatcher
{
    public bool CheckAccess()
    {
        return Dispatcher.UIThread.CheckAccess();
    }

    public void VerifyAccess()
    {
        Dispatcher.UIThread.VerifyAccess();
    }

    public void Post(Action action, DispatcherPriority priority = default)
    {
        Dispatcher.UIThread.Post(action, priority);
    }

    public async Task InvokeAsync(Action action, DispatcherPriority priority = default)
    {
        await Dispatcher.UIThread.InvokeAsync(action, priority);
    }

    public Task InvokeAsync(Func<Task> function, DispatcherPriority priority = default)
    {
        return Dispatcher.UIThread.InvokeAsync(function, priority);
    }
}