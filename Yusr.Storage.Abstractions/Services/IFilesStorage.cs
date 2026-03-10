using Yusr.Storage.Abstractions.Primitives;

namespace Yusr.Storage.Abstractions.Services
{
    public interface IFilesStorage
    {
        Task<StorageFileResult> UploadFileAsync(Stream file, string filePath, string contentType, Dictionary<string, string>? metadata = null);
        Task<StorageFileResult> DeleteFileAsync(string filePath);
        Task<string?> HandleUpdatingFile(StorageFile storageFiles, string? pathPrefix = null, string? fileName = null, Dictionary<string, string>? metadata = null);
        Task<List<string>> HandleUpdatingFiles(List<StorageFile> storageFiles, string? pathPrefix = null);
        Task<Dictionary<long, string>> HandleUpdatingFilesWithIds(Dictionary<long, StorageFile> filesWithIds, string? pathPrefix = null);
        string? GenerateSignedUrl(string? filePath, int expiresInMinutes = 1440);
        string? ExtractKeyFromUrl(string url);
        Task<Dictionary<string, string>?> GetFileMetadataAsync(string filePath);
    }
}
