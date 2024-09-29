using Microsoft.AspNetCore.Mvc;
using Posts.API.Messaging.Events;
using Posts.API.Messaging.Interfaces;
using Posts.API.Models.Requests;

namespace Posts.API.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class CommentsController(IMessageProducer messageProducer) : ControllerBase
    {
        [HttpPost("{postId}/comments")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateComment([FromRoute] int postId, [FromBody] CreateCommentRequest request)
        {
            // Fire the event which handler will do the comment creation and counter updating
            var eventModel = new CreateCommentRequestedEvent
            {
                PostId = postId,
                Author = request.Author,
                Text = request.Text
            };

            messageProducer.SendMessage(eventModel, Consts.Queues.CreateCommentRequestedQueue);

            return Ok();
        }

        [HttpDelete("{postId}/comments/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public IActionResult DeleteComment([FromRoute] int postId, [FromRoute] long id)
        {
            // Fire the event which handler will delete the comment and update the counter
            var eventModel = new DeleteCommentRequestedEvent
            {
                PostId = postId,
                CommentId = id
            };

            messageProducer.SendMessage(eventModel, Consts.Queues.DeleteCommentRequestedQueue);

            return Ok();
        }
    }
}
