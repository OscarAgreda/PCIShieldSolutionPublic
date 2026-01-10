using System;
using System.Linq;
using PCIShield.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PCIShield.Api.Auth.ApplicationUserOnly
{
    public static class ProblemDetailsResponseExample
    {
        public static IActionResult ValidationErrorExample(ControllerBase controller, ModelStateDictionary modelState)
        {
            var errors = modelState
                .Where(kvp => kvp.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            var problemDetails = controller.HttpContext.CreateValidationProblemDetails(
                title: "Validation failed",
                detail: "One or more validation errors occurred",
                errors: errors
            );

            return new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status
            };
        }
        public static IActionResult AuthenticationErrorExample(ControllerBase controller, string detail)
        {
            var problemDetails = controller.HttpContext.CreateAuthenticationProblemDetails(detail);
            
            return new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status
            };
        }
        public static IActionResult NotFoundExample(ControllerBase controller, string resourceType, string resourceId)
        {
            var problemDetails = controller.HttpContext.CreateNotFoundProblemDetails(resourceType, resourceId);
            
            return new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status
            };
        }
        public static IActionResult ServerErrorExample(ControllerBase controller, Exception ex)
        {
            var problemDetails = controller.HttpContext.CreateServerErrorProblemDetails(
                detail: "An unexpected error occurred",
                exception: ex
            );
            
            return new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status
            };
        }
        public static IActionResult LoginWithProblemDetails(
            ControllerBase controller,
            LoginRequest model,
            bool isSuccess,
            string errorMessage = null)
        {
            if (!controller.ModelState.IsValid)
            {
                return ValidationErrorExample(controller, controller.ModelState);
            }

            if (!isSuccess)
            {
                return AuthenticationErrorExample(controller, errorMessage ?? "Invalid username or password");
            }
            return controller.Ok(new
            {
                success = true,
                message = "Login successful"
            });
        }
    }
}