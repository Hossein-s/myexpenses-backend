using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyExpenses.Entities;

public class Category
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; }
}
