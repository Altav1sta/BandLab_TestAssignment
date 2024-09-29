using System.ComponentModel.DataAnnotations;

namespace Gateway.API.Models.Requests
{
    public class CreatePostRequest
    {
        [Required]
        public string Author { get; set; }

        [Required]
        public string Caption { get; set; }

        [Required]
        public IFormFile Image { get; set; }
    }
}
