namespace Yusr.Storage.Abstractions.Options
{
    public class FilesStorageOptions
    {
        public const string SectionName = "FilesStorage";

        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public string ServiceURL { get; set; } = string.Empty;
    }
}
