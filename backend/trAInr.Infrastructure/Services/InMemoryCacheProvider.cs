using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using trAInr.Application.Interfaces;

namespace trAInr.Infrastructure.Services;

/// <summary>
/// In-memory cache provider implementation using IMemoryCache.
/// Tracks cache keys by prefix for efficient bulk invalidation.
/// </summary>
public class InMemoryCacheProvider : ICacheProvider
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<InMemoryCacheProvider> _logger;
    
    // Thread-safe dictionary to track all cache keys by their prefix for efficient bulk removal
    private readonly ConcurrentDictionary<string, ConcurrentBag<string>> _keysByPrefix = new();
    
    // Thread-safe set to track all cache keys for cleanup
    private readonly ConcurrentDictionary<string, byte> _allKeys = new();
    
    private readonly int _defaultExpirationMinutes;
    private readonly bool _useSlidingExpiration;

    public InMemoryCacheProvider(
        IMemoryCache memoryCache,
        IConfiguration configuration,
        ILogger<InMemoryCacheProvider> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
        
        // Load cache settings from configuration
        _defaultExpirationMinutes = configuration.GetValue("CacheSettings:DefaultExpirationMinutes", 30);
        _useSlidingExpiration = configuration.GetValue("CacheSettings:SlidingExpiration", true);
    }

    public Task<T?> GetAsync<T>(string key) where T : class
    {
        var value = _memoryCache.Get<T>(key);
        
        if (value != null)
        {
            _logger.LogDebug("Cache hit for key: {Key}", key);
        }
        else
        {
            _logger.LogDebug("Cache miss for key: {Key}", key);
        }
        
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, int? expirationMinutes = null) where T : class
    {
        var expiration = expirationMinutes ?? _defaultExpirationMinutes;
        
        var cacheEntryOptions = new MemoryCacheEntryOptions();
        
        if (_useSlidingExpiration)
        {
            cacheEntryOptions.SlidingExpiration = TimeSpan.FromMinutes(expiration);
        }
        else
        {
            cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expiration);
        }
        
        // Register callback to remove key from tracking when it expires
        cacheEntryOptions.RegisterPostEvictionCallback((evictedKey, evictedValue, reason, state) =>
        {
            var keyString = evictedKey.ToString();
            if (keyString != null)
            {
                _allKeys.TryRemove(keyString, out _);
                RemoveKeyFromPrefixTracking(keyString);
            }
        });
        
        _memoryCache.Set(key, value, cacheEntryOptions);
        
        // Track the key
        _allKeys.TryAdd(key, 0);
        TrackKeyByPrefix(key);
        
        _logger.LogDebug("Cached value with key: {Key}, expiration: {Expiration} minutes", key, expiration);
        
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _memoryCache.Remove(key);
        _allKeys.TryRemove(key, out _);
        RemoveKeyFromPrefixTracking(key);
        
        _logger.LogDebug("Removed cache key: {Key}", key);
        
        return Task.CompletedTask;
    }

    public Task RemoveByPrefixAsync(string prefix)
    {
        if (_keysByPrefix.TryGetValue(prefix, out var keys))
        {
            var keysSnapshot = keys.ToArray();
            
            foreach (var key in keysSnapshot)
            {
                _memoryCache.Remove(key);
                _allKeys.TryRemove(key, out _);
            }
            
            // Clear the bag for this prefix
            _keysByPrefix.TryRemove(prefix, out _);
            
            _logger.LogDebug("Removed {Count} cache keys with prefix: {Prefix}", keysSnapshot.Length, prefix);
        }
        else
        {
            _logger.LogDebug("No cache keys found with prefix: {Prefix}", prefix);
        }
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Tracks a cache key by extracting its prefix and adding to the tracking dictionary.
    /// Prefix is everything before the last colon (:) in the key.
    /// </summary>
    private void TrackKeyByPrefix(string key)
    {
        // Extract prefix from key (everything before the last colon)
        var lastColonIndex = key.LastIndexOf(':');
        if (lastColonIndex > 0)
        {
            var prefix = key.Substring(0, lastColonIndex);
            
            var keyBag = _keysByPrefix.GetOrAdd(prefix, _ => new ConcurrentBag<string>());
            keyBag.Add(key);
        }
    }

    /// <summary>
    /// Removes a key from prefix tracking.
    /// Note: ConcurrentBag doesn't support removal, so we rely on cleanup during RemoveByPrefixAsync.
    /// This method is intentionally minimal as cleanup happens lazily.
    /// </summary>
    private static void RemoveKeyFromPrefixTracking(string _)
    {
        // ConcurrentBag doesn't support item removal, but that's acceptable because:
        // 1. Keys are removed from _allKeys and _memoryCache immediately
        // 2. RemoveByPrefixAsync rebuilds from the current bag snapshot
        // 3. This avoids complex locking while maintaining correctness
        
        // Note: The bag for this prefix still contains the key reference, but it won't be accessed
        // since it's removed from _allKeys and _memoryCache. Future RemoveByPrefixAsync will clean it up.
        
        // This method exists for symmetry with TrackKeyByPrefix and future extensibility
    }
}
