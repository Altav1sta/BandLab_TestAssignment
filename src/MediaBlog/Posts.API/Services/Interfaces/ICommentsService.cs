using Posts.API.Messaging.Events;

namespace Posts.API.Services.Interfaces
{
    public interface ICommentsService
    {
        Task CreateCommentAsync(CreateCommentRequestedEvent model);

        Task DeleteCommentAsync(DeleteCommentRequestedEvent model);
    }
}
