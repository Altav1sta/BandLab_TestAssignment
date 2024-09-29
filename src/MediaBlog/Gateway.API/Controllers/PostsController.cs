using Common.Caching;
using Common.Caching.Interfaces;
using Common.Messaging;
using Common.Messaging.Events;
using Common.Messaging.Interfaces;
using Gateway.API.Models.Requests;
using Gateway.API.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Posts.API.SDK;

namespace Gateway.API.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostsController(IPostsApiClient postsApiClient, IRedisService redisService, IMessageProducer messageProducer) : ControllerBase
    {
        public const int CacheTtlSeconds = 60;

        [HttpGet]
        [ProducesResponseType(typeof(GetPostsResponse), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllPosts([FromQuery] int? cursorCommentsCount, [FromQuery] int? cursorId, [FromQuery] int limit = 10)
        {
            var cacheKey = string.Format(CachingConsts.PostsFeedPageCacheKey, cursorCommentsCount, cursorId, limit);
            var cachedResponse = await redisService.GetFromJsonCacheAsync<GetPostsResponse>(cacheKey);

            if (cachedResponse != null)
            {
                return Ok(cachedResponse);
            }

            var posts = await postsApiClient.GetPostsAsync(cursorCommentsCount, cursorId, limit);
            var feedPage = new GetPostsResponse(posts, limit);

            await redisService.SetAsJsonCacheAsync(cacheKey, feedPage);

            // store cache key for each comment count to be able to selectively invalidate them on feed change
            var commentsCountValues = feedPage.GetAffectingCommentCounts();
            await redisService.AddFeedPageKeyToSet(cacheKey, commentsCountValues);

            return Ok(feedPage);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreatePost([FromForm] CreatePostRequest request)
        {
            // Fire the event which handler will do the image processing and post creation
            var eventModel = new CreatePostRequestedEvent
            {
                Author = request.Author,
                Caption = request.Caption,
                ImageUrl = "example.com" // todo: add storing the original image as a background process and getting its url or request id
            };

            messageProducer.SendMessage(eventModel, MessagingConsts.Queues.CreatePostRequestedQueue);

            return Ok();
        }
    }
}
