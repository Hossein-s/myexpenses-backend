namespace MyExpenses.Models;

public class CreateExpenseInput
{
    public decimal Amount { get; set; }

    public DateTime SpentAt { get; set; }

    public Guid Category { get; set; }
}
