using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CRN_Technosoft.API.Middleware;

public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            ValidationException => (
                StatusCodes.Status400BadRequest,
                "Validation failed",
                "One or more request fields are invalid."),
            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "The supplied credentials are invalid."),
            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "Not found",
                "The requested resource was not found."),
            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Bad request",
                "The request is invalid."),
            InvalidOperationException => (
                StatusCodes.Status409Conflict,
                "Conflict",
                "The request conflicts with the current resource state."),
            _ => (0, string.Empty, string.Empty)
        };

        if (statusCode == 0)
        {
            return false;
        }

        logger.LogWarning(exception, "Request failed with {StatusCode}", statusCode);
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        if (exception is ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(failure => failure.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(failure => failure.ErrorMessage).ToArray());
            await httpContext.Response.WriteAsJsonAsync(
                new ValidationProblemDetails(errors)
                {
                    Status = statusCode,
                    Title = title,
                    Instance = httpContext.Request.Path
                },
                cancellationToken);
        }
        else
        {
            await httpContext.Response.WriteAsJsonAsync(
                new ProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    Detail = detail,
                    Instance = httpContext.Request.Path
                },
                cancellationToken);
        }

        return true;
    }
}
