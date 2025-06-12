using Microsoft.EntityFrameworkCore;
using RiseUpAPI.Data;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RiseUpAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar a porta para o Render
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Add services to the container.
builder.Services.AddControllers();

// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Configurar DbContext
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
if (string.IsNullOrEmpty(connectionString))
{
    var dbServer = Environment.GetEnvironmentVariable("DB_SERVER");
    var dbName = Environment.GetEnvironmentVariable("DB_NAME");
    var dbUser = Environment.GetEnvironmentVariable("DB_USER");
    var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

    if (string.IsNullOrEmpty(dbServer) || string.IsNullOrEmpty(dbName) || 
        string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPassword))
    {
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    }
    else
    {
        connectionString = $"Host={dbServer};Database={dbName};Username={dbUser};Password={dbPassword};SSL Mode=Require;Trust Server Certificate=true";
    }
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
    });
});

// Configurar JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not found"));

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"]
    };
});

// Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RiseUp API", Version = "v1" });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Registrar serviços
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOpportunityService, OpportunityService>();

var app = builder.Build();

// Aplicar migrações automaticamente
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        logger.LogInformation("Verificando conexão com o banco de dados...");
        if (context.Database.CanConnect())
        {
            logger.LogInformation("Conexão com o banco de dados estabelecida com sucesso.");
        }
        else
        {
            logger.LogError("Não foi possível conectar ao banco de dados.");
            throw new Exception("Não foi possível conectar ao banco de dados.");
        }

        logger.LogInformation("Verificando migrações pendentes...");
        var pendingMigrations = context.Database.GetPendingMigrations().ToList();
        if (pendingMigrations.Any())
        {
            logger.LogInformation("Migrações pendentes encontradas: {Migrations}", string.Join(", ", pendingMigrations));
            logger.LogInformation("Aplicando migrações...");
            context.Database.Migrate();
            logger.LogInformation("Migrações aplicadas com sucesso.");
        }
        else
        {
            logger.LogInformation("Não há migrações pendentes.");
        }

        // Verificar se a tabela Users existe
        var tableExists = context.Database.SqlQueryRaw<bool>(
            "SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'Users')").FirstOrDefault();
        
        if (!tableExists)
        {
            logger.LogError("A tabela Users não existe após a aplicação das migrações.");
            throw new Exception("A tabela Users não existe após a aplicação das migrações.");
        }
        
        logger.LogInformation("A tabela Users existe no banco de dados.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ocorreu um erro ao configurar o banco de dados. Detalhes: {Message}", ex.Message);
        if (ex.InnerException != null)
        {
            logger.LogError("Erro interno: {InnerMessage}", ex.InnerException.Message);
        }
        throw; // Re-throw para garantir que a aplicação não inicie com o banco de dados não configurado
    }
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RiseUp API v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

// Habilitar CORS
app.UseCors("AllowAll");

// Habilitar autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run(); 