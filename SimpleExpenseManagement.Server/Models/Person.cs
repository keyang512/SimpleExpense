using System.ComponentModel.DataAnnotations;

namespace SimpleExpenseManagement.Server.Models
{
    public class Person
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for one-to-many relationship with expenses
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
