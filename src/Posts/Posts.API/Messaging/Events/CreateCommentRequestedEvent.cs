namespace Posts.API.Messaging.Events
{
    public class CreateCommentRequestedEvent
    {
        public int PostId { get; set; }

        public string Author { get; set; }

        public string Text { get; set; }
    }
}
