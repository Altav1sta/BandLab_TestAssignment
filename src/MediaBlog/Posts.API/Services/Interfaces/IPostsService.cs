using Common.Messaging.Events;
using Posts.Data.Entities;

namespace Posts.API.Services.Interfaces
{
    public interface IPostsService
    {
        Task CreatePostAsync(CreatePostRequestedEvent model);

        Task<Post[]> GetPostsAsync(int? cursorCommentsCount, int? cursorId, int limit);
    }
}
