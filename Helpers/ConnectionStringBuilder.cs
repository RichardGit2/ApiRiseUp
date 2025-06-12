namespace RiseUpAPI.Helpers;

public static class ConnectionStringBuilder
{
    /// <summary>
    /// Constrói a string de conexão a partir das variáveis de ambiente ou configuração
    /// </summary>
    public static string Build(IConfiguration configuration)
    {
        // Verifica se há variáveis de ambiente definidas no Render
        var server = Environment.GetEnvironmentVariable("DB_SERVER") ?? configuration.GetValue<string>("Database:Server");
        var database = Environment.GetEnvironmentVariable("DB_NAME") ?? configuration.GetValue<string>("Database:Name");
        var user = Environment.GetEnvironmentVariable("DB_USER") ?? configuration.GetValue<string>("Database:User");
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? configuration.GetValue<string>("Database:Password");

        // Verifica DATABASE_URL (usado em alguns provedores como Heroku)
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        if (!string.IsNullOrEmpty(databaseUrl))
        {
            return ConvertDatabaseUrlToConnectionString(databaseUrl);
        }

        // Se as variáveis estiverem definidas, constrói a string de conexão
        if (!string.IsNullOrEmpty(server) && !string.IsNullOrEmpty(database) && 
            !string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
        {
            return $"Host={server};Database={database};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true";
        }

        // Retorna a string de conexão padrão se nenhuma variável de ambiente for encontrada
        return configuration.GetConnectionString("DefaultConnection") ?? 
               throw new InvalidOperationException("Nenhuma string de conexão válida foi encontrada");
    }

    /// <summary>
    /// Converte uma URL de banco de dados no formato postgres://username:password@host:port/database
    /// para o formato de string de conexão do Npgsql
    /// </summary>
    private static string ConvertDatabaseUrlToConnectionString(string databaseUrl)
    {
        try
        {
            var uri = new Uri(databaseUrl);
            var userInfoParts = uri.UserInfo.Split(':');
            
            var username = userInfoParts[0];
            var password = userInfoParts.Length > 1 ? userInfoParts[1] : string.Empty;
            var host = uri.Host;
            var port = uri.Port > 0 ? uri.Port : 5432;
            var database = uri.AbsolutePath.TrimStart('/');
            
            return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
        }
        catch
        {
            return databaseUrl;
        }
    }
}
