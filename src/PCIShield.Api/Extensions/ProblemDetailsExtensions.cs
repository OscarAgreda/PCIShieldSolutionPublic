using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace PCIShield.Api.Extensions
{
    public static class ProblemDetailsExtensions
    {
        public static ProblemDetails CreateValidationProblemDetails(
            this HttpContext context,
            string title,
            string detail,
            IDictionary<string, string[]> errors)
        {
            return new ValidationProblemDetails(errors)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = title,
                Status = StatusCodes.Status400BadRequest,
                Detail = detail,
                Instance = context.Request.Path,
                Extensions =
                {
                    ["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier,
                    ["timestamp"] = DateTimeOffset.UtcNow
                }
            };
        }
        public static ProblemDetails CreateAuthenticationProblemDetails(
            this HttpContext context,
            string detail)
        {
            return new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                Title = "Authentication failed",
                Status = StatusCodes.Status401Unauthorized,
                Detail = detail,
                Instance = context.Request.Path,
                Extensions =
                {
                    ["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier,
                    ["timestamp"] = DateTimeOffset.UtcNow
                }
            };
        }
        public static ProblemDetails CreateAuthorizationProblemDetails(
            this HttpContext context,
            string detail)
        {
            return new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                Title = "Access denied",
                Status = StatusCodes.Status403Forbidden,
                Detail = detail,
                Instance = context.Request.Path,
                Extensions =
                {
                    ["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier,
                    ["timestamp"] = DateTimeOffset.UtcNow
                }
            };
        }
        public static ProblemDetails CreateNotFoundProblemDetails(
            this HttpContext context,
            string resourceType,
            string resourceId)
        {
            return new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Resource not found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"{resourceType} with id '{resourceId}' was not found",
                Instance = context.Request.Path,
                Extensions =
                {
                    ["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier,
                    ["timestamp"] = DateTimeOffset.UtcNow,
                    ["resourceType"] = resourceType,
                    ["resourceId"] = resourceId
                }
            };
        }
        public static ProblemDetails CreateConflictProblemDetails(
            this HttpContext context,
            string detail)
        {
            return new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                Title = "Conflict",
                Status = StatusCodes.Status409Conflict,
                Detail = detail,
                Instance = context.Request.Path,
                Extensions =
                {
                    ["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier,
                    ["timestamp"] = DateTimeOffset.UtcNow
                }
            };
        }
        public static ProblemDetails CreateServerErrorProblemDetails(
            this HttpContext context,
            string detail,
            Exception exception = null)
        {
            var problemDetails = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "An error occurred while processing your request",
                Status = StatusCodes.Status500InternalServerError,
                Detail = detail,
                Instance = context.Request.Path,
                Extensions =
                {
                    ["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier,
                    ["timestamp"] = DateTimeOffset.UtcNow
                }
            };
            if (exception != null && context.RequestServices
                .GetService(typeof(IWebHostEnvironment)) is IWebHostEnvironment env 
                && env.EnvironmentName == "Development")
            {
                problemDetails.Extensions["exception"] = new
                {
                    message = exception.Message,
                    type = exception.GetType().Name,
                    stackTrace = exception.StackTrace
                };
            }

            return problemDetails;
        }
    }
}