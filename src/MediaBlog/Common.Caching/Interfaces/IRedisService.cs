namespace Common.Caching.Interfaces
{
    public interface IRedisService
    {
        Task AddFeedPageKeyToSet(string cacheKey, ICollection<int> commentsCountValues);
        Task ClearFeedPageKeysSetAsync(int commentsCount);
        Task<T?> GetFromJsonCacheAsync<T>(string cacheKey) where T : notnull;
        Task<bool> SetAsJsonCacheAsync<T>(string cacheKey, T value) where T : notnull;
    }
}
