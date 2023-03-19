using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyExpenses.Entities;

public class Income
{
    [Key]
    public Guid Id { get; set; }

    public decimal Amount { get; set; }

    public User User { get; set; }

    public DateTime RecievedAt { get; set; }
}
