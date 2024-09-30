namespace Common.Messaging.Events
{
    public class DeleteCommentRequestedEvent
    {
        public int PostId { get; set; }

        public long CommentId { get; set; }

        public string Author { get; set; }
    }
}
