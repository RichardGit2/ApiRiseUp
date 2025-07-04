using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RiseUpAPI.Data;
using RiseUpAPI.Models;
using RiseUpAPI.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RiseUpAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        ApplicationDbContext context, 
        IConfiguration configuration, 
        IAuthService authService,
        ILogger<AuthController> logger)
    {
        _context = context;
        _configuration = configuration;
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                _logger.LogWarning("Modelo inválido: {Errors}", string.Join(", ", errors));
                return BadRequest(new { message = "Dados inválidos", errors });
            }

            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                _logger.LogWarning("Email ou senha não fornecidos");
                return BadRequest(new { message = "Email e senha são obrigatórios" });
            }

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                _logger.LogWarning("Email já está em uso: {Email}", request.Email);
                return BadRequest(new { message = "Email já está em uso" });
            }

            if (await _context.Users.AnyAsync(u => u.CPF == request.CPF))
            {
                _logger.LogWarning("CPF já está em uso: {CPF}", request.CPF);
                return BadRequest(new { message = "CPF já está em uso" });
            }

            var user = new User
            {
                CPF = request.CPF,
                FullName = request.FullName,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                ZipCode = request.ZipCode,
                Street = request.Street,
                City = request.City,
                State = request.State,
                Country = request.Country,
                Role = request.Role ?? "User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Usuário registrado com sucesso: {Email}", user.Email);
            return Ok(new { message = "Usuário registrado com sucesso", user = new { id = user.Id, email = user.Email } });
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Erro ao salvar no banco de dados");
            return StatusCode(500, new { message = "Erro ao salvar no banco de dados", details = ex.InnerException?.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar usuário");
            return StatusCode(500, new { message = "Erro interno do servidor ao registrar usuário", details = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User request)
    {
        try
        {
            var user = await _authService.ValidateUser(request.Email, request.Password);
            if (user == null)
            {
                _logger.LogWarning("Tentativa de login inválida para o email: {Email}", request.Email);
                return Unauthorized(new { message = "Email ou senha inválidos" });
            }

            // Usar o método no serviço para gerar o token JWT
            var tokenString = _authService.GenerateJwtToken(user);

            _logger.LogInformation("Login bem-sucedido para o usuário: {Email}", user.Email);
            return Ok(new
            {
                token = tokenString,
                user = new
                {
                    id = user.Id,
                    name = user.FullName,
                    email = user.Email,
                    role = user.Role
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer login");
            return StatusCode(500, new { message = "Erro interno do servidor ao fazer login" });
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Tentativa de acessar perfil sem ID de usuário");
                return Unauthorized();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == int.Parse(userId));
            if (user == null)
            {
                _logger.LogWarning("Usuário não encontrado: {UserId}", userId);
                return NotFound();
            }

            return Ok(new
            {
                id = user.Id,
                email = user.Email,
                fullName = user.FullName,
                role = user.Role
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter usuário atual");
            return StatusCode(500, new { message = "Erro interno do servidor ao obter usuário atual" });
        }
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}