using Microsoft.AspNetCore.Mvc;
using Posts.API.Messaging.Events;
using Posts.API.Messaging.Interfaces;
using Posts.API.Models.Requests;
using Posts.API.Models.Responses;

namespace Posts.API.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostsController(IMessageProducer messageProducer) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(GetPostsResponse), 200)]
        [ProducesResponseType(500)]
        public IActionResult GetAllPosts([FromQuery] int? cursorCommentsCount, [FromQuery] int? cursorId, [FromQuery] int limit = 10)
        {
            // Get posts from cache

            return Ok(new GetPostsResponse());
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

            messageProducer.SendMessage(eventModel, Consts.Queues.CreatePostRequestedQueue);

            return Ok();
        }
    }
}
