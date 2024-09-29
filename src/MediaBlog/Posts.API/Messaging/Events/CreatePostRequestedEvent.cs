namespace Posts.API.Messaging.Events
{
    public class CreatePostRequestedEvent
    {
        public string Author { get; set; }

        public string Caption { get; set; }

        public string ImageUrl { get; set; }
    }
}
