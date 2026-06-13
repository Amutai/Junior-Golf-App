namespace JuniorGolf.Core.Interfaces;

/// <summary>
/// Distributed cache abstraction.
/// Implemented by RedisCacheService in Infrastructure.
/// Any service can cache/retrieve data without knowing the backing store.
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
}
