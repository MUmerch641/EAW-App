using System.Collections.Concurrent;
using System.Text.Json;

namespace MauiHybridApp.Services.Performance;

/// <summary>
/// High-performance in-memory cache service with TTL support
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;
    Task RemoveAsync(string key);
    Task ClearAsync();
    Task<bool> ExistsAsync(string key);
    Task<int> GetCountAsync();
    Task<string[]> GetKeysAsync(string pattern = "*");
}

public class CacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, CacheItem> _cache = new();
    private readonly Timer _cleanupTimer;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public CacheService()
    {
        // Run cleanup every 5 minutes
        _cleanupTimer = new Timer(CleanupExpiredItems, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        if (string.IsNullOrEmpty(key))
            return null;

        await _semaphore.WaitAsync();
        try
        {
            if (_cache.TryGetValue(key, out var item))
            {
                if (item.IsExpired)
                {
                    _cache.TryRemove(key, out _);
                    return null;
                }

                item.LastAccessed = DateTime.UtcNow;
                return JsonSerializer.Deserialize<T>(item.Data);
            }

            return null;
        }
        catch (Exception)
        {
            // Log error in production
            return null;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
    {
        if (string.IsNullOrEmpty(key) || value == null)
            return;

        await _semaphore.WaitAsync();
        try
        {
            var serializedData = JsonSerializer.Serialize(value);
            var expiryTime = expiry.HasValue ? DateTime.UtcNow.Add(expiry.Value) : DateTime.UtcNow.AddHours(1);

            var cacheItem = new CacheItem
            {
                Data = serializedData,
                ExpiryTime = expiryTime,
                CreatedAt = DateTime.UtcNow,
                LastAccessed = DateTime.UtcNow
            };

            _cache.AddOrUpdate(key, cacheItem, (k, v) => cacheItem);
        }
        catch (Exception)
        {
            // Log error in production
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task RemoveAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
            return;

        await _semaphore.WaitAsync();
        try
        {
            _cache.TryRemove(key, out _);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task ClearAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            _cache.Clear();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
            return false;

        await _semaphore.WaitAsync();
        try
        {
            if (_cache.TryGetValue(key, out var item))
            {
                if (item.IsExpired)
                {
                    _cache.TryRemove(key, out _);
                    return false;
                }
                return true;
            }
            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<int> GetCountAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            return _cache.Count;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<string[]> GetKeysAsync(string pattern = "*")
    {
        await _semaphore.WaitAsync();
        try
        {
            var keys = _cache.Keys.ToArray();
            
            if (pattern == "*")
                return keys;

            // Simple pattern matching (supports * wildcard)
            var regexPattern = pattern.Replace("*", ".*");
            return keys.Where(k => System.Text.RegularExpressions.Regex.IsMatch(k, regexPattern)).ToArray();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private void CleanupExpiredItems(object? state)
    {
        Task.Run(async () =>
        {
            await _semaphore.WaitAsync();
            try
            {
                var expiredKeys = _cache
                    .Where(kvp => kvp.Value.IsExpired)
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var key in expiredKeys)
                {
                    _cache.TryRemove(key, out _);
                }

                // Also remove least recently used items if cache is too large
                if (_cache.Count > 1000)
                {
                    var lruKeys = _cache
                        .OrderBy(kvp => kvp.Value.LastAccessed)
                        .Take(_cache.Count - 800)
                        .Select(kvp => kvp.Key)
                        .ToList();

                    foreach (var key in lruKeys)
                    {
                        _cache.TryRemove(key, out _);
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }
        });
    }

    public void Dispose()
    {
        _cleanupTimer?.Dispose();
        _semaphore?.Dispose();
    }
}

internal class CacheItem
{
    public string Data { get; set; } = string.Empty;
    public DateTime ExpiryTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastAccessed { get; set; }

    public bool IsExpired => DateTime.UtcNow > ExpiryTime;
}
