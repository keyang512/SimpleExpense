namespace SimpleExpenseManagement.Server.Models
{
    public class ExpenseCategory
    {
        public Guid ExpenseId { get; set; }
        public Expense Expense { get; set; } = null!;

        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
