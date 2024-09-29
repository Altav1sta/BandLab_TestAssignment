namespace Posts.Data.Entities
{
    public class Comment
    {
        public long Id { get; set; }

        public int PostId { get; set; }

        public string Author { get; set; }

        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
