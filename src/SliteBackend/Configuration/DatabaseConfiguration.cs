using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SliteBackend.Data;

namespace SliteBackend.Configuration;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = GetConnectionString(configuration);
        
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
            });
            
            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
        });

        return services;
    }

    private static string GetConnectionString(IConfiguration configuration)
    {
        var host = configuration["DB_HOST"] ?? "localhost";
        var port = configuration["DB_PORT"] ?? "5432";
        var database = configuration["DB_NAME"] ?? "slitedb";
        var username = configuration["DB_USERNAME"] ?? "postgres";
        var password = configuration["DB_PASSWORD"] ?? "password";

        return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Prefer;";
    }

    public static async Task InitializeDatabaseAsync(ApplicationDbContext context)
    {
        try
        {
            await context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to initialize database", ex);
        }
    }
}