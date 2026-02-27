namespace DigitalWallet.Application.Common.Interfaces;
public interface IFileStorageService
{
    /// <summary>
    /// Saves a file to persistent storage.
    /// </summary>
    /// <param name="file">The file to save</param>
    /// <param name="destinationPath">Relative path/filename.</param>
    /// <returns>The full path/URL of the saved file.</returns>
    Task<string> SaveFileAsync(IFormFile file, string destinationPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deleted a file from storage
    /// </summary>
    Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);

}
