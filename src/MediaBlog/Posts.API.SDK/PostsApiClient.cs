using Posts.API.SDK.Models;
using System.Net.Http.Json;

namespace Posts.API.SDK
{
    public interface IPostsApiClient
    {
        Task<Post[]> GetPostsAsync(int? cursorCommentsCount, int? cursorId, int limit = 10);
    }

    public class PostsApiClient(HttpClient client) : IPostsApiClient
    {
        public async Task<Post[]> GetPostsAsync(int? cursorCommentsCount, int? cursorId, int limit = 10)
        {
            var url = $"api/posts?limit={limit}";

            if (cursorCommentsCount.HasValue && cursorId.HasValue)
            {
                url += $"&cursorCommentsCount={cursorCommentsCount}&cursorId={cursorId}";
            }

            var response = await client.GetFromJsonAsync<Post[]>(url);

            return response ?? [];
        }
    }
}
