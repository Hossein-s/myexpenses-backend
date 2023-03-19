using System.ComponentModel.DataAnnotations;

namespace MyExpenses.Entities;

public class Expense
{
    [Key]
    public Guid Id { get; set; }

    public decimal Amount { get; set; }

    public User User { get; set; }

    public DateTime SpentAt { get; set; }

    public Category Category { get; set; }
}
