using FluentValidation;
using System.Net;
using System.Text.Json;

namespace DataAnalysis.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error.");
            var errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
            await WriteResponse(context, HttpStatusCode.BadRequest, errors);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt.");
            await WriteResponse(context, HttpStatusCode.Unauthorized, ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument.");
            await WriteResponse(context, HttpStatusCode.BadRequest, "Geçersiz istek parametresi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            await WriteResponse(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task WriteResponse(HttpContext context, HttpStatusCode statusCode, object message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            StatusCode = (int)statusCode,
            Message = message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}