using Microsoft.AspNetCore.Mvc;
using Posts.API.Models.Requests;

namespace Posts.API.Controllers
{
    [Route("api/comments")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateComment([FromBody] CreateCommentRequest request)
        {
            // Fire the event which handler will do the comment creation and counter updating

            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public IActionResult DeleteComment([FromRoute] int id)
        {
            // Fire the event which handler will delete the comment and update the counter

            return Ok();
        }
    }
}
