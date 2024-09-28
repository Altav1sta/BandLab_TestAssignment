﻿using Posts.API.Messaging.Events;
using Posts.API.Services.Interfaces;
using Posts.Data;
using Posts.Data.Entities;

namespace Posts.API.Services
{
    public class PostsService(PostsDbContext context) : IPostsService
    {
        public async Task CreatePostAsync(CreatePostRequestedEvent model)
        {
            var post = new Post
            {
                Author = model.Author,
                Caption = model.Caption,
                ImageUrl = model.ImageUrl,
                CommentsCount = 0,
                CreatedAt = DateTime.UtcNow
            };

            // ... here we need to convert image to proper size and format, get the new image link and save it in post
            // as post does not make sense without image ...

            context.Posts.Add(post);
            await context.SaveChangesAsync();
        }
    }
}
