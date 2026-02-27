using Microsoft.Extensions.DependencyInjection;

namespace DigitalWallet.Application;
/// <summary>
/// Provides extention methods for reegistering Application layer
/// /// services, handlers, validators, and pipeline behaviors
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

        services.AddAutoMapper(assembly);
        services.AddValidatorsFromAssembly(assembly);
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(IdempotencyBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

            services.AddScoped<TransferDomainService>();

            return services;
    }
}
