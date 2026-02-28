using Microsoft.AspNetCore.Http;

namespace DigitalWallet.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly FileStorageSettings _settings;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(
        IOptions<FileStorageSettings> settings,
        ILogger<LocalFileStorageService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<string> SaveFileAsync(IFormFile file, string destinationPath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_settings.BasePath, destinationPath);
        var directory = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory!);

        using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);

        _logger.LogInformation("File saved to {Path}", fullPath);
        return fullPath;
    }

    public Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            _logger.LogInformation("File deleted: {Path}", filePath);
        }
        return Task.CompletedTask;
    }
}

public class FileStorageSettings
{
    public string BasePath { get; set; } = "uploads";
}
