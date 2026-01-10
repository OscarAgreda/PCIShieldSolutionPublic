/*
 *
 *
 *Ah yes, let's fix the type conversion issue. The problem is that HandleWithEither returns an Either<DomainError, TResponse> but we need to return TResponse. Here's the corrected version:
   ```csharp
   public class ExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
   {
       private readonly ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> _logger;
       private readonly Subject<(Exception Exception, TRequest Request)> _errorStream = new();
       public ExceptionHandlingBehavior(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
       {
           _logger = logger;
           _errorStream
               .Buffer(TimeSpan.FromSeconds(1))
               .Where(errors => errors.Any())
               .Subscribe(errors =>
               {
                   foreach (var (ex, req) in errors)
                   {
                       _logger.LogError(
                           ex,
                           "Error processing request {RequestType}: {ErrorMessage}. Request: {@Request}",
                           typeof(TRequest).Name,
                           ex.Message,
                           req);
                   }
               });
       }
       public async Task<TResponse> Handle(
           TRequest request,
           RequestHandlerDelegate<TResponse> next,
           CancellationToken ct)
       {
           try
           {
               if (typeof(TResponse).IsGenericType && 
                   typeof(TResponse).GetGenericTypeDefinition() == typeof(Either<,>))
               {
                   return await HandleWithEither(request, next, ct);
               }
               return await next();
           }
           catch (Exception ex)
           {
               _errorStream.OnNext((ex, request));
               throw WrapException(ex);
           }
       }
       private async Task<TResponse> HandleWithEither(
           TRequest request,
           RequestHandlerDelegate<TResponse> next,
           CancellationToken ct)
       {
           try
           {
               var result = await next();
               return result;
           }
           catch (Exception ex)
           {
               _errorStream.OnNext((ex, request));
               if (typeof(TResponse).IsGenericType && 
                   typeof(TResponse).GetGenericTypeDefinition() == typeof(Either<,>))
               {
                   var error = DomainError.FromException(ex);
                   var leftMethod = typeof(Either)
                       .GetMethod(nameof(Either.Left))
                       .MakeGenericMethod(typeof(DomainError), typeof(TResponse).GetGenericArguments()[1]);
                   return (TResponse)leftMethod.Invoke(null, new object[] { error });
               }
               throw WrapException(ex);
           }
       }
       private Exception WrapException(Exception ex) => ex switch
       {
           ArgumentException _ => new ProblemDetailsException(ex.Message, StatusCodes.Status400BadRequest, ex),
           KeyNotFoundException _ => new ProblemDetailsException(ex.Message, StatusCodes.Status404NotFound, ex),
           InvalidOperationException _ => new ProblemDetailsException(ex.Message, StatusCodes.Status409Conflict, ex),
           _ => new ProblemDetailsException(ex.Message, StatusCodes.Status500InternalServerError, ex)
       };
   }
   public class UpdateCustomerHandler 
       : IRequestHandler<UpdateCustomerCommand, Either<DomainError, CustomerDto>>
   {
       public async Task<Either<DomainError, CustomerDto>> Handle(
           UpdateCustomerCommand request,
           CancellationToken ct)
       {
           return await TryAsync(async () =>
           {
               var result = await UpdateCustomer(request.Customer);
               return Right<DomainError, CustomerDto>(result);
           });
       }
   }
   ```
   Key changes:
   1. We check the response type using `IsGenericType` and `GetGenericTypeDefinition()`
   2. We properly handle the Either type construction when needed
   3. We maintain backward compatibility for non-Either responses
   Now you can use it with both types of handlers:
   ```csharp
   public class WithEitherHandler : IRequestHandler<MyCommand, Either<DomainError, MyResponse>>
   {
       public async Task<Either<DomainError, MyResponse>> Handle(MyCommand request, CancellationToken ct)
       {
       }
   }
   public class RegularHandler : IRequestHandler<MyCommand, MyResponse>
   {
       public async Task<MyResponse> Handle(MyCommand request, CancellationToken ct)
       {
       }
   }
   ```
 *
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Runtime;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;
using Unit = System.Reactive.Unit;
namespace PCIShield.Infrastructure.Behaviors
{
    public enum ErrorType
    {
        Validation = 400,
        NotFound = 404,
        Conflict = 409,
        Internal = 500
    }
    public record DomainError(
        string Code,
        string Message,
        ErrorType Type,
        Option<string> TraceId = default,
        Option<Exception> Exception = default)
    {
        public static DomainError FromException(Exception ex, string code = "INTERNAL_ERROR") =>
            new(
                code,
                ex.Message,
                ErrorType.Internal,
                Some(Activity.Current?.Id ?? Guid.NewGuid().ToString()),
                Some(ex));
        public static DomainError NotFound(string code, string message) =>
            new(code, message, ErrorType.NotFound);
        public static DomainError Validation(string code, string message) =>
            new(code, message, ErrorType.Validation);
        public static DomainError Conflict(string code, string message) =>
            new(code, message, ErrorType.Conflict);
    }
    public interface IErrorHandlingRequest<TResponse> : IRequest<Either<DomainError, TResponse>> { }
    public class ExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> _logger;
        private readonly Subject<(Exception Exception, TRequest Request)> _errorStream = new();
        public ExceptionHandlingBehavior(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
            _errorStream
                .Buffer(TimeSpan.FromSeconds(1))
                .Where(errors => errors.Any())
                .Subscribe(errors =>
                {
                    foreach (var (ex, req) in errors)
                    {
                        _logger.LogError(
                            ex,
                            "Error processing request {RequestType}: {ErrorMessage}. Request: {@Request}",
                            typeof(TRequest).Name,
                            ex.Message,
                            req);
                    }
                });
        }
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            try
            {
                return await next();
            }
            catch (Exception ex)
            {
                _errorStream.OnNext((ex, request));
                if (IsEither(typeof(TResponse)))
                {
                    var error = DomainError.FromException(ex);
                    var responseType = typeof(TResponse).GetGenericArguments()[1];
                    return (TResponse)(object)Left<DomainError, object>(error);
                }
                throw WrapException(ex);
            }
        }
        private static bool IsEither(Type type) =>
            type.IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(Either<,>) &&
            type.GetGenericArguments()[0] == typeof(DomainError);
        private Exception WrapException(Exception ex) => ex switch
        {
            ArgumentException _ => new ProblemDetailsException(ex.Message, StatusCodes.Status400BadRequest, ex),
            KeyNotFoundException _ => new ProblemDetailsException(ex.Message, StatusCodes.Status404NotFound, ex),
            InvalidOperationException _ => new ProblemDetailsException(ex.Message, StatusCodes.Status409Conflict, ex),
            _ => new ProblemDetailsException(ex.Message, StatusCodes.Status500InternalServerError, ex)
        };
    }
    public class ProblemDetailsException : Exception
    {
        public int StatusCode { get; }
        public ProblemDetailsException(string message, int statusCode, Exception innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
