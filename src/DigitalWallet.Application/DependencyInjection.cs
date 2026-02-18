using System.Reflection;
using DigitalWallet.Application.Common.Behaviors;
using DigitalWallet.Domain.TransferDomainServices;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalWallet.Application;
/// <summary>
/// Provides extention methods for reegistering Application layer
/// services, handlers, validators, and pipeline behaviors
/// into the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers Application layer components including:
    /// MediatR handlers, pipeline behaviors, AutoMapper profiles,
    /// FluentValidation validators, and domain services.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // AutoMapper profiles
        services.AddAutoMapper(assembly);

        // FluentValidation validators
        services.AddValidatorsFromAssembly(assembly);

        // MediatR + pipeline behaviors
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            // Order matters (outer → inner)
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(IdempotencyBehavior<,>));
        cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

            // Domain services
            services.AddScoped<TransferDomainService>();

            return services;
    }
}
