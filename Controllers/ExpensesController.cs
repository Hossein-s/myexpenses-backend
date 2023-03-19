namespace MyExpenses.Controllers;

using MyExpenses.Entities;
using MyExpenses.Data;
using MyExpenses.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly MyExpensesContext _context;

    public ExpensesController(MyExpensesContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateExpenseInput input)
    {
        var userId = Guid.Parse(User.Identity.Name);
        var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == userId);
        var category = await _context.Categories.SingleOrDefaultAsync(x => x.Id == input.Category);

        var expense = new Expense
        {
            Amount = input.Amount,
            SpentAt = input.SpentAt,
            Category = category,
            User = user,
        };

        await _context.Expenses.AddAsync(expense);
        await _context.SaveChangesAsync();

        return Ok(expense);
    }

    [HttpGet]
    public async Task<IActionResult> Get(PaginationArgs args)
    {
        var id = Guid.Parse(User.Identity.Name);
        var count = await _context.Expenses.Where(x => x.User.Id == id).CountAsync();
        args.Total = count;

        var items = await _context.Expenses
            .Where(x => x.User.Id == id)
            .Include(x => x.Category)
            .Skip((args.Page - 1) * args.PageSize)
            .Take(args.PageSize)
            .ToListAsync();

        return Ok(new { total = count, items = items });
    }
}
