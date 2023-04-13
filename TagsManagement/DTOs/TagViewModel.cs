using System.ComponentModel.DataAnnotations;

namespace TagsManagement.DTOs
{
    public class TagViewModel
    {
        [Required]
        [StringLength(maximumLength: 20, MinimumLength = 2, ErrorMessage = "Tag name must be between 2 and 20 characters long.")]
        public string Name { get; set; }
    }
}
