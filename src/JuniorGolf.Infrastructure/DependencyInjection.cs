using JuniorGolf.Core.Interfaces;
using JuniorGolf.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JuniorGolf.Infrastructure;

/// <summary>
/// Extension method to register all Infrastructure services with the DI container.
///
/// Called from: JuniorGolf.Api/Program.cs
/// Registers:   AppDbContext, Repository<T>
///
/// Data flow:
///   Program.cs calls AddInfrastructure(config)
///     → Reads connection string from config
///     → Registers AppDbContext with Npgsql provider
///     → Registers generic IRepository<T> → Repository<T>
///     → All controllers/services can now inject IRepository<T>
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
            )
        );

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }
}
