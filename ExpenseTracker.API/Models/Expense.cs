using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.API.Models
{
    public class Expense
    {
        [Key]
        public int ExpenseId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [Range(1,double.MaxValue,ErrorMessage ="Amount Must Be Greater Than 0")]
        public decimal Amount { get; set; }

        public DateTime ExpenseDate { get; set; } = DateTime.Now;

        [StringLength(300)]
        public string? Description { get; set; }

        // this is foreign key
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        // this is for navigation property
        public Category Category { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now; 
    }
}
