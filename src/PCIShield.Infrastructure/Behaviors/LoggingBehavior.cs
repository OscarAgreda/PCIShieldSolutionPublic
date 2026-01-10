/*
 *
   This approach:
   1. Keeps all MediatR and behavior registrations together
   2. Maintains the same assembly scanning functionality you had before
   3. Adds the new `LoggingBehavior` in the correct order
   4. Keeps all the existing MediatR behavior registrations
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PCIShield.Infrastructure.Services;
using MediatR;
namespace PCIShield.Infrastructure.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IAppLoggerService<LoggingBehavior<TRequest, TResponse>> _logger;
        public LoggingBehavior(IAppLoggerService<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Handling {RequestName} with content: {@RequestContent}",
                    typeof(TRequest).Name,
                    request);
                var response = await next();
                _logger.LogInformation(
                    "Handled {RequestName} with response: {@Response}",
                    typeof(TRequest).Name,
                    response);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    $"Error handling {typeof(TRequest).Name} with content: {request}");
                throw;
            }
        }
    }
}
