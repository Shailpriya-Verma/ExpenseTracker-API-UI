using ExpenseTracker.API.Validation;

namespace ExpenseTracker.API.DTOs.Reports
{
    public class DateRangeFilterDto
    {
        [ValidExpenseDate]
        public DateTime? StartDate { get; set; }

        [ValidExpenseDate]
        public DateTime? EndDate { get; set; }
    }
}
