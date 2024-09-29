using Common.Messaging;
using Common.Messaging.Events;
using Common.Messaging.Interfaces;
using Gateway.API.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Controllers
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

            messageProducer.SendMessage(eventModel, MessagingConsts.Queues.CreateCommentRequestedQueue);

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

            messageProducer.SendMessage(eventModel, MessagingConsts.Queues.DeleteCommentRequestedQueue);

            return Ok();
        }
    }
}
