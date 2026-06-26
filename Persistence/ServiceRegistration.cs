using DataAnalysis.Domain.Interfaces;
using DataAnalysis.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DataAnalysis.Persistence;

public static class ServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

        return services;
    }
}