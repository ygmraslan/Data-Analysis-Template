using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Common.Snapshots;
using DataAnalysis.Application.Features.Agency.Abstractions;
using DataAnalysis.Application.Features.AuditLogs.Abstractions;
using DataAnalysis.Application.Features.Auth.Abstractions;
using DataAnalysis.Application.Features.AuthLogs.Abstractions;
using DataAnalysis.Application.Features.Brand.Abstractions;
using DataAnalysis.Application.Features.City.Abstractions;
using DataAnalysis.Application.Features.Company.Abstractions;
using DataAnalysis.Application.Features.CustomSegment.Abstractions;
using DataAnalysis.Application.Features.Dashboard.Abstractions;
using DataAnalysis.Application.Features.Demographic.Abstractions;
using DataAnalysis.Application.Features.ExecSummary.Abstractions;
using DataAnalysis.Application.Features.Permissions.Abstractions;
using DataAnalysis.Application.Features.Region.Abstractions;
using DataAnalysis.Application.Features.Users.Abstractions;
using DataAnalysis.Application.Features.Vehicle.Abstractions;
using DataAnalysis.Infrastructure.Authorization;
using DataAnalysis.Infrastructure.Logging;
using DataAnalysis.Infrastructure.Octopus;
using DataAnalysis.Infrastructure.Repositories;
using DataAnalysis.Infrastructure.Services;
using DataAnalysis.Infrastructure.Snapshots;
using DataAnalysis.Infrastructure.Snapshots.Agency;
using DataAnalysis.Infrastructure.Snapshots.Brand;
using DataAnalysis.Infrastructure.Snapshots.City;
using DataAnalysis.Infrastructure.Snapshots.Company;
using DataAnalysis.Infrastructure.Snapshots.Dashboard;
using DataAnalysis.Infrastructure.Snapshots.Demographic;
using DataAnalysis.Infrastructure.Snapshots.Region;
using DataAnalysis.Infrastructure.Snapshots.Vehicle;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

namespace DataAnalysis.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isProd)
    {
        // PII Masking Logger 
        services.AddTransient(typeof(ILogger<>), typeof(MaskingLogger<>));

        // Octopus
        services.AddSingleton<OctopusConnection>();

        // Redis
        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var redisConnection = configuration.GetConnectionString("RedisConnection")
                ?? throw new InvalidOperationException("RedisConnection string is not configured.");
            return ConnectionMultiplexer.Connect(redisConnection);
        });
        
        services.AddScoped<ISnapshotReader, RedisSnapshotReader>();
        services.AddScoped<ISnapshotStore, RedisSnapshotStore>();
        services.AddSingleton<ISnapshotLazyCache, RedisSnapshotLazyCache>();
        services.AddHostedService<SnapshotRefreshService>();

        services.AddScoped<ISnapshotSource, DashboardSnapshotSource>();
        services.AddScoped<ISnapshotSource, RegionSnapshotSource>();
        services.AddScoped<ISnapshotSource, DemographicSnapshotSource>();
        services.AddScoped<ISnapshotSource, BrandSnapshotSource>();
        services.AddScoped<ISnapshotSource, CitySnapshotSource>();
        services.AddScoped<ISnapshotSource, VehicleSnapshotSource>();
        services.AddScoped<ISnapshotSource, CompanySnapshotSource>();
        services.AddScoped<ISnapshotSource, AgencySnapshotSource>();

        // Services
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IMfaService, MfaService>();
        services.AddScoped<IExcelExportService, ExcelExportService>();
        services.AddScoped<IPdfExportService,PdfExportService>();
        services.AddHttpClient<IAiSummaryService, OllamaService>();
        services.AddHttpClient<ICustomSegmentAiService, CustomSegmentAiService>();
        services.AddScoped<IWeekCalculatorService, WeekCalculatorService>();

        if (isProd)
            services.AddScoped<IEmailService, EmailService>();
        else
            services.AddScoped<IEmailService, LogEmailService>();

        // Repositories
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();

        services.AddScoped<DashboardRepository>();
        services.AddScoped<IDashboardRepository>(sp =>
            new CachedDashboardRepository(
                sp.GetRequiredService<DashboardRepository>(),
                sp.GetRequiredService<ISnapshotReader>()));

        services.AddScoped<RegionRepository>();
        services.AddScoped<IRegionRepository>(sp =>
            new CachedRegionRepository(
                sp.GetRequiredService<RegionRepository>(),
                sp.GetRequiredService<ISnapshotReader>()));    

        services.AddScoped<BrandRepository>();
        services.AddScoped<IBrandRepository>(sp =>
            new CachedBrandRepository(
                sp.GetRequiredService<BrandRepository>(),
                sp.GetRequiredService<ISnapshotReader>(),
                sp.GetRequiredService<ISnapshotLazyCache>()));

        services.AddScoped<CityRepository>();
        services.AddScoped<ICityRepository>(sp =>
            new CachedCityRepository(
                sp.GetRequiredService<CityRepository>(),
                sp.GetRequiredService<ISnapshotReader>(),
                sp.GetRequiredService<ISnapshotLazyCache>()));

        services.AddScoped<VehicleRepository>();
        services.AddScoped<IVehicleRepository>(sp =>
            new CachedVehicleRepository(
                sp.GetRequiredService<VehicleRepository>(),
                sp.GetRequiredService<ISnapshotReader>(),
                sp.GetRequiredService<ISnapshotLazyCache>()));

        services.AddScoped<CompanyRepository>();
        services.AddScoped<ICompanyRepository>(sp =>
            new CachedCompanyRepository(
                sp.GetRequiredService<CompanyRepository>(),
                sp.GetRequiredService<ISnapshotReader>(),
                sp.GetRequiredService<ISnapshotLazyCache>()));

        services.AddScoped<AgencyRepository>();
        services.AddScoped<IAgencyRepository>(sp =>
            new CachedAgencyRepository(
                sp.GetRequiredService<AgencyRepository>(),
                sp.GetRequiredService<ISnapshotReader>(),
                sp.GetRequiredService<ISnapshotLazyCache>()));

        services.AddScoped<DemoRepository>();
        services.AddScoped<IDemoRepository>(sp =>
            new CachedDemoRepository(
                sp.GetRequiredService<DemoRepository>(),
                sp.GetRequiredService<ISnapshotReader>()));
                
        services.AddScoped<IExecSummaryRepository, ExecSummaryRepository>();
        services.AddScoped<IExecAiCacheRepository,ExecAiCacheRepository>();
        services.AddScoped<ICustomSegmentRepository,CustomSegmentRepository>();
        services.AddScoped<ICustomSegmentDbRepository,CustomSegmentDbRepository>();
        services.AddScoped<IComparisonRepository,ComparisonRepository>();
        services.AddScoped<IAuthLogRepository, AuthLogRepository>();

        // Authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]!)),
                ValidateIssuer = true,
                ValidIssuer = configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["JwtSettings:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var token = context.Request.Cookies["accessToken"];
                    if (!string.IsNullOrEmpty(token))
                        context.Token = token;
                    return Task.CompletedTask;
                }
            };
        });

        // Authorization
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddAuthorization();

        return services;
    }
}