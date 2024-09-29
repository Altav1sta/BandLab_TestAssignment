using Common.Caching.Interfaces;
using Common.Messaging.Events;
using Microsoft.EntityFrameworkCore;
using Posts.API.Services.Interfaces;
using Posts.Data;
using Posts.Data.Entities;

namespace Posts.API.Services
{
    public class PostsService(PostsDbContext context, IRedisService redisService, ILogger<PostsService> logger) : IPostsService
    {
        public const int AttachedCommentsCount = 2;

        public async Task CreatePostAsync(CreatePostRequestedEvent model)
        {
            var post = new Post
            {
                Author = model.Author,
                Caption = model.Caption,
                ImageUrl = model.ImageUrl,
                CommentsCount = 0,
                CreatedAt = DateTime.UtcNow
            };

            // ... here we need to convert image to proper size and format, get the new image link and save it in post
            // as post does not make sense without image ...

            context.Posts.Add(post);
            await context.SaveChangesAsync();

            logger.LogInformation("Post was created.");

            await redisService.ClearFeedPageKeysSetAsync(0);

            logger.LogInformation("Cache was updated.");
        }

        public async Task<Post[]> GetPostsAsync(int? cursorCommentsCount, int? cursorId, int limit)
        {
            var query = context.Posts.AsNoTracking();

            if (cursorCommentsCount.HasValue && cursorId.HasValue)
            {
                query = query.Where(x => x.CommentsCount < cursorCommentsCount
                    || (x.CommentsCount == cursorCommentsCount && x.Id > cursorId));
            }

            query = query.OrderByDescending(x => x.CommentsCount).ThenBy(x => x.Id);

            var posts = await query
                .Take(limit)
                .Include(x => x.Comments.OrderByDescending(x => x.Id).Take(AttachedCommentsCount))
                .ToArrayAsync();

            return posts;
        }
    }
}
