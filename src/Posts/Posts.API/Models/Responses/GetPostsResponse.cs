﻿namespace Posts.API.Models.Responses
{
    public class GetPostsResponse
    {
        public Post[] Posts { get; set; } = [];

        public int CursorCommentCount { get; set; }

        public int CursorId { get; set; }
    }
}
