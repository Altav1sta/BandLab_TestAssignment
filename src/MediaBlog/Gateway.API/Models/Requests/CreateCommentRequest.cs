using System.ComponentModel.DataAnnotations;

namespace Gateway.API.Models.Requests
{
    public class CreateCommentRequest
    {
        [Required]
        public string Author { get; set; }

        [Required]
        public string Text { get; set; }
    }
}
