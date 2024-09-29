using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Posts.API.SDK.Models;
using Posts.API.Services.Interfaces;

namespace Posts.API.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostsController(IPostsService postsService, IMapper mapper) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(Post[]), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllPosts([FromQuery] int? cursorCommentsCount, [FromQuery] int? cursorId, [FromQuery] int limit = 10)
        {
            var posts = await postsService.GetPostsAsync(cursorCommentsCount, cursorId, limit);
            var response = mapper.Map<Post[]>(posts);

            return Ok(response);
        }
    }
}
