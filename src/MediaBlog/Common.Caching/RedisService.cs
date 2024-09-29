using Common.Caching.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace Common.Caching
{
    public class RedisService(IConnectionMultiplexer redisConnection) : IRedisService
    {
        private readonly IDatabase cache = redisConnection.GetDatabase();

        public async Task AddFeedPageKeyToSet(string cacheKey, ICollection<int> commentsCountValues)
        {
            foreach (var commentsCount in commentsCountValues)
            {
                var setCacheKey = string.Format(CachingConsts.PostsFeedPageKeysSetKey, commentsCount);
                await cache.SetAddAsync(setCacheKey, cacheKey);
            }
        }

        public async Task ClearFeedPageKeysSetAsync(int commentsCount)
        {
            var setCacheKey = string.Format(CachingConsts.PostsFeedPageKeysSetKey, commentsCount);
            var feedCacheKeys = await cache.SetMembersAsync(setCacheKey);

            foreach (var feedCacheKey in feedCacheKeys)
            {
                await cache.KeyDeleteAsync(feedCacheKey.ToString());
            }

            await cache.KeyDeleteAsync(setCacheKey);
        }

        public async Task<T?> GetFromJsonCacheAsync<T>(string cacheKey) where T : notnull
        {
            var cachedValue = await cache.StringGetSetExpiryAsync(cacheKey, CachingConsts.CacheTtl);

            if (!cachedValue.IsNullOrEmpty)
            {
                var cachedResponse = JsonSerializer.Deserialize<T>(cachedValue!);
                return cachedResponse;
            }

            return default;
        }

        public async Task<bool> SetAsJsonCacheAsync<T>(string cacheKey, T value) where T : notnull
        {
            var json = JsonSerializer.Serialize(value);
            var success = await cache.StringSetAsync(cacheKey, json, CachingConsts.CacheTtl);

            return success;
        }
    }
}
