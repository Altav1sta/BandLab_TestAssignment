using Posts.API.Messaging.Events;

namespace Posts.API.Services.Interfaces
{
    public interface IPostsService
    {
        Task CreatePostAsync(CreatePostRequestedEvent model);
    }
}
