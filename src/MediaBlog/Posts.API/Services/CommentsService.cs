using Common.Caching.Interfaces;
using Common.Messaging.Events;
using Microsoft.EntityFrameworkCore;
using Posts.API.Services.Interfaces;
using Posts.Data;
using Posts.Data.Entities;

namespace Posts.API.Services
{
    public class CommentsService(PostsDbContext context, IRedisService redisService, ILogger<CommentsService> logger) : ICommentsService
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

            var post = await context.Posts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.PostId);
            if (post is null)
            {
                logger.LogWarning("Post with ID = {PostId} was not found after adding the comment. Cache was not updated.", model.PostId);
                return;
            }

            await redisService.ClearFeedPageKeysSetAsync(post.CommentsCount);
            await redisService.ClearFeedPageKeysSetAsync(post.CommentsCount - 1);

            logger.LogInformation("Cache was updated");
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

            var post = await context.Posts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.PostId);
            if (post is null)
            {
                logger.LogWarning("Post with ID = {PostId} was not found after deleting the comment. Cache was not updated.", model.PostId);
                return;
            }

            await redisService.ClearFeedPageKeysSetAsync(post.CommentsCount + 1);
            await redisService.ClearFeedPageKeysSetAsync(post.CommentsCount);

            logger.LogInformation("Cache was updated");
        }
    }
}
