using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using SimpleExpenseManagement.Server.Data;
using SimpleExpenseManagement.Server.Models;

namespace SimpleExpenseManagement.Server.Controllers
{
    public class CategoriesController : ODataController
    {
        private readonly ExpenseDbContext _context;

        public CategoriesController(ExpenseDbContext context)
        {
            _context = context;
        }

        // GET: odata/Categories
        [EnableQuery]
        public IQueryable<CategoryDto> Get()
        {
            return _context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                });
        }

        // GET: odata/Categories({id})
        [EnableQuery]
        public async Task<ActionResult<CategoryDto>> Get([FromRoute] Guid key)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == key);

            if (category == null)
            {
                return NotFound();
            }

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };

            return Ok(categoryDto);
        }

        // GET: odata/Categories({id})/Expenses
        [EnableQuery]
        public IQueryable<ExpenseDto> GetExpenses([FromRoute] Guid key)
        {
            return _context.ExpenseCategories
                .Where(ec => ec.CategoryId == key)
                .Include(ec => ec.Expense)
                    .ThenInclude(e => e.Person)
                .Include(ec => ec.Expense)
                    .ThenInclude(e => e.ExpenseCategories)
                        .ThenInclude(ec2 => ec2.Category)
                .Select(ec => new ExpenseDto
                {
                    Id = ec.Expense.Id,
                    Description = ec.Expense.Description,
                    Amount = ec.Expense.Amount,
                    Date = ec.Expense.Date,
                    Notes = ec.Expense.Notes,
                    Person = new PersonDto
                    {
                        Id = ec.Expense.Person.Id,
                        Name = ec.Expense.Person.Name
                    },
                    Categories = ec.Expense.ExpenseCategories.Select(ec2 => new CategoryDto
                    {
                        Id = ec2.Category.Id,
                        Name = ec2.Category.Name
                    }).ToList(),
                    CreatedAt = ec.Expense.CreatedAt,
                    UpdatedAt = ec.Expense.UpdatedAt
                });
        }

        // GET: odata/Categories({id})/Expenses/$count
        public async Task<ActionResult<int>> GetExpensesCount([FromRoute] Guid key)
        {
            var count = await _context.ExpenseCategories
                .Where(ec => ec.CategoryId == key)
                .CountAsync();

            return Ok(count);
        }

        // GET: odata/Categories/summary
        [HttpGet("summary")]
        public async Task<ActionResult<object>> GetSummary()
        {
            var categoriesSummary = await _context.Categories
                .Select(c => new
                {
                    CategoryId = c.Id,
                    CategoryName = c.Name,
                    ExpenseCount = c.ExpenseCategories.Count,
                    TotalAmount = c.ExpenseCategories.Sum(ec => ec.Expense.Amount),
                    LastExpenseDate = c.ExpenseCategories.Max(ec => ec.Expense.Date)
                })
                .ToListAsync();

            return Ok(categoriesSummary);
        }

        // GET: odata/Categories/popular
        [HttpGet("popular")]
        public async Task<ActionResult<object>> GetPopularCategories()
        {
            var popularCategories = await _context.Categories
                .Select(c => new
                {
                    CategoryId = c.Id,
                    CategoryName = c.Name,
                    ExpenseCount = c.ExpenseCategories.Count,
                    TotalAmount = c.ExpenseCategories.Sum(ec => ec.Expense.Amount)
                })
                .OrderByDescending(c => c.ExpenseCount)
                .ThenByDescending(c => c.TotalAmount)
                .ToListAsync();

            return Ok(popularCategories);
        }
    }
}
