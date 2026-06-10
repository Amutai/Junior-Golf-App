using System.Text.Json;
using JuniorGolf.Core.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace JuniorGolf.Infrastructure.Services;

/// <summary>
/// Redis-backed distributed cache implementation.
///
/// Data flow:
///   GetAsync: key → Redis GET → deserialize JSON → return T?
///   SetAsync: T → serialize JSON → Redis SET with TTL
///   RemoveAsync: key → Redis DEL
///
/// Uses IDistributedCache (Microsoft abstraction over Redis).
/// Default TTL: 5 minutes. Override per call via expiry parameter.
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(5);

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var data = await _cache.GetStringAsync(key);
        return data is null ? default : JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? DefaultExpiry
        };

        var json = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, json, options);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        var data = await _cache.GetAsync(key);
        return data is not null;
    }
}
