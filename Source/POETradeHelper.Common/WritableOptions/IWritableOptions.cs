using System;

using Microsoft.Extensions.Options;

namespace POETradeHelper.Common.WritableOptions
{
    public interface IWritableOptions<out TOptions> : IOptionsSnapshot<TOptions>
        where TOptions : class, new()
    {
        void Update(Action<TOptions> update);
    }
}