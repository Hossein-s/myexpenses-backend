namespace MyExpenses.Controllers;

using MyExpenses.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class OverviewController : Controller
{
    private readonly MyExpensesContext _context;

    public OverviewController(MyExpensesContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int year, int month)
    {
        var id = Guid.Parse(User.Identity.Name);
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

        Console.WriteLine(year.ToString());
        Console.WriteLine(month.ToString());

        // get month expenses grouped by category
        var expensesByCategory = await _context.Expenses
            .Where(e => e.SpentAt.Month == month && e.SpentAt.Year == year)
            .GroupBy(e => e.Category)
            .Select(ge => new { Category = ge.Key, Amount = ge.Sum(e => e.Amount) })
            .ToListAsync();

        // get expenses and incomes grouped by month
        var expensesAndIncomesByMonth = await _context.Expenses
            .Where(x => x.SpentAt > new DateTime(year, 1, 1).ToUniversalTime())
            .GroupBy(x => x.SpentAt.Month)
            .Select(
                x =>
                    new
                    {
                        Month = x.Key,
                        TotalExpense = x.Sum(e => e.Amount),
                        TotalIncomes = x.Sum(i => i.Amount)
                    }
            )
            .ToListAsync();

        return Ok(
            new
            {
                ExpensesByCategory = expensesByCategory,
                ExpensesByMonth = expensesAndIncomesByMonth.Select(
                    x => new { Month = x.Month, Expenses = x.TotalExpense }
                ),
                IncomesByMonth = expensesAndIncomesByMonth.Select(
                    x => new { Month = x.Month, Incomes = x.TotalIncomes }
                )
            }
        );
    }
}
