
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CRN_Technosoft.API.Middleware;

public sealed class ApiExceptionHandler(
    ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (httpContext.Response.HasStarted)
        {
            logger.LogError(
                exception,
                "Response already started. TraceId: {TraceId}",
                httpContext.TraceIdentifier);

            return false;
        }

        var (statusCode, title, detail) = exception switch
        {
            ValidationException => (
                StatusCodes.Status400BadRequest,
                "Validation failed",
                "One or more request fields are invalid."),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "You are not authorized to perform this action."),

            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "Not found",
                "The requested resource was not found."),

            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Bad request",
                "The request contains invalid arguments."),

            InvalidOperationException => (
                StatusCodes.Status409Conflict,
                "Conflict",
                "The request conflicts with the current resource state."),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal server error",
                "An unexpected error occurred. Please try again later.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(
                exception,
                "Unhandled exception. TraceId: {TraceId}",
                httpContext.TraceIdentifier);
        }
        else
        {
            logger.LogWarning(
                exception,
                "Request failed with status {StatusCode}. TraceId: {TraceId}",
                statusCode,
                httpContext.TraceIdentifier);
        }

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        var traceId = httpContext.TraceIdentifier;

        if (exception is ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(e => e.ErrorMessage)
                        .Distinct()
                        .ToArray());

            var problem = new ValidationProblemDetails(errors)
            {
                Status = statusCode,
                Title = title,
                Instance = httpContext.Request.Path
            };

            problem.Extensions["traceId"] = traceId;

            await httpContext.Response.WriteAsJsonAsync(
                problem,
                cancellationToken);
        }
        else
        {
            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            };

            problem.Extensions["traceId"] = traceId;

            await httpContext.Response.WriteAsJsonAsync(
                problem,
                cancellationToken);
        }

        return true;
    }
}
