namespace Posts.Data.Entities
{
    public class Post
    {
        public int Id { get; set; }

        public string Author { get; set; }

        public string Caption { get; set; }

        public string? ImageUrl { get; set; }

        public int CommentsCount { get; set; }

        public DateTime CreatedAt { get; set; }


        public virtual ICollection<Comment> Comments { get; set; } = [];
    }
}
