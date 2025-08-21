using System.ComponentModel.DataAnnotations;

namespace SimpleExpenseManagement.Server.Models
{
    public class CreateExpenseDto
    {
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

        [Required]
        public Guid PersonId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one category must be selected")]
        public List<Guid> CategoryIds { get; set; } = new List<Guid>();
    }
}
