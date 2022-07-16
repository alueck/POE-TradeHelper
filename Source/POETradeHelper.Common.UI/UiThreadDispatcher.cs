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

    public void Post(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
    {
        Dispatcher.UIThread.Post(action, priority);
    }

    public Task InvokeAsync(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
    {
        return Dispatcher.UIThread.InvokeAsync(action, priority);
    }

    public Task<TResult> InvokeAsync<TResult>(Func<TResult> function, DispatcherPriority priority = DispatcherPriority.Normal)
    {
        return Dispatcher.UIThread.InvokeAsync(function, priority);
    }

    public Task InvokeAsync(Func<Task> function, DispatcherPriority priority = DispatcherPriority.Normal)
    {
        return Dispatcher.UIThread.InvokeAsync(function, priority);
    }

    public Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> function, DispatcherPriority priority = DispatcherPriority.Normal)
    {
        return Dispatcher.UIThread.InvokeAsync(function, priority);
    }
}