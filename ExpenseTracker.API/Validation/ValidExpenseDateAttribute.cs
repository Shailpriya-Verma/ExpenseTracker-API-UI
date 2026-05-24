using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.API.Validation
{
    public class ValidExpenseDateAttribute:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Expense date is required.");
            }

            if (!(value is DateTime expenseDate))
            {
                return new ValidationResult("Invalid expense date.");
            }

            int minYear = DateTime.Today.Year - 1; // Last 2 years allowed

            // Future date not allowed
            if (expenseDate.Date > DateTime.Today)
            {
                return new ValidationResult("Future date is not allowed.");
            }

            // Too old date not allowed
            if (expenseDate.Year < minYear)
            {
                return new ValidationResult($"Expense date cannot be before {minYear}.");
            }

            return ValidationResult.Success;
        }
    }
}
