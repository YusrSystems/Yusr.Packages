using Yusr.Storage.Abstractions.Enums;
using Yusr.Storage.Abstractions.Services;

namespace Yusr.Storage.Abstractions.Primitives
{
    public class StorageFile
    {
        public string? Url { get; set; } = string.Empty;
        public string? Base64File { get; set; }
        public string? Extension { get; set; }
        public string? ContentType { get; set; }
        public StorageFileStatus Status { get; set; }

        public StorageFile()
        {

        }

        public StorageFile(string? filePath, IFilesStorage wasabiService)
        {
            Url = wasabiService.GenerateSignedUrl(filePath);
            Base64File = null;
            Extension = null;
            ContentType = null;
            Status = StorageFileStatus.Unchanged;
        }
    }
}
