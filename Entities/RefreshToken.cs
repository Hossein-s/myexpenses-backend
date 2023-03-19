using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyExpenses.Entities;

public class RefreshToken
{
    [Key]
    public Guid Id { get; set; }

    public string Token { get; set; }

    public User User { get; set; }

    public DateTime ExpiresAt { get; set; }
}
