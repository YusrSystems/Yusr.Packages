using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Yusr.Storage.Abstractions.Enums;
using Yusr.Storage.Abstractions.Options;
using Yusr.Storage.Abstractions.Primitives;
using Yusr.Storage.Abstractions.Services;

namespace Yusr.Storage.Providers
{
    public class S3Service : IFilesStorage
    {
        private readonly FilesStorageOptions _options;
        private readonly AmazonS3Client _s3Client;

        public S3Service(IOptions<FilesStorageOptions> options)
        {
            _options = options.Value;

            var s3Config = new AmazonS3Config
            {
                ServiceURL = _options.ServiceURL,
                ForcePathStyle = true // Required for Wasabi/MinIO
            };

            _s3Client = new AmazonS3Client(_options.AccessKey, _options.SecretKey, s3Config);
        }


        public async Task<StorageFileResult> UploadFileAsync(Stream file, string filePath, string contentType, Dictionary<string, string>? metadata = null)
        {
            StorageFileResult res = new();

            if (file == null || file.Length == 0)
            {
                res.Success = false;
                res.Error = "الملف المرسل فارغ";
                return res;
            }

            if (file.CanSeek)
                file.Position = 0;

            var request = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = filePath,
                InputStream = file,
                ContentType = contentType,
                CannedACL = S3CannedACL.Private,
                DisablePayloadSigning = true
            };

            if (metadata != null)
            {
                foreach (var item in metadata)
                {
                    request.Metadata.Add(item.Key, item.Value);
                }
            }

            try
            {
                await _s3Client.PutObjectAsync(request);
                res.Path = filePath;
                res.Success = true;
                return res;
            }
            catch (AmazonS3Exception ex)
            {
                res.Success = false;
                res.Error = "[حدث خطأ أثناء محاولة إضافة الملف]:" + ex.Message;
                return res;
            }
        }

        public async Task<StorageFileResult> DeleteFileAsync(string filePath)
        {
            StorageFileResult res = new();

            if (string.IsNullOrEmpty(filePath))
            {
                res.Success = false;
                res.Error = "لم يتم تحديد المسار لحذف الملف";
                return res;
            }

            try
            {
                var request = new DeleteObjectRequest
                {
                    BucketName = _options.BucketName,
                    Key = filePath
                };

                await _s3Client.DeleteObjectAsync(request);
                res.Success = true;
                return res;
            }
            catch (AmazonS3Exception ex)
            {
                res.Success = false;
                res.Error = "[حدث خطأ أثناء محاولة حذف الملف]:" + ex.Message;
                return res;
            }
        }

        public async Task<List<string>> HandleUpdatingFiles(List<StorageFile> filesDTO, string? pathPrefix = null)
        {
            var tasks = filesDTO.Select(file => HandleUpdatingFile(file, pathPrefix));

            var results = await Task.WhenAll(tasks);

            return results.Where(path => path != null).ToList()!;
        }

        public async Task<Dictionary<long, string>> HandleUpdatingFilesWithIds(Dictionary<long, StorageFile> filesWithIds, string? pathPrefix = null)
        {
            var tasks = filesWithIds.Select(async kvp => new
            {
                Id = kvp.Key,
                Path = await HandleUpdatingFile(kvp.Value, pathPrefix)
            });

            var results = await Task.WhenAll(tasks);

            return results
                .Where(x => x.Path != null)
                .ToDictionary(x => x.Id, x => x.Path!);
        }

        public string? GenerateSignedUrl(string? filePath, int expiresInMinutes = 1440)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _options.BucketName,
                Key = filePath,
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes)
            };

            return _s3Client.GetPreSignedURL(request);
        }

        public string? ExtractKeyFromUrl(string url)
        {
            var uri = new Uri(url);

            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            int bucketIndex = Array.FindIndex(segments,
                s => s.Equals(_options.BucketName, StringComparison.OrdinalIgnoreCase));

            if (bucketIndex == -1 || bucketIndex == segments.Length - 1)
                return null;

            return string.Join("/", segments[(bucketIndex + 1)..]);
        }

        public async Task<string?> HandleUpdatingFile(StorageFile fileDTO, string? pathPrefix, string? fileName = null, Dictionary<string, string>? metadata = null)
        {
            string? imagePath = (!string.IsNullOrWhiteSpace(fileDTO.Url) && fileDTO.Status != StorageFileStatus.New)
                ? ExtractKeyFromUrl(fileDTO.Url)
                : null;

            if (fileDTO.Status == StorageFileStatus.New && string.IsNullOrEmpty(fileDTO.Base64File))
            {
                return string.Empty;
            }

            StorageFileResult res = new();

            switch (fileDTO.Status)
            {
                case StorageFileStatus.New when !string.IsNullOrEmpty(fileDTO.Base64File):
                    try
                    {
                        byte[] fileBytes = Convert.FromBase64String(fileDTO.Base64File);
                        using var stream = new MemoryStream(fileBytes);

                        string extension = fileDTO.Extension ?? ".bin";
                        if (!extension.StartsWith('.')) extension = "." + extension;

                        string contentType = fileDTO.ContentType ?? "application/octet-stream";
                        string finalPath = imagePath ?? $"{pathPrefix}{fileName ?? Guid.NewGuid().ToString()}{extension}";

                        res = await UploadFileAsync(stream, finalPath, contentType, metadata);
                    }
                    catch { return null; }
                    break;

                case StorageFileStatus.Delete when imagePath != null:
                    res = await DeleteFileAsync(imagePath);
                    res.Path = string.Empty;
                    break;

                case StorageFileStatus.Unchanged:
                    res = new StorageFileResult { Path = imagePath ?? string.Empty, Success = true };
                    break;

                default:
                    // If status is unknown or "None", keep the current path
                    return imagePath ?? string.Empty;
            }

            return res.Success ? res.Path : null;
        }

        public async Task<Dictionary<string, string>?> GetFileMetadataAsync(string filePath)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = _options.BucketName,
                    Key = filePath
                };

                var response = await _s3Client.GetObjectMetadataAsync(request);
                var dict = new Dictionary<string, string>();

                foreach (string key in response.Metadata.Keys)
                {
                    dict.Add(key, response.Metadata[key]);
                }
                return dict;
            }
            catch { return null; }
        }
    }
}