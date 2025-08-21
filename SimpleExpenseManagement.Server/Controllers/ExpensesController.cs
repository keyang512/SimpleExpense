using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using SimpleExpenseManagement.Server.Data;
using SimpleExpenseManagement.Server.Models;

namespace SimpleExpenseManagement.Server.Controllers
{
    public class ExpensesController : ODataController
    {
        private readonly ExpenseDbContext _context;

        public ExpensesController(ExpenseDbContext context)
        {
            _context = context;
        }

        // GET: odata/Expenses
        [EnableQuery]
        public IQueryable<ExpenseDto> Get()
        {
            return _context.Expenses
                .Include(e => e.Person)
                .Include(e => e.ExpenseCategories)
                    .ThenInclude(ec => ec.Category)
                .OrderByDescending(e => e.Date)
                .Select(e => new ExpenseDto
                {
                    Id = e.Id,
                    Description = e.Description,
                    Amount = e.Amount,
                    Date = e.Date,
                    Notes = e.Notes,
                    Person = new PersonDto
                    {
                        Id = e.Person.Id,
                        Name = e.Person.Name
                    },
                    Categories = e.ExpenseCategories.Select(ec => new CategoryDto
                    {
                        Id = ec.Category.Id,
                        Name = ec.Category.Name
                    }).ToList(),
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt
                });
        }

        // GET: odata/Expenses({id})
        [EnableQuery]
        public async Task<ActionResult<ExpenseDto>> Get([FromRoute] Guid key)
        {
            var expense = await _context.Expenses
                .Include(e => e.Person)
                .Include(e => e.ExpenseCategories)
                    .ThenInclude(ec => ec.Category)
                .Where(e => e.Id == key)
                .Select(e => new ExpenseDto
                {
                    Id = e.Id,
                    Description = e.Description,
                    Amount = e.Amount,
                    Date = e.Date,
                    Notes = e.Notes,
                    Person = new PersonDto
                    {
                        Id = e.Person.Id,
                        Name = e.Person.Name
                    },
                    Categories = e.ExpenseCategories.Select(ec => new CategoryDto
                    {
                        Id = ec.Category.Id,
                        Name = ec.Category.Name
                    }).ToList(),
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (expense == null)
            {
                return NotFound();
            }

            return Ok(expense);
        }

        // POST: odata/Expenses
        public async Task<ActionResult<ExpenseDto>> Post([FromBody] CreateExpenseDto createDto)
        {
            // Validate that the person exists
            var person = await _context.People.FindAsync(createDto.PersonId);
            if (person == null)
            {
                return BadRequest("Invalid Person ID");
            }

            // Validate that all categories exist
            var categories = await _context.Categories
                .Where(c => createDto.CategoryIds.Contains(c.Id))
                .ToListAsync();

            if (categories.Count != createDto.CategoryIds.Count)
            {
                return BadRequest("One or more Category IDs are invalid");
            }

            var expense = new Expense
            {
                Description = createDto.Description,
                Amount = createDto.Amount,
                Date = createDto.Date,
                Notes = createDto.Notes,
                PersonId = createDto.PersonId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            // Add category relationships
            foreach (var categoryId in createDto.CategoryIds)
            {
                var expenseCategory = new ExpenseCategory
                {
                    ExpenseId = expense.Id,
                    CategoryId = categoryId
                };
                _context.ExpenseCategories.Add(expenseCategory);
            }

            await _context.SaveChangesAsync();

            // Return the created expense with full details
            return await Get(expense.Id);
        }

        // PUT: odata/Expenses({id})
        public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] UpdateExpenseDto updateDto)
        {
            var existingExpense = await _context.Expenses
                .Include(e => e.ExpenseCategories)
                .FirstOrDefaultAsync(e => e.Id == key);

            if (existingExpense == null)
            {
                return NotFound();
            }

            // Validate that the person exists
            var person = await _context.People.FindAsync(updateDto.PersonId);
            if (person == null)
            {
                return BadRequest("Invalid Person ID");
            }

            // Validate that all categories exist
            var categories = await _context.Categories
                .Where(c => updateDto.CategoryIds.Contains(c.Id))
                .ToListAsync();

            if (categories.Count != updateDto.CategoryIds.Count)
            {
                return BadRequest("One or more Category IDs are invalid");
            }

            existingExpense.Description = updateDto.Description;
            existingExpense.Amount = updateDto.Amount;
            existingExpense.Date = updateDto.Date;
            existingExpense.Notes = updateDto.Notes;
            existingExpense.PersonId = updateDto.PersonId;
            existingExpense.UpdatedAt = DateTime.UtcNow;

            // Update category relationships
            _context.ExpenseCategories.RemoveRange(existingExpense.ExpenseCategories);

            foreach (var categoryId in updateDto.CategoryIds)
            {
                var expenseCategory = new ExpenseCategory
                {
                    ExpenseId = existingExpense.Id,
                    CategoryId = categoryId
                };
                _context.ExpenseCategories.Add(expenseCategory);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpenseExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: odata/Expenses({id})
        public async Task<IActionResult> Delete([FromRoute] Guid key)
        {
            var expense = await _context.Expenses
                .Include(e => e.ExpenseCategories)
                .FirstOrDefaultAsync(e => e.Id == key);

            if (expense == null)
            {
                return NotFound();
            }

            // Remove category relationships first
            _context.ExpenseCategories.RemoveRange(expense.ExpenseCategories);
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: odata/Expenses/test-connection
        [HttpGet("test-connection")]
        public async Task<ActionResult<object>> TestConnection()
        {
            try
            {
                // Test database connection
                var canConnect = await _context.Database.CanConnectAsync();
                var connectionString = _context.Database.GetConnectionString();

                return Ok(new
                {
                    CanConnect = canConnect,
                    ConnectionString = connectionString?.Replace("Password=***", "Password=***"),
                    DatabaseName = _context.Database.GetDbConnection().Database,
                    ServerName = _context.Database.GetDbConnection().DataSource,
                    Status = canConnect ? "Connected" : "Failed to connect"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    Status = "Connection failed"
                });
            }
        }

        // GET: odata/Expenses/summary
        [HttpGet("summary")]
        public async Task<ActionResult<object>> GetSummary()
        {
            var totalExpenses = await _context.Expenses.SumAsync(e => e.Amount);
            var expenseCount = await _context.Expenses.CountAsync();

            var categoryBreakdown = await _context.ExpenseCategories
                .GroupBy(ec => ec.Category.Name)
                .Select(g => new { Category = g.Key, Total = g.Sum(ec => ec.Expense.Amount) })
                .OrderByDescending(x => x.Total)
                .ToListAsync();

            var personBreakdown = await _context.Expenses
                .GroupBy(e => e.Person.Name)
                .Select(g => new { Person = g.Key, Total = g.Sum(e => e.Amount) })
                .OrderByDescending(x => x.Total)
                .ToListAsync();

            return Ok(new
            {
                TotalExpenses = totalExpenses,
                ExpenseCount = expenseCount,
                CategoryBreakdown = categoryBreakdown,
                PersonBreakdown = personBreakdown
            });
        }

        private bool ExpenseExists(Guid id)
        {
            return _context.Expenses.Any(e => e.Id == id);
        }
    }
}
