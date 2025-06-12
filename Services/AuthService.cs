using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RiseUpAPI.Data;
using RiseUpAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RiseUpAPI.Services;

public interface IAuthService
{
    Task<User?> ValidateUser(string email, string password);
    string GenerateJwtToken(User user);
}

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthService> _logger;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, ILogger<AuthService> logger, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<User?> ValidateUser(string email, string password)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                _logger.LogWarning("Usuário não encontrado: {Email}", email);
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                _logger.LogWarning("Senha inválida para o usuário: {Email}", email);
                return null;
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar usuário");
            return null;
        }
    }
    
    public string GenerateJwtToken(User user)
    {
        // Obtém os valores das variáveis de ambiente ou das configurações
        var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? 
                     _configuration.GetSection("JwtSettings")["Key"];
                     
        var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? 
                        _configuration.GetSection("JwtSettings")["Issuer"];
                        
        var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? 
                          _configuration.GetSection("JwtSettings")["Audience"];
                          
        var durationStr = Environment.GetEnvironmentVariable("JWT_DURATION") ?? 
                         _configuration.GetSection("JwtSettings")["DurationInMinutes"];

        var duration = int.TryParse(durationStr, out int minutes) ? minutes : 60;
        
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey ?? 
            throw new InvalidOperationException("JWT Key não encontrada")));
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddMinutes(duration),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
            Issuer = jwtIssuer,
            Audience = jwtAudience
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}