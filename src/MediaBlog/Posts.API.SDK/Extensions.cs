using Microsoft.Extensions.DependencyInjection;

namespace Posts.API.SDK
{
    public static class Extensions
    {
        public static IServiceCollection AddPostsApiClient(this IServiceCollection services, string baseAddress)
        {
            services.AddHttpClient<IPostsApiClient, PostsApiClient>(x => x.BaseAddress = new Uri(baseAddress));

            return services;
        }
    }
}
