namespace DigitalWallet.Infrastructure.Persistence
{
    /// <summary>
    /// Factory for creating ApplicationDbContext at design time.
    /// Reads the connection string from the API project's appsettings.json.
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Determine the base path (when running migrations from the API project, this is the API directory)
            var basePath = Directory.GetCurrentDirectory();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(basePath, "appsettings.json"), optional: false, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}