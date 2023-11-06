using Microsoft.Extensions.Caching.Distributed;
using Ndknitor.System;
public static class CacheExtension
{
    public static T Get<T>(this IDistributedCache cache, string key)
    {
        string s = cache.GetString(key);
        if (s == null)
        {
            return default;
        }
        return s.ToClass<T>();
    }
    public static async Task<T> GetAsync<T>(this IDistributedCache cache, string key)
    {
        string s = await cache.GetStringAsync(key);
        if (s == null)
        {
            return default;
        }
        return s.ToClass<T>();
    }
    public static T GetRefresh<T>(this IDistributedCache cache, string key)
    {
        string s = cache.GetString(key);
        if (s != null)
        {
            cache.Refresh(key);
        }
        return s.ToClass<T>();
    }
    public static async Task<T> GetRefreshAsync<T>(this IDistributedCache cache, string key)
    {
        string s = await cache.GetStringAsync(key);
        if (s != null)
        {
            await cache.RefreshAsync(key);
        }
        return s.ToClass<T>();
    }
    public static void Set<T>(this IDistributedCache cache, string key, T value, int exprieSec = 0)
    {
        if (exprieSec == 0)
        {
            cache.SetString(key, value.ToJson());
        }
        else
        {
            cache.SetString(key, value.ToJson(), new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.Add(TimeSpan.FromSeconds(exprieSec))
            });
        }
    }
    public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, int exprieSec = 0)
    {
        if (exprieSec == 0)
        {
            await cache.SetStringAsync(key, value.ToJson());
        }
        else
        {
            await cache.SetStringAsync(key, value.ToJson(), new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.Add(TimeSpan.FromSeconds(exprieSec))
            });
        }
    }
}