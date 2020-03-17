using Microsoft.Extensions.Options;
using System;

namespace POETradeHelper.Common
{
    public interface IWritableOptions<out TOptions> : IOptionsSnapshot<TOptions> where TOptions : class, new()
    {
        void Update(Action<TOptions> update);
    }
}