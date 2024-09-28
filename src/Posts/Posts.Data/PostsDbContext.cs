using Microsoft.EntityFrameworkCore;
using Posts.Data.Entities;

namespace Posts.Data
{
    public class PostsDbContext : DbContext
    {
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }

        public PostsDbContext(DbContextOptions<PostsDbContext> options) : base(options)
        {

        }
    }
}
