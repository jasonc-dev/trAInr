using trAInr.Application.Interfaces;

namespace trAInr.Application.Helpers;

/// <summary>
/// Helper methods for working with cache providers.
/// Provides convenient methods to reduce boilerplate in cached service decorators.
/// </summary>
public static class CacheHelper
{
    /// <summary>
    /// Gets a value from cache, or executes the factory function and caches the result if not found.
    /// This method is designed for non-nullable return types. The factory must return a non-null value.
    /// </summary>
    /// <typeparam name="T">The type of value to cache (must be a class)</typeparam>
    /// <param name="cache">The cache provider instance</param>
    /// <param name="key">The cache key</param>
    /// <param name="factory">The factory function to execute if cache miss occurs (must return non-null)</param>
    /// <param name="expirationMinutes">Optional expiration time in minutes</param>
    /// <returns>The cached or newly created value (guaranteed non-null)</returns>
    /// <exception cref="InvalidOperationException">Thrown if the factory returns null</exception>
    public static async Task<T> GetOrSetAsync<T>(
        this ICacheProvider cache,
        string key,
        Func<Task<T>> factory,
        int? expirationMinutes = null) where T : class
    {
        var cachedValue = await cache.GetAsync<T>(key);
        if (cachedValue is not null) return cachedValue;

        var value = await factory();
        if (value is null)
        {
            throw new InvalidOperationException(
                $"Factory function for cache key '{key}' returned null. Use GetOrSetNullableAsync for nullable results.");
        }

        await cache.SetAsync(key, value, expirationMinutes);
        return value;
    }

    /// <summary>
    /// Gets a nullable value from cache, or executes the factory function and caches the result if not found.
    /// Only caches non-null results.
    /// </summary>
    /// <typeparam name="T">The type of value to cache (must be a class)</typeparam>
    /// <param name="cache">The cache provider instance</param>
    /// <param name="key">The cache key</param>
    /// <param name="factory">The factory function to execute if cache miss occurs</param>
    /// <param name="expirationMinutes">Optional expiration time in minutes</param>
    /// <returns>The cached or newly created value, or null if not found</returns>
    public static async Task<T?> GetOrSetNullableAsync<T>(
        this ICacheProvider cache,
        string key,
        Func<Task<T?>> factory,
        int? expirationMinutes = null) where T : class
    {
        var cachedValue = await cache.GetAsync<T>(key);
        if (cachedValue is not null) return cachedValue;

        var value = await factory();
        if (value is not null)
        {
            await cache.SetAsync(key, value, expirationMinutes);
        }

        return value;
    }
}