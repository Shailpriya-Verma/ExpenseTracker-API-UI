using ExpenseTracker.API.Data;
using ExpenseTracker.API.DTOs.Category;
using ExpenseTracker.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        public CategoriesController(AppDbContext _dbContext)
        {
            dbContext= _dbContext;
        }


        #region AddCategory
        [HttpPost]
        public async Task<IActionResult> AddCategoryAsync(CategoryCreateDto dto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            bool exists = await dbContext.tbl_Category.AnyAsync(
                    x => x.Name.ToLower() == dto.Name.ToLower()
                );
            if (exists)
            {
                return BadRequest("Category Already Exists");
            }

            Category category = new Category
            {
                Name = dto.Name
            };

            dbContext.tbl_Category.Add(category);
            await dbContext.SaveChangesAsync();

            return Ok(new {success=true, category });

        }
        #endregion AddCategory


        #region GetAllCategoris
        [HttpGet]
        public async Task<IActionResult> GetAllCategoris()
        {
            var categories = await dbContext.tbl_Category.Select(
                    x => new CategoryReadDto
                    {
                        CategoryId = x.CategoryId,
                        Name = x.Name
                    }
                ).ToListAsync();

            return Ok(categories);
        }
        #endregion GetAllCategory

        #region GetCategoryById
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category =await dbContext.tbl_Category.Where(c => c.CategoryId == id).
                Select(c => new CategoryReadDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name
                }).FirstOrDefaultAsync();

            if (category == null)
                return NotFound("Category Not Found");
            return Ok(category);
        }
        #endregion GetCategoryById

        #region UpdateCategory
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var category = await dbContext.tbl_Category.FindAsync(id);

            if (category == null)
                return NotFound("Category Not Exists");

            bool duplicate= await dbContext.tbl_Category.AnyAsync(
                    x=> x.CategoryId!=id && x.Name.ToLower()==dto.Name.ToLower()
                );

            if (duplicate)
                return BadRequest("Category With Same Name Already Exists");

            category.Name = dto.Name;

            await dbContext.SaveChangesAsync();

            CategoryReadDto categoryread = new CategoryReadDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name
            };
            return Ok(new {success=true, categoryread });
        }
        #endregion UpdateCategory

        #region DeleteCategory
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category =await dbContext.tbl_Category.FindAsync(id);

            if (category == null)
                return NotFound("Category Not Found");

            bool hasExpenses = await dbContext.tbl_Expense.AnyAsync(x=> x.CategoryId==id);

            if (hasExpenses)
                return BadRequest("Cannot delete category because it has expenses");

            dbContext.tbl_Category.Remove(category);
            await dbContext.SaveChangesAsync();

            return Ok(new {success=true,message= "Category Deleted Successfully" });
        }
        #endregion DeleteCategory

    }
}
