namespace Posts.API
{
    public static class Consts
    {
        public static class Queues
        {
            public const string CreateCommentRequestedQueue = "create-comment-requested";
            public const string CreatePostRequestedQueue = "create-post-requested";
            public const string DeleteCommentRequestedQueue = "delete-comment-requested";
        }
    }
}
