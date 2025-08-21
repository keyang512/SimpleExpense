using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using SimpleExpenseManagement.Server.Data;
using SimpleExpenseManagement.Server.Models;

namespace SimpleExpenseManagement.Server.Controllers
{
    public class PeopleController : ODataController
    {
        private readonly ExpenseDbContext _context;

        public PeopleController(ExpenseDbContext context)
        {
            _context = context;
        }

        // GET: odata/People
        [EnableQuery]
        public IQueryable<PersonDto> Get()
        {
            return _context.People
                .OrderBy(p => p.Name)
                .Select(p => new PersonDto
                {
                    Id = p.Id,
                    Name = p.Name
                });
        }

        // GET: odata/People({id})
        [EnableQuery]
        public async Task<ActionResult<PersonDto>> Get([FromRoute] Guid key)
        {
            var person = await _context.People
                .FirstOrDefaultAsync(p => p.Id == key);

            if (person == null)
            {
                return NotFound();
            }

            var personDto = new PersonDto
            {
                Id = person.Id,
                Name = person.Name
            };

            return Ok(personDto);
        }

        // GET: odata/People({id})/Expenses
        [EnableQuery]
        public IQueryable<ExpenseDto> GetExpenses([FromRoute] Guid key)
        {
            return _context.Expenses
                .Where(e => e.PersonId == key)
                .Include(e => e.Person)
                .Include(e => e.ExpenseCategories)
                    .ThenInclude(ec => ec.Category)
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

        // GET: odata/People({id})/Expenses/$count
        public async Task<ActionResult<int>> GetExpensesCount([FromRoute] Guid key)
        {
            var count = await _context.Expenses
                .Where(e => e.PersonId == key)
                .CountAsync();

            return Ok(count);
        }

        // GET: odata/People/summary
        [HttpGet("summary")]
        public async Task<ActionResult<object>> GetSummary()
        {
            var peopleSummary = await _context.People
                .Select(p => new
                {
                    PersonId = p.Id,
                    PersonName = p.Name,
                    ExpenseCount = p.Expenses.Count,
                    TotalAmount = p.Expenses.Sum(e => e.Amount),
                    LastExpenseDate = p.Expenses.Max(e => e.Date)
                })
                .ToListAsync();

            return Ok(peopleSummary);
        }
    }
}
