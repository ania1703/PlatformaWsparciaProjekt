using System.ComponentModel.DataAnnotations;

namespace PlatformaWsparciaProjekt.Models
{
    public class PostViewModel
    {
        [Required]
        public string Content { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
