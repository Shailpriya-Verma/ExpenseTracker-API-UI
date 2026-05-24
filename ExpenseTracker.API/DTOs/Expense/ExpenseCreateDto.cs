using ExpenseTracker.API.Validation;
using System.ComponentModel.DataAnnotations;
namespace ExpenseTracker.API.DTOs.Expense
{
    public class ExpenseCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(300)]
        public string? Description { get; set; }

        [Required]
        [Range(1,double.MaxValue,ErrorMessage = "Amount Must Be Greater Than 0")]
        public decimal Amount { get; set; }

        [ValidExpenseDate]
        public DateTime ExpenseDate { get; set; }

        [Required]
        public int CategoryId { get; set; }

    }
}
