namespace Common.Messaging.Events
{
    public class CreateCommentRequestedEvent
    {
        public int PostId { get; set; }

        public string Author { get; set; }

        public string Text { get; set; }
    }
}
