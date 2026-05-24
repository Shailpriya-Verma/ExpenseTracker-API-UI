using ExpenseTracker.API.Data;
using ExpenseTracker.API.DTOs.Expense;
using ExpenseTracker.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ExpenseTracker.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private readonly AppDbContext dbContext;

        public ExpensesController(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        // To get UserId from JWT token
        private int GetLoggedInUserId()
        {
            return int.Parse(User.FindFirst("UserId")?.Value);
        }

        #region AddExpense
        [HttpPost]
        public async Task<IActionResult> AddExpense(ExpenseCreateDto dto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            bool categoryExists = await dbContext.tbl_Category.AnyAsync(x=> x.CategoryId==dto.CategoryId);
            
            if(!categoryExists)
                return BadRequest("Invalid CategoryId");

            int userId = GetLoggedInUserId();

            Expense expense = new Expense
            {
                Title = dto.Title,
                Amount = dto.Amount,
                ExpenseDate = dto.ExpenseDate,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                UserId= userId
            };

            dbContext.tbl_Expense.Add(expense);
            await dbContext.SaveChangesAsync();

            //var result = new ExpenseReadDto
            //{
            //    ExpenseId = expense.ExpenseId,
            //    Title = expense.Title,
            //    Amount = expense.Amount,
            //    ExpenseDate = expense.ExpenseDate,
            //    Description = expense.Description,
            //    CategoryId = expense.CategoryId
            //};

            //return CreatedAtAction(nameof(GetExpenseById), new { id = expense.ExpenseId }, result);
            return Created("Expenses/AddExpense", new {success=true,message="Expense Added Successfully"});

        }
        #endregion AddExpense


        #region GetAllExpenses
        [HttpGet]
        public async Task<IActionResult> GetAllExpense()
        {
            int userId = GetLoggedInUserId();
            var expenses =await dbContext.tbl_Expense
                .Where(e=> e.UserId== userId)
                .Select(
                    e => new ExpenseReadDto
                    {
                        ExpenseId = e.ExpenseId,
                        Title = e.Title,
                        Amount = e.Amount,
                        ExpenseDate = e.ExpenseDate,
                        Description = e.Description,
                        CategoryId = e.CategoryId,
                        CategoryName=e.Category.Name
                    }
                ).OrderByDescending(e=> e.ExpenseDate).ThenByDescending(e => e.ExpenseId)
                .ToListAsync();
            return Ok(new {success=true, expenses });
        }
        #endregion GetAllExpenses

        #region GetExpenseById
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExpenseById(int id)
        {
            int userId = GetLoggedInUserId();
            var expense = await dbContext.tbl_Expense
                .Where(e => e.UserId==userId && e.ExpenseId == id)
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
                ).FirstOrDefaultAsync();

            if (expense == null)
                return NotFound("Expense Not Found");
            return Ok(new {success=true,expense});
        }
        #endregion GetExpenseById

        #region UpdateExpense
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, ExpenseUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = GetLoggedInUserId();

            var expense = await dbContext.tbl_Expense.FirstOrDefaultAsync
                (e => e.ExpenseId == id && e.UserId == userId);

            if (expense == null)
                return NotFound("Expense Not Found");

            bool isCategoryExists = await dbContext.tbl_Category.AnyAsync(c=> c.CategoryId==dto.CategoryId);
            if (!isCategoryExists)
                return BadRequest("Invalid CategoryId");

            expense.Title = dto.Title;
            expense.Description = dto.Description;
            expense.ExpenseDate = dto.ExpenseDate;
            expense.Amount = dto.Amount;
            expense.CategoryId = dto.CategoryId;

            await dbContext.SaveChangesAsync();

            return Ok(new {success=true,message="Updated Successfully"});

        }
        #endregion UpdateExpense


        #region DeleteExpense
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            int userId = GetLoggedInUserId();
            var expense = await dbContext.tbl_Expense.FirstOrDefaultAsync
                (e => e.ExpenseId == id && e.UserId == userId);

            if (expense == null)
                return NotFound("Expense Not Found");

            dbContext.tbl_Expense.Remove(expense);
            await dbContext.SaveChangesAsync();
            return Ok(new {success=true,message="Expense Deleted Successfully"});
        }
        #endregion DeleteExpense
    }
}
