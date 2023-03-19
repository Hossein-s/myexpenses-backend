namespace MyExpenses.Controllers;

using MyExpenses.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ProfileController : Controller
{
    private readonly MyExpensesContext _context;

    public ProfileController(MyExpensesContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var id = Guid.Parse(User.Identity.Name);
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

        return Ok(user);
    }
}
