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
public class CategoriesController : ControllerBase
{
    private readonly MyExpensesContext _context;

    public CategoriesController(MyExpensesContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryInput input)
    {
        Guid userId = Guid.Parse(User.Identity.Name);
        var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == userId);
        var category = new Category { Name = input.Name };

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

        return Ok(category);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var id = Guid.Parse(User.Identity.Name);
        return Ok(await _context.Categories.ToListAsync());
    }
}
