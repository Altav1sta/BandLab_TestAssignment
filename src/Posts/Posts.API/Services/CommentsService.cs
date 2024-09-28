using Microsoft.EntityFrameworkCore;
using Posts.API.Messaging.Events;
using Posts.API.Services.Interfaces;
using Posts.Data;
using Posts.Data.Entities;

namespace Posts.API.Services
{
    public class CommentsService(PostsDbContext context, ILogger<CommentsService> logger) : ICommentsService
    {
        public async Task CreateCommentAsync(CreateCommentRequestedEvent model)
        {
            var comment = new Comment
            {
                Author = model.Author,
                PostId = model.PostId,
                Text = model.Text,
                CreatedAt = DateTime.UtcNow
            };

            context.Comments.Add(comment);
            await context.SaveChangesAsync();

            logger.LogInformation("Comment created successfully");

            await context.Posts
                .Where(x => x.Id == model.PostId)
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.CommentsCount, p => p.CommentsCount + 1));

            logger.LogInformation("Comments count in post was incremented");
        }

        public async Task DeleteCommentAsync(DeleteCommentRequestedEvent model)
        {
            await context.Comments
                .Where(x => x.Id == model.CommentId)
                .ExecuteDeleteAsync();

            logger.LogInformation("Comment deleted successfully");

            await context.Posts
                .Where(x => x.Id == model.PostId)
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.CommentsCount, p => p.CommentsCount - 1));

            logger.LogInformation("Comments count in post was decremented");
        }
    }
}
