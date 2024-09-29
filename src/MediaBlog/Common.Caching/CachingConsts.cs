namespace Common.Caching
{
    public static class CachingConsts
    {
        public const string PostsFeedPageCacheKey = "PostsFeed_{0}_{1}_{2}";
        public const string PostsFeedPageKeysSetKey = "PostsFeed_CacheKeys_{0}";

        public static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(60);
    }
}
