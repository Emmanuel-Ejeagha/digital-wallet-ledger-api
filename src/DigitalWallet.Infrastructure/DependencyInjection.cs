namespace DigitalWallet.Infrastructure;
/// <summary>
/// Extension method for registering infrastructure services
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // PostgreSQL  DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IEmailService, EmailService>();
        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services]
        services.AddScoped<IDateTime, DateTimeService>();
        services.AddScoped<IIdempotencyService, IdempotencyService>();

        return services;
    }
}
