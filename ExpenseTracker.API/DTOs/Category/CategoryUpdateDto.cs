using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.API.DTOs.Category
{
    public class CategoryUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
