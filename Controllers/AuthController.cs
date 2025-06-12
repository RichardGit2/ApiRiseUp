using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RiseUpAPI.Data;
using RiseUpAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RiseUpAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(DatabaseContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login/user")]
    public async Task<IActionResult> LoginUser([FromBody] LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || user.Password != request.Password) // Em produção, use hash de senha
        {
            return Unauthorized();
        }

        var token = GenerateJwtToken(user.Id.ToString(), "User");
        return Ok(new { Token = token });
    }

    [HttpPost("login/organization")]
    public async Task<IActionResult> LoginOrganization([FromBody] LoginRequest request)
    {
        var organization = await _context.Organizations.FirstOrDefaultAsync(o => o.Email == request.Email);
        if (organization == null || organization.Password != request.Password) // Em produção, use hash de senha
        {
            return Unauthorized();
        }

        var token = GenerateJwtToken(organization.Id.ToString(), "Organization");
        return Ok(new { Token = token });
    }

    private string GenerateJwtToken(string userId, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
} 