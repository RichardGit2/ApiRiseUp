using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RiseUpAPI.Data;
using RiseUpAPI.Helpers;
using RiseUpAPI.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar a porta para o Render
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Add services to the container.
builder.Services.AddControllers();

// Configure PostgreSQL connection using environment variables or appsettings
var connectionString = ConnectionStringBuilder.Build(builder.Configuration);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        // Configurar retry policy para lidar com falhas temporárias de conexão
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
    });
});

// Adiciona serviços
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOpportunityService, OpportunityService>();

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Usar JWT_KEY da variável de ambiente ou do appsettings
        var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? 
                     builder.Configuration.GetSection("JwtSettings")["Key"];
        
        var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? 
                         builder.Configuration.GetSection("JwtSettings")["Issuer"];
                         
        var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? 
                           builder.Configuration.GetSection("JwtSettings")["Audience"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey ?? "")),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "RiseUp API", 
        Version = "v1",
        Description = "API para a plataforma RiseUp de voluntariado e oportunidades",
        Contact = new OpenApiContact
        {
            Name = "Equipe RiseUp",
            Email = "contato@riseup.com"
        }
    });
    
    // Configuração para usar JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
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

// Configuração CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "RiseUp API v1");
    // Configura a página inicial para apontar para o Swagger UI
    options.RoutePrefix = string.Empty;
});

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Redirecionamento da raiz para o Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

// Adicionar um healthcheck endpoint
app.MapGet("/health", () => Results.Ok(new { 
    status = "healthy", 
    timestamp = DateTime.UtcNow,
    database = "PostgreSQL",
    environment = app.Environment.EnvironmentName
}));

// Aplicar migrações automaticamente com tratamento de exceção robusto
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Tentando aplicar migrações ao banco de dados...");
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Teste a conexão antes de tentar aplicar as migrações
        var canConnect = await context.Database.CanConnectAsync();
        if (canConnect)
        {
            logger.LogInformation("Conexão com o banco de dados estabelecida com sucesso!");
            await context.Database.MigrateAsync();
            logger.LogInformation("Migrações aplicadas com sucesso!");
        }
        else
        {
            logger.LogError("Não foi possível conectar ao banco de dados!");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ocorreu um erro ao aplicar as migrações ou inicializar o banco de dados.");
    }
}

app.Run();