using FluentValidation;
using DataAnalysis.Application.Common.Behaviors;
using DataAnalysis.Application.Common.Settings;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DataAnalysis.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<EnvironmentSettings>(configuration.GetSection("EnvironmentSettings"));
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        services.Configure<SeedDataSettings>(configuration.GetSection("SeedData"));
        services.Configure<OllamaSettings>(configuration.GetSection("OllamaSettings"));
        services.Configure<SnapshotSettings>(configuration.GetSection("SnapshotSettings"));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}