using ExpenseTracker.API.Data;
using ExpenseTracker.API.DTOs.Expense;
using ExpenseTracker.API.DTOs.Reports;
using ExpenseTracker.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ExpenseTracker.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext dbContext;

        public ReportsController(AppDbContext _dbContext)
        {
            dbContext= _dbContext;
        }

        private int GetLoggedInUserId()
        {
            return int.Parse(User.FindFirst("UserId")?.Value);
        }

        #region MonthlyExpenses
        [HttpGet("monthly_expenses")]
        public async Task<IActionResult> GetMonthlyExpenses(int year, int month)
        {
           // for date validation
            var validationResult = ValidateYearMonth(year, month);
            if (validationResult != null)
                return validationResult;

            int UserId = GetLoggedInUserId();

            var expense = await dbContext.tbl_Expense
                .Include(e => e.Category)
                .Where(
                    e => e.UserId==UserId &&
                    e.ExpenseDate.Year == year && e.ExpenseDate.Month == month)

                .OrderByDescending(e => e.ExpenseDate)
                .ThenByDescending(e=> e.ExpenseId)
                    .Select(e => new ExpenseReadDto
                    {
                        ExpenseId = e.ExpenseId,
                        Title = e.Title,
                        Amount = e.Amount,
                        ExpenseDate = e.ExpenseDate,
                        Description = e.Description,
                        CategoryId = e.CategoryId,
                        CategoryName = e.Category.Name
                    })
                    .ToListAsync();

            return Ok(expense);
        }

        #endregion MonthlyExpenses

        #region MonthlyTotal
        [HttpGet("monthly_total")]
        public async Task<IActionResult> GetMonthlyTotal(int year, int month)
        {
            var validationResult = ValidateYearMonth(year, month);
            if (validationResult != null)
                return validationResult;

            int UserId = GetLoggedInUserId();
            var expensetotal = await dbContext.tbl_Expense
                .Where(
                    e => e.UserId==UserId &&
                    e.ExpenseDate.Year == year && e.ExpenseDate.Month == month)
                .SumAsync(e=> (decimal?)e.Amount) ?? 0;

            return Ok(new {Year=year,Month=month,Total=expensetotal});

        }
        #endregion MonthlyTotal


        #region CategoryWiseExpenseTotal
        [HttpGet("CategoryWiseExpenseTotal")]
        public async Task<IActionResult> GetCategoryWiseExpenseTotal(int year, int month)
        {
            var validationResult = ValidateYearMonth(year, month);
            if (validationResult != null)
                return validationResult;

            int UserId = GetLoggedInUserId();
            var catTotal = await dbContext.tbl_Expense
                .Include(e => e.Category)
                .Where(
                    e => e.UserId == UserId &&
                    e.ExpenseDate.Year == year && e.ExpenseDate.Month == month)

                            .GroupBy(e => e.Category.Name)
                            .Select(g => new
                            {
                                CategoryName = g.Key,
                                TotalAmount = g.Sum(e => e.Amount)
                            })
                            .ToListAsync();

            return Ok(catTotal);
        }
        #endregion CategoryWiseExpenseTotal


        #region ExpenseWithDateRange
        [HttpGet("date_range")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateRangeFilterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.StartDate > dto.EndDate)
                return BadRequest("Start date cannot be greater than end date.");

            int UserId = GetLoggedInUserId();
            var expenses = await dbContext.tbl_Expense
                .Include(e => e.Category)
                .Where(
                    e => e.UserId == UserId &&
                    e.ExpenseDate.Date >= dto.StartDate.Value.Date &&
                            e.ExpenseDate.Date <= dto.EndDate.Value.Date)
                .OrderByDescending(e => e.ExpenseDate)
                .ThenByDescending(e => e.ExpenseId)
                .Select(e => new ExpenseReadDto
                {
                    ExpenseId = e.ExpenseId,
                    Title = e.Title,
                    Amount = e.Amount,
                    ExpenseDate = e.ExpenseDate,
                    Description = e.Description,
                    CategoryId = e.CategoryId,
                    CategoryName = e.Category.Name
                })
                .ToListAsync();

            return Ok(expenses);
        }

        #endregion ExpenseWithDateRange



        #region GetAllExpensesOfToday
        [HttpGet("GetAllExpensesOfToday")]
        public async Task<IActionResult> GetAllExpensesOfToday()
        {
            int userId = GetLoggedInUserId();
            var expenses = await dbContext.tbl_Expense
                .Where(e => e.UserId == userId && e.ExpenseDate == DateTime.UtcNow.Date)
                .Select(
                    e => new ExpenseReadDto
                    {
                        ExpenseId = e.ExpenseId,
                        Title = e.Title,
                        Amount = e.Amount,
                        ExpenseDate = e.ExpenseDate,
                        Description = e.Description,
                        CategoryId = e.CategoryId,
                        CategoryName = e.Category.Name
                    }
                ).OrderByDescending(e => e.ExpenseDate).ThenByDescending(e => e.ExpenseId)
                .ToListAsync();
            return Ok(new { success = true, expenses });
        }
        #endregion GetAllExpensesOfToday



        private IActionResult? ValidateYearMonth(int year, int month)
        {
            
            var today = DateTime.Today;

            // Month validation
            if (month < 1 || month > 12)
                return BadRequest("Month must be between 1 and 12.");

            // Too old year validation
            int minYear = today.Year - 1;
            if (year < minYear)
                return BadRequest($"Year cannot be before {minYear}.");

            // Future month validation
            var selectedMonth = new DateTime(year, month, 1);
            var currentMonth = new DateTime(today.Year, today.Month, 1);

            if (selectedMonth > currentMonth)
                return BadRequest("Future month is not allowed.");

            return null;
        }
    }
}
