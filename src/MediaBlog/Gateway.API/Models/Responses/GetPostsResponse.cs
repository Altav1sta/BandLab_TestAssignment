using Posts.API.SDK.Models;

namespace Gateway.API.Models.Responses
{
    public class GetPostsResponse
    {
        public Post[] Posts { get; set; } = [];

        public int? NextCursorCommentCount { get; set; }

        public int? NextCursorId { get; set; }

        public int Limit { get; set; }

        public GetPostsResponse(Post[] posts, int limit)
        {
            Posts = posts;
            Limit = limit;
            NextCursorCommentCount = posts?[^1].CommentsCount;
            NextCursorId = posts?[^1].Id;
        }

        public int[] GetAffectingCommentCounts()
        {
            var isFullPage = Posts.Length == Limit;
            var lowestAffectingCommentsCount = isFullPage ? Posts[^1].CommentsCount : 0;
            var affectingRange = Enumerable.Range(lowestAffectingCommentsCount, Posts[0].CommentsCount - lowestAffectingCommentsCount + 1);

            return affectingRange.ToArray();
        }
    }
}
