using System.ComponentModel.DataAnnotations;

namespace SimpleExpenseManagement.Server.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for many-to-many relationship with expenses
        public ICollection<ExpenseCategory> ExpenseCategories { get; set; } = new List<ExpenseCategory>();
    }
}
