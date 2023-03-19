using System.Security.Cryptography;
using MyExpenses.Entities;
using Microsoft.IdentityModel.Tokens;
using Isopoh.Cryptography.Argon2;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyExpenses.Services;

public class AuthUtils
{
    private readonly IConfiguration _config;

    public AuthUtils(IConfiguration config)
    {
        _config = config;
    }

    public string HashPassword(string password)
    {
        var config = new Argon2Config
        {
            Type = Argon2Type.DataIndependentAddressing,
            Salt = CreateSalt(),
            Version = Argon2Version.Nineteen,
            TimeCost = 10,
            MemoryCost = 32768,
            Lanes = 5,
            Threads = Environment.ProcessorCount,
            Password = Encoding.UTF8.GetBytes(password)
        };
        var argon2 = new Argon2(config);

        using (var hash = argon2.Hash())
        {
            return config.EncodeString(hash.Buffer);
        }
    }

    public bool VerifyPassword(string input, string originalHash)
    {
        return Argon2.Verify(originalHash, Encoding.UTF8.GetBytes(input));
    }

    public string GenerateAccessToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[] { new Claim(ClaimTypes.Name, user.Id.ToString()) };
        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken(Guid id)
    {
        byte[] rand = new byte[32];
        Guid.NewGuid().ToByteArray().CopyTo(rand, 0);
        id.ToByteArray().CopyTo(rand, 16);

        var config = new Argon2Config
        {
            Type = Argon2Type.DataIndependentAddressing,
            Salt = CreateSalt(),
            Version = Argon2Version.Nineteen,
            TimeCost = 10,
            MemoryCost = 32768,
            Lanes = 5,
            Threads = Environment.ProcessorCount,
            Password = rand
        };
        var argon2 = new Argon2(config);

        using (var hash = argon2.Hash())
        {
            return hash.Buffer.ToB64String();
        }
    }

    private byte[] CreateSalt()
    {
        using (var generator = RandomNumberGenerator.Create())
        {
            Span<byte> rand = new Span<byte>(new byte[8]);
            generator.GetBytes(rand);
            return rand.ToArray();
        }
    }
}
