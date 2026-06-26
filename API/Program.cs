using Scalar.AspNetCore;
using DataAnalysis.Persistence;
using DataAnalysis.Application;
using DataAnalysis.Infrastructure;
using DataAnalysis.API.Middleware;
using DataAnalysis.Persistence.SeedData;
using DataAnalysis.Application.Common.Settings;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

var environmentSettings = builder.Configuration
    .GetSection("EnvironmentSettings")
    .Get<EnvironmentSettings>() ?? new EnvironmentSettings();

var isProd = environmentSettings.Prod;

// Reverse proxy (nginx) arkasında mı çalışıyor?
var isBehindProxy = builder.Configuration.GetValue<bool>("ReverseProxy:Enabled");

var connectionString = isProd
    ? builder.Configuration.GetConnectionString("AppConnectionProd")
    : builder.Configuration.GetConnectionString("AppConnection");

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddPersistenceServices(connectionString!);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration, isProd);

// Reverse proxy arkasındayken forwarded header'ları tanı
if (isBehindProxy)
{
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders =
            Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
            Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();
    });
}

// Cookie Policy
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
    options.Secure = isProd && !isBehindProxy
        ? CookieSecurePolicy.Always
        : CookieSecurePolicy.None;
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });

    options.AddPolicy("ProductionPolicy", policy =>
    {
        policy.WithOrigins(builder.Configuration["AppSettings:DefaultUrlProd"]!)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });

    // Docker reverse proxy arkasında: nginx aynı origin'den proxy yaptığı için
    // tarayıcı cross-origin istek görmez, ama container'lar arası iletişim için
    // yine de izin veriyoruz
    options.AddPolicy("DockerPolicy", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("GeneralLimit", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    options.AddPolicy("AuthLimit", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    options.AddPolicy("ForgotPasswordLimit", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                Window = TimeSpan.FromHours(1),
                QueueLimit = 0
            }));

    
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";
        
        var retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retry) 
            ? retry.TotalSeconds 
            : 60;
        
        context.HttpContext.Response.Headers.RetryAfter = retryAfter.ToString();
        
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            StatusCode = 429,
            Message = "Too many requests. Please try again later.",
            RetryAfterSeconds = retryAfter
        }, cancellationToken);
    };
});

var app = builder.Build();

// Reverse proxy arkasında forwarded header'ları en başta işle
if (isBehindProxy)
{
    app.UseForwardedHeaders();
}

if (!isProd)
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("DataAnalysis API")
               .WithTheme(ScalarTheme.Moon);
    });
    app.UseCors("DevelopmentPolicy");
}
else if (isBehindProxy)
{
    // Docker + nginx reverse proxy: aynı origin, CORS fiilen gereksiz
    // ama container arası istekler için açık bırakıyoruz
    app.UseCors("DockerPolicy");
}
else
{
    app.UseCors("ProductionPolicy");
}


app.UseSecurityHeaders();

app.UseMiddleware<ExceptionMiddleware>();

// Reverse proxy arkasında HTTPS yönlendirme yapma — nginx halleder
if (!isBehindProxy)
{
    app.UseHttpsRedirection();
}

app.UseRateLimiter(); 
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuditLogMiddleware>();
app.MapControllers();

await DataSeeder.SeedAsync(app.Services);

app.Run();
