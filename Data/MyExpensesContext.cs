namespace MyExpenses.Data;

using Microsoft.EntityFrameworkCore;
using MyExpenses.Entities;

public class MyExpensesContext : DbContext
{
    public MyExpensesContext(DbContextOptions<MyExpensesContext> options)
        : base(options) { }

    public DbSet<Income> Incomes { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}
