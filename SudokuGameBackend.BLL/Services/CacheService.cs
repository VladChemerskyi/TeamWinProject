using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SudokuGameBackend.BLL.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SudokuGameBackend.BLL.Services
{
    public static class CacheKeys
    {
        public static string DuelRating { get => "DuelRating"; }
        public static string SolvingRating { get => "SolvingRating"; }
    }

    public class CacheService : ICacheService
    {
        private readonly ConcurrentDictionary<string, SemaphoreSlim> semaphores;

        private readonly IMemoryCache memoryCache;
        private readonly ILogger<CacheService> logger;

        public CacheService(IMemoryCache memoryCache, ILogger<CacheService> logger)
        {
            semaphores = new ConcurrentDictionary<string, SemaphoreSlim>();

            this.memoryCache = memoryCache;
            this.logger = logger;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, TimeSpan expiration, Func<Task<T>> factory)
        {
            var semaphore = semaphores.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
            semaphore.Wait();
            T returnValue;
            if (memoryCache.TryGetValue(key, out T cachedValue))
            {
                returnValue = cachedValue;
            }
            else
            {
                logger.LogDebug($"Cashing values. key: {key}");
                returnValue = await factory.Invoke();
                memoryCache.Set(key, returnValue, expiration);
            }
            semaphore.Release();
            return returnValue;
        }
    }
}
