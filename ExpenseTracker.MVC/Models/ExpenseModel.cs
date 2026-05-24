using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.MVC.Models
{
    public class ExpenseModel
    {
        [Required]
        public string Title  { get; set; }

        public string Description { get; set; }

        [Required]
        [Range(1,Double.MaxValue,ErrorMessage ="Amount Must Be Greater Than 0")]
        public double Amount { get; set; }

        [Required]
        public DateTime? ExpenseDate { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
