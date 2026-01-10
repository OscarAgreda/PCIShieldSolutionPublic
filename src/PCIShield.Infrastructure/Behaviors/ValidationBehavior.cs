/*
   Now you can use it with your existing validators:
   ```csharp
   public class UpdateCustomerValidator : AbstractValidator<CustomerDto>
   {
       public UpdateCustomerValidator()
       {
           this.AddCustomRule(
               customer => !string.IsNullOrEmpty(customer.CustomerCode),
               "Customer Code is required")
               .WithErrorCode("CUSTOMER.CUSTOMERCODE_REQUIRED");
       }
   }
   ```
   Yes, let me enhance the validation pipeline to include the skip validation functionality while keeping the functional approach:
   Key improvements:
   1. **Smart Validation Skipping**:
   ```csharp
   public Task<TResponse> Handle(
       TRequest request, 
       RequestHandlerDelegate<TResponse> next, 
       CancellationToken ct) =>
       ShouldValidate(request) 
           ? HandleWithValidation(request, next, ct)
           : next();
   private bool ShouldValidate(TRequest request) =>
       !Attribute.IsDefined(typeof(TRequest), typeof(SkipValidationAttribute)) &&
       !typeof(TRequest).Name.EndsWith("Query", StringComparison.OrdinalIgnoreCase) &&
       _validators.Any();
   ```
   2. **Helper Extension for MediatR**:
   ```csharp
   public static Task<Either<Seq<ValidationError>, TResponse>> ValidateAndHandle<TRequest, TResponse>(
       this IMediator mediator,
       TRequest request,
       IValidator<TRequest> validator) 
   ```
   3. **Easy Skip Validation**:
   ```csharp
   [SkipValidation]
   public class SkipValidationCommand : IRequest<Result> 
   {
   }
   ```
   Usage examples:
   1. **Skip Validation for a Command**:
   ```csharp
   [SkipValidation]
   public class QuickUpdateCommand : IRequest<Result>
   {
       public int Id { get; set; }
       public string Value { get; set; }
   }
   ```
   2. **Validate and Handle in One Go**:
   ```csharp
   public class CustomerController
   {
       private readonly IMediator _mediator;
       private readonly IValidator<UpdateCustomerCommand> _validator;
       public async Task<IActionResult> UpdateCustomer(UpdateCustomerCommand command)
       {
           var result = await _mediator.ValidateAndHandle(command, _validator);
           return result.Match(
               Right: success => Ok(success),
               Left: errors => BadRequest(errors));
       }
   }
   ```
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using PCIShield.Infrastructure.Services;
using LanguageExt;
using MediatR;
using static LanguageExt.Prelude;
using Unit = System.Reactive.Unit;
namespace PCIShield.Infrastructure.Behaviors
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SkipValidationAttribute : Attribute { }
    public record ValidationError(string PropertyName, string ErrorMessage, string ErrorCode);
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly IAppLoggerService<ValidationBehavior<TRequest, TResponse>> _logger;
        private readonly Subject<ValidationError> _validationErrors = new();
        public ValidationBehavior(
            IEnumerable<IValidator<TRequest>> validators,
            IAppLoggerService<ValidationBehavior<TRequest, TResponse>> logger)
        {
            _validators = validators;
            _logger = logger;
            _validationErrors
                .Buffer(TimeSpan.FromMilliseconds(100))
                .Where(errors => errors.Any())
                .Subscribe(errors =>
                    _logger.LogWarning(
                        "Validation errors: {@Errors}",
                        errors.GroupBy(e => e.PropertyName)
                              .ToDictionary(g => g.Key, g => g.ToList())));
        }
        public Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct) =>
            ShouldValidate(request)
                ? HandleWithValidation(request, next, ct)
                : next();
        private bool ShouldValidate(TRequest request) =>
            !Attribute.IsDefined(typeof(TRequest), typeof(SkipValidationAttribute)) &&
            !typeof(TRequest).Name.EndsWith("Query", StringComparison.OrdinalIgnoreCase) &&
            _validators.Any();
        private async Task<TResponse> HandleWithValidation(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            _logger.LogInformation($"Validating {typeof(TRequest).Name}");
            var validationResult = await ValidateRequest(request, ct);
            return await validationResult.Match(
                Right: _ => next(),
                Left: errors =>
                {
                    var failures = errors.Map(e =>
                        new ValidationFailure(e.PropertyName, e.ErrorMessage) { ErrorCode = e.ErrorCode })
                        .ToList();
                    throw new ValidationException(
                        $"Validation failed for {typeof(TRequest).Name}",
                        failures);
                });
        }
        private async Task<Either<Seq<ValidationError>, Unit>> ValidateRequest(
            TRequest request,
            CancellationToken ct)
        {
            var context = new ValidationContext<TRequest>(request);
            var results = await ValidateWithAllValidators(context, ct);
            return ProcessValidationResults(results);
        }
        private async Task<IEnumerable<ValidationResult>> ValidateWithAllValidators(
            ValidationContext<TRequest> context,
            CancellationToken ct) =>
            await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, ct)));
        private Either<Seq<ValidationError>, Unit> ProcessValidationResults(
            IEnumerable<ValidationResult> results)
        {
            var errors = results
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .Select(failure => new ValidationError(
                    failure.PropertyName,
                    failure.ErrorMessage,
                    failure.ErrorCode))
                .ToSeq();
            errors.Iter(error => _validationErrors.OnNext(error));
            return errors.IsEmpty
                ? Right<Seq<ValidationError>, Unit>(Unit.Default)
                : Left<Seq<ValidationError>, Unit>(errors);
        }
    }
    public static class ValidationBehaviorExtensions
    {
        public static bool ShouldValidate(this IRequest request) =>
            !Attribute.IsDefined(request.GetType(), typeof(SkipValidationAttribute));
        public static Task<Either<Seq<ValidationError>, TResponse>> ValidateAndHandle<TRequest, TResponse>(
            this IMediator mediator,
            TRequest request,
            IValidator<TRequest> validator) where TRequest : IRequest<TResponse> =>
            validator.Validate(request).Errors.Any()
                ? Task.FromResult(Left<Seq<ValidationError>, TResponse>(
                    validator.Validate(request).Errors
                        .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage, e.ErrorCode))
                        .ToSeq()))
                : mediator.Send(request)
                    .Map(response => Right<Seq<ValidationError>, TResponse>(response));
    }
}
