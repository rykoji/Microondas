using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microondas.WebApplication.Exceptions;

namespace Microondas.WebApplication.Middleware;

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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new
        {
            Message = exception.Message,
            InnerException = exception.InnerException?.Message,
            StackTrace = exception.StackTrace,
            Timestamp = DateTime.UtcNow
        };

        LogToFile(exception);

        switch (exception)
        {
            case BusinessException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        _logger.LogError(exception, "Erro capturado pelo middleware");

        var options = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
        await response.WriteAsync(JsonSerializer.Serialize(errorResponse, options));
    }

    private void LogToFile(Exception ex)
    {
        var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        Directory.CreateDirectory(logPath);
        
        var logFile = Path.Combine(logPath, $"error_{DateTime.Now:yyyyMMdd}.log");
        var logEntry = $"""
            [{DateTime.Now:yyyy-MM-dd HH:mm:ss}]
            Exception: {ex.GetType().Name}
            Message: {ex.Message}
            InnerException: {ex.InnerException?.Message}
            StackTrace: {ex.StackTrace}
            -------------------------------------------
            
            """;
        
        File.AppendAllText(logFile, logEntry);
    }
}
