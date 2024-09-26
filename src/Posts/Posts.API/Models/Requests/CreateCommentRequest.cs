using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Posts.API.Models.Requests
{
    public class CreateCommentRequest
    {
        [BindRequired]
        public int PostId { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string Text { get; set; }
    }
}
