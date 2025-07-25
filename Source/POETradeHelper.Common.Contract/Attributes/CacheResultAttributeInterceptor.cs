﻿using System;
using System.Collections.Concurrent;
using System.IO.Hashing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

using Castle.DynamicProxy;

using Microsoft.Extensions.Caching.Memory;

namespace POETradeHelper.Common.Contract.Attributes;

public class CacheResultAttributeInterceptor : IInterceptor
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
    };

    private static readonly ConcurrentDictionary<MethodInfo, CacheResultAttribute?> AttributeCache = new();

    private readonly IMemoryCache memoryCache;

    public CacheResultAttributeInterceptor(IMemoryCache memoryCache)
    {
        this.memoryCache = memoryCache;
    }

    public void Intercept(IInvocation invocation)
    {
        CacheResultAttribute? attribute = GetAttribute(invocation);
        if (attribute == null
            || invocation.MethodInvocationTarget!.ReturnType == typeof(void)
            || invocation.MethodInvocationTarget.ReturnType == typeof(Task))
        {
            invocation.Proceed();
            return;
        }

        string cacheKey = GetCacheKey(invocation);
        if (this.memoryCache.TryGetValue(cacheKey, out object? result))
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
        Task task = (Task)invocation.ReturnValue!;
        task.ContinueWith(
            t =>
            {
                var taskType = t.GetType();
                if (!taskType.IsGenericType || ((dynamic)t).Result != null)
                {
                    this.CreateCacheEntry(cacheKey, t, cacheDurationSeconds);
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
            invocation.TargetType!.FullName,
            invocation.MethodInvocationTarget!.Name,
            JsonSerializer.Serialize(invocation.Arguments.Where(a => a is not CancellationToken), JsonSerializerOptions));

        byte[] hashBytes = XxHash128.Hash(Encoding.UTF8.GetBytes(methodKey));

        return Convert.ToHexString(hashBytes);
    }

    private static CacheResultAttribute? GetAttribute(IInvocation invocation)
    {
        CacheResultAttribute? attribute = null;
        if (invocation.MethodInvocationTarget != null && !AttributeCache.TryGetValue(invocation.MethodInvocationTarget, out attribute))
        {
            AttributeCache[invocation.MethodInvocationTarget] = attribute = invocation.MethodInvocationTarget.GetCustomAttribute<CacheResultAttribute>();
        }

        return attribute;
    }
}