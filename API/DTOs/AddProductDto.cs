using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class AddProductDto
    {
        [Required] public int ProductCategoryId { get; set; }
        [Required] public string Description { get; set; }
        [Required] public string LanguageCode { get; set; }
    }
}