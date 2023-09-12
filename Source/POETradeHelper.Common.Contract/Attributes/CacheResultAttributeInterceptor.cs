using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Castle.DynamicProxy;

using Microsoft.Extensions.Caching.Memory;

namespace POETradeHelper.Common.Contract.Attributes;

public class CacheResultAttributeInterceptor : IInterceptor
{
    private static readonly IDictionary<MethodInfo, CacheResultAttribute?> AttributeCache = new ConcurrentDictionary<MethodInfo, CacheResultAttribute?>();

    private readonly IMemoryCache memoryCache;

    public CacheResultAttributeInterceptor(IMemoryCache memoryCache)
    {
        this.memoryCache = memoryCache;
    }

    public void Intercept(IInvocation invocation)
    {
        CacheResultAttribute? attribute = GetAttribute(invocation);
        if (attribute == null || invocation.MethodInvocationTarget.ReturnType == typeof(void) || invocation.MethodInvocationTarget.ReturnType == typeof(Task))
        {
            invocation.Proceed();
            return;
        }

        string cacheKey = GetCacheKey(invocation);
        if (this.memoryCache.TryGetValue(cacheKey, out object result))
        {
            invocation.ReturnValue = result;
            return;
        }

        if (invocation.MethodInvocationTarget.ReturnType.IsSubclassOf(typeof(Task)))
        {
            this.HandleAsyncInvocation(invocation, cacheKey, attribute.DurationSeconds);
        }
        else
        {
            this.HandleSyncInvocation(invocation, cacheKey, attribute.DurationSeconds);
        }
    }

    private void HandleSyncInvocation(IInvocation invocation, string cacheKey, int cacheDurationSeconds)
    {
        invocation.Proceed();
        if (invocation.ReturnValue != null)
        {
            this.CreateCacheEntry(cacheKey, invocation.ReturnValue, cacheDurationSeconds);
        }
    }

    private void HandleAsyncInvocation(IInvocation invocation, string cacheKey, int cacheDurationSeconds)
    {
        invocation.Proceed();
        Task task = (Task)invocation.ReturnValue;
        task.ContinueWith(
            t =>
            {
                var taskType = t.GetType();
                if (!taskType.IsGenericType || ((dynamic)t).Result != null)
                {
                    this.CreateCacheEntry(cacheKey, t!, cacheDurationSeconds);
                }
            },
            TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);
    }

    private void CreateCacheEntry(string cacheKey, object value, int cacheDurationSeconds)
    {
        using var entry = this.memoryCache.CreateEntry(cacheKey);
        entry.Value = value;
        if (cacheDurationSeconds > 0)
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheDurationSeconds);
        }
    }

    private static string GetCacheKey(IInvocation invocation)
    {
        string methodKey = string.Join(
            '-',
            invocation.TargetType.FullName,
            invocation.MethodInvocationTarget.Name,
            JsonSerializer.Serialize(invocation.Arguments.Where(a => a is not CancellationToken)));

        using var sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(methodKey));

        return Convert.ToHexString(hashBytes);
    }

    private static CacheResultAttribute? GetAttribute(IInvocation invocation)
    {
        if (!AttributeCache.TryGetValue(invocation.MethodInvocationTarget, out var attribute))
        {
            AttributeCache[invocation.MethodInvocationTarget] = attribute = invocation.MethodInvocationTarget.GetCustomAttribute<CacheResultAttribute>();
        }

        return attribute;
    }
}