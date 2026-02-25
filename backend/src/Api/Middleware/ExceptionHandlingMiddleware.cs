using System.Text.Json;
using Api.Common;
using Application.Auth;
using FluentValidation;

namespace Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        var (statusCode, errorCode, message, details) = exception switch
        {
            ValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                ErrorCodes.ValidationError,
                "One or more validation errors occurred.",
                (object?)validationEx.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            ),
            UnauthorizedException => (
                StatusCodes.Status401Unauthorized,
                ErrorCodes.Unauthorized,
                exception.Message,
                (object?)null
            ),
            ForbiddenException => (
                StatusCodes.Status403Forbidden,
                ErrorCodes.Forbidden,
                exception.Message,
                (object?)null
            ),
            NotFoundException => (
                StatusCodes.Status404NotFound,
                ErrorCodes.NotFound,
                exception.Message,
                (object?)null
            ),
            ConflictException => (
                StatusCodes.Status409Conflict,
                ErrorCodes.Conflict,
                exception.Message,
                (object?)null
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                ErrorCodes.InternalError,
                "An unexpected error occurred.",
                (object?)null
            )
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Unhandled exception");
        else
            _logger.LogWarning("Handled exception: {ErrorCode} - {Message}", errorCode, message);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new ApiErrorResponse
        {
            Error = new ApiError
            {
                Code = errorCode,
                Message = message,
                Details = details
            }
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
