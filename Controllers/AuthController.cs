using MyExpenses.Services;
using MyExpenses.Data;
using MyExpenses.Entities;
using MyExpenses.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : Controller
{
    private readonly AuthUtils _authUtils;
    private readonly MyExpensesContext _context;

    public AuthController(AuthUtils authUtils, MyExpensesContext context)
    {
        _authUtils = authUtils;
        _context = context;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(RegisterInput input)
    {
        var count = await _context.Users.Where(x => x.Email == input.Email).CountAsync();

        if (count != 0)
        {
            return BadRequest();
        }

        var user = new User
        {
            FirstName = input.FirstName,
            LastName = input.LastName,
            Email = input.Email,
            Password = _authUtils.HashPassword(input.Password)
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var refreshToken = _authUtils.GenerateRefreshToken(user.Id);
        var refreshTokenObj = new RefreshToken
        {
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            User = user,
        };
        await _context.RefreshTokens.AddAsync(refreshTokenObj);
        await _context.SaveChangesAsync();

        var accessToken = _authUtils.GenerateAccessToken(user);

        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginInput input)
    {
        var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == input.Email);

        if (user == null)
        {
            return Unauthorized();
        }

        if (!_authUtils.VerifyPassword(input.Password, user.Password))
        {
            return Unauthorized();
        }

        // generate access token
        var accessToken = _authUtils.GenerateAccessToken(user);

        // generate refresh token
        var refreshToken = _authUtils.GenerateRefreshToken(user.Id);
        var refreshTokenObj = new RefreshToken
        {
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            User = user,
        };
        await _context.RefreshTokens.AddAsync(refreshTokenObj);
        await _context.SaveChangesAsync();

        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken, });
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken(string token)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.Token == token);

        if (refreshToken == null || refreshToken.ExpiresAt < DateTime.Now)
        {
            return Unauthorized();
        }

        // generate access token
        var accessToken = _authUtils.GenerateAccessToken(refreshToken.User);

        // generate refresh token
        var newRefreshToken = _authUtils.GenerateRefreshToken(refreshToken.User.Id);
        var refreshTokenObj = new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            User = refreshToken.User,
        };

        // delete used refresh token
        await _context.RefreshTokens.Where(x => x.Id == refreshToken.Id).ExecuteDeleteAsync();
        await _context.RefreshTokens.AddAsync(refreshTokenObj);
        await _context.SaveChangesAsync();

        return Ok(new { AccessToken = accessToken, RefreshToken = newRefreshToken });
    }
}
