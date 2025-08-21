using Microsoft.EntityFrameworkCore;
using SimpleExpenseManagement.Server.Models;

namespace SimpleExpenseManagement.Server.Data
{
    public class ExpenseDbContext : DbContext
    {
        public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options)
            : base(options)
        {
        }

        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the Person entity
            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            // Configure the Category entity
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            // Configure the Expense entity
            modelBuilder.Entity<Expense>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Description).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Notes).HasMaxLength(500);

                // Configure relationship with Person
                entity.HasOne(e => e.Person)
                      .WithMany()
                      .HasForeignKey(e => e.PersonId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Configure many-to-many relationship with Categories
                entity.HasMany(e => e.ExpenseCategories)
                      .WithOne(ec => ec.Expense)
                      .HasForeignKey(ec => ec.ExpenseId);
            });

            // Configure the ExpenseCategory junction table
            modelBuilder.Entity<ExpenseCategory>(entity =>
            {
                entity.HasKey(ec => new { ec.ExpenseId, ec.CategoryId });

                entity.HasOne(ec => ec.Expense)
                      .WithMany(e => e.ExpenseCategories)
                      .HasForeignKey(ec => ec.ExpenseId);

                entity.HasOne(ec => ec.Category)
                      .WithMany()
                      .HasForeignKey(ec => ec.CategoryId);
            });

            // Seed People data
            var antonId = Guid.NewGuid();
            var steveId = Guid.NewGuid();

            modelBuilder.Entity<Person>().HasData(
                new Person
                {
                    Id = antonId,
                    Name = "Anton",
                    CreatedAt = DateTime.UtcNow
                },
                new Person
                {
                    Id = steveId,
                    Name = "Steve",
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Seed Categories data
            var officeExpensesId = Guid.NewGuid();
            var homeExpensesId = Guid.NewGuid();

            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = officeExpensesId,
                    Name = "Office Expenses",
                    CreatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Id = homeExpensesId,
                    Name = "Home Expenses",
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Seed some sample expenses
            var expense1Id = Guid.NewGuid();
            var expense2Id = Guid.NewGuid();

            modelBuilder.Entity<Expense>().HasData(
                new Expense
                {
                    Id = expense1Id,
                    Description = "Lunch at restaurant",
                    Amount = 25.50m,
                    Date = DateTime.Today.AddDays(-1),
                    Notes = "Business lunch with client",
                    PersonId = antonId,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new Expense
                {
                    Id = expense2Id,
                    Description = "Office supplies",
                    Amount = 45.75m,
                    Date = DateTime.Today.AddDays(-2),
                    Notes = "Printer paper and ink",
                    PersonId = steveId,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                }
            );

            // Seed expense-category relationships
            modelBuilder.Entity<ExpenseCategory>().HasData(
                new ExpenseCategory
                {
                    ExpenseId = expense1Id,
                    CategoryId = officeExpensesId
                },
                new ExpenseCategory
                {
                    ExpenseId = expense2Id,
                    CategoryId = officeExpensesId
                }
            );
        }
    }
}
