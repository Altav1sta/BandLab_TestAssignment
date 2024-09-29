using Common.Messaging;
using Common.Messaging.Events;
using Common.Messaging.Interfaces;
using Gateway.API.Models.Requests;
using Gateway.API.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Posts.API.SDK;
using System.Text.Json;

namespace Gateway.API.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostsController(IPostsApiClient postsApiClient, IMessageProducer messageProducer, IDistributedCache cache) : ControllerBase
    {
        public const int CacheTtlSeconds = 60;

        [HttpGet]
        [ProducesResponseType(typeof(GetPostsResponse), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllPosts([FromQuery] int? cursorCommentsCount, [FromQuery] int? cursorId, [FromQuery] int limit = 10)
        {
            var cacheKey = $"PostsFeed_{cursorCommentsCount}_{cursorId}_{limit}";
            var cachedResponseJson = await cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedResponseJson))
            {
                var cachedResponse = JsonSerializer.Deserialize<GetPostsResponse>(cachedResponseJson);
                return Ok(cachedResponse);
            }

            var posts = await postsApiClient.GetPostsAsync(cursorCommentsCount, cursorId, limit);
            var response = new GetPostsResponse
            {
                Posts = posts,
                CursorCommentCount = posts[^1].CommentsCount,
                CursorId = posts[^1].Id
            }; 
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CacheTtlSeconds)
            };
            var responseJson = JsonSerializer.Serialize(response);

            await cache.SetStringAsync(cacheKey, responseJson, cacheOptions);

            return Ok(response);
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
