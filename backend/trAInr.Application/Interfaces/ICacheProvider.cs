namespace trAInr.Application.Interfaces;

/// <summary>
/// Provides an abstraction for caching operations, supporting both in-memory and distributed caches.
/// </summary>
public interface ICacheProvider
{
    /// <summary>
    /// Retrieves a cached value by key.
    /// </summary>
    /// <typeparam name="T">The type of the cached value</typeparam>
    /// <param name="key">The cache key</param>
    /// <returns>The cached value if found, otherwise null</returns>
    Task<T?> GetAsync<T>(string key) where T : class;

    /// <summary>
    /// Stores a value in the cache with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache</typeparam>
    /// <param name="key">The cache key</param>
    /// <param name="value">The value to cache</param>
    /// <param name="expirationMinutes">Optional expiration time in minutes. If not specified, uses default expiration.</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task SetAsync<T>(string key, T value, int? expirationMinutes = null) where T : class;

    /// <summary>
    /// Removes a cached value by key.
    /// </summary>
    /// <param name="key">The cache key to remove</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task RemoveAsync(string key);

    /// <summary>
    /// Removes all cached values whose keys start with the specified prefix.
    /// </summary>
    /// <param name="prefix">The key prefix to match</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task RemoveByPrefixAsync(string prefix);
}
