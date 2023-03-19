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
public class IncomesController : ControllerBase
{
    private readonly MyExpensesContext _context;

    public IncomesController(MyExpensesContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateIncomeInput input)
    {
        var userId = Guid.Parse(User.Identity.Name);
        var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == userId);

        var income = new Income
        {
            Amount = input.Amount,
            RecievedAt = input.RecievedAt,
            User = user
        };

        await _context.Incomes.AddAsync(income);
        await _context.SaveChangesAsync();

        return Ok(income);
    }

    [HttpGet]
    public async Task<IActionResult> Get(PaginationArgs args)
    {
        var userId = Guid.Parse(User.Identity.Name);
        var count = await _context.Expenses.Where(x => x.User.Id == userId).CountAsync();
        args.Total = count;

        var items = await _context.Expenses
            .Where(x => x.User.Id == userId)
            .Skip((args.Page - 1) * args.PageSize)
            .Take(args.PageSize)
            .ToListAsync();

        return Ok(new { total = count, items = items });
    }
}
