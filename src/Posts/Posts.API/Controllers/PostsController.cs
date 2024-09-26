using Microsoft.AspNetCore.Mvc;
using Posts.API.Models.Requests;
using Posts.API.Models.Responses;

namespace Posts.API.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostsController : ControllerBase
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
        [ProducesResponseType(500)]
        public IActionResult CreatePost([FromForm] CreatePostRequest request)
        {
            // Fire the event which handler will do the image processing and post creation

            return Ok();
        }
    }
}
