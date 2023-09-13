using System;
using System.Threading.Tasks;

using Avalonia.Threading;

namespace POETradeHelper.Common.UI;

public interface IUiThreadDispatcher : IDispatcher
{
    Task InvokeAsync(Action action, DispatcherPriority priority = default);

    Task InvokeAsync(Func<Task> function, DispatcherPriority priority = default);
}