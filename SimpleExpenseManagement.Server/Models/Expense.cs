using System.ComponentModel.DataAnnotations;

namespace SimpleExpenseManagement.Server.Models
{
    public class Expense
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Person relationship
        public Guid PersonId { get; set; }
        public Person Person { get; set; } = null!;

        // Categories relationship (many-to-many)
        public ICollection<ExpenseCategory> ExpenseCategories { get; set; } = new List<ExpenseCategory>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
