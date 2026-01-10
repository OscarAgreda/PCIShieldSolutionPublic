using PCIShield.Api.CQRS;

using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using IMediator = MediatR.IMediator;
namespace PCIShield.Api
{
    public static class CqrsRegistration
    {
        public static IServiceCollection AddCqrs(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<FastEndpoints.ICommand<object>>()
                .AddClasses(classes => classes.AssignableTo(typeof(FastEndpoints.ICommandHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());
            services.Scan(scan => scan
                .FromAssemblyOf<ICommand<object>>()
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());
            services.Scan(scan => scan
                .FromAssemblyOf<IQuery<object>>()
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());
            services.AddScoped<IMediator, Mediator>();
            services.AddScoped<ICommandMediator, CommandMediator>();
            services.Decorate<IMediator>((inner, provider) =>
                new MediatorAdapter(
                    inner,
                    provider.GetRequiredService<ICommandMediator>()));
            return services;
        }
    }
}
