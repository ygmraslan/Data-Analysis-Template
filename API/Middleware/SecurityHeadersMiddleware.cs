namespace DataAnalysis.API.Middleware;
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly bool _isProduction;

    public SecurityHeadersMiddleware(RequestDelegate next, IWebHostEnvironment env)
    {
        _next = next;
        _isProduction = env.IsProduction();
    }

    public async Task InvokeAsync(HttpContext context)
        {
            var headers = context.Response.Headers;
            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-Frame-Options"] = "DENY";
            headers["X-XSS-Protection"] = "1; mode=block";
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            headers["Permissions-Policy"] = "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()";

            var path = context.Request.Path;
            var isDocs = !_isProduction &&
                (path.StartsWithSegments("/scalar") || path.StartsWithSegments("/openapi"));

            if (!isDocs)
            {
                headers["Content-Security-Policy"] = "default-src 'self'; frame-ancestors 'none';";
            }

            headers["Cache-Control"] = "no-store, no-cache, must-revalidate, proxy-revalidate";
            headers["Pragma"] = "no-cache";

            if (_isProduction)
            {
                headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";
            }

            await _next(context);
        }
    }

    public static class SecurityHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }