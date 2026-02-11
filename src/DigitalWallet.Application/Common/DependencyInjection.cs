using Microsoft.Extensions.DependencyInjection;
using MediatR;
using FluentValidation;
using AutoMapper;
using DigitalWallet.Application.Common.Behaviors;
using DigitalWallet.Application.Common.Mappings;
using DigitalWallet.Application.Wallets.Services;

namespace DigitalWallet.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register AutoMapper
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // Register FluentValidation validators
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

            // Register MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(IdempotencyBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
            });

            // Register application services
            services.AddScoped<IFinancialTransactionService, FinancialTransactionService>();

            return services;
        }
    }
}