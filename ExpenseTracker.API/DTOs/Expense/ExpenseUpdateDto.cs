using ExpenseTracker.API.Validation;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.API.DTOs.Expense
{
    public class ExpenseUpdateDto
    {
        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [ValidExpenseDate]
        public DateTime ExpenseDate { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
