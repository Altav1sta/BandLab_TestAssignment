using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Posts.API.Models;
using Posts.API.Models.Responses;
using Posts.API.Services.Interfaces;

namespace Posts.API.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostsController(IPostsService postsService, IMapper mapper) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(GetPostsResponse), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllPosts([FromQuery] int? cursorCommentsCount, [FromQuery] int? cursorId, [FromQuery] int limit = 10)
        {
            // TODO: Get posts from cache
            var postEntities = await postsService.GetPostsAsync(cursorCommentsCount, cursorId, limit);
            var posts = mapper.Map<Post[]>(postEntities);
            var response = new GetPostsResponse
            {
                Posts = posts,
                CursorCommentCount = posts[^1].CommentsCount,
                CursorId = posts[^1].Id
            };

            return Ok(response);
        }
    }
}
