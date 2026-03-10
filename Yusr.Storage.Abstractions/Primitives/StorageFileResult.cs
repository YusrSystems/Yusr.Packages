namespace Yusr.Storage.Abstractions.Primitives
{
    public class StorageFileResult
    {
        public bool Success { get; set; } = false;
        public string Error { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
    }
}