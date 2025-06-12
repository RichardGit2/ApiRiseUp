using Microsoft.EntityFrameworkCore;
using RiseUpAPI.Data;
using RiseUpAPI.Models;

namespace RiseUpAPI.Services;

public interface IAuthService
{
    Task<User?> ValidateUser(string email, string password);
}

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthService> _logger;

    public AuthService(ApplicationDbContext context, ILogger<AuthService> logger)
    {
        _context = context;
        _logger = logger;
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
}