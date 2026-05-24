namespace ExpenseTracker.API.DTOs.Expense
{
    public class ExpenseReadDto
    {
        public int ExpenseId { get; set; }
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string? Description { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
