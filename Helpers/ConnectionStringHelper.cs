using System;

namespace RiseUpAPI.Helpers;

public static class ConnectionStringHelper
{
    /// <summary>
    /// Verifica se a string de conexão está configurada como variável de ambiente e a retorna se estiver,
    /// caso contrário, retorna a string de conexão configurada no appsettings.json.
    /// </summary>
    public static string GetConnectionString(IConfiguration configuration)
    {
        // Verificar se existe uma string de conexão externa (ex: POSTGRESQLCONNSTR_DefaultConnection)
        var connectionString = Environment.GetEnvironmentVariable("POSTGRESQLCONNSTR_DefaultConnection");
        
        // Verificar se existe uma variável de ambiente DATABASE_URL (comum em plataformas como Heroku/Render)
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
            
            // Se DATABASE_URL estiver definido, converte para o formato Npgsql
            if (!string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConvertDatabaseUrlToConnectionString(connectionString);
            }
        }
        
        // Se nenhuma variável de ambiente for encontrada, usa a string de conexão do appsettings.json
        return connectionString ?? configuration.GetConnectionString("DefaultConnection");
    }
    
    /// <summary>
    /// Converte uma URL de banco de dados no formato postgres://username:password@host:port/database
    /// para o formato de string de conexão do Npgsql.
    /// </summary>
    private static string ConvertDatabaseUrlToConnectionString(string databaseUrl)
    {
        // Formato esperado: postgres://username:password@host:port/database
        try
        {
            var uri = new Uri(databaseUrl);
            var userInfo = uri.UserInfo.Split(':');
            
            var username = userInfo[0];
            var password = userInfo.Length > 1 ? userInfo[1] : "";
            var host = uri.Host;
            var port = uri.Port;
            var database = uri.AbsolutePath.TrimStart('/');
            
            return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
        }
        catch
        {
            // Se houver qualquer erro na conversão, retorna a URL original
            return databaseUrl;
        }
    }
}
