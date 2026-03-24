# Yusr.Storage

**Yusr.Storage** is a robust, lightweight, and highly efficient S3-compatible file storage abstraction for .NET applications. 

While it is optimized for **Wasabi** and **MinIO** (utilizing `ForcePathStyle`), it works seamlessly with **Amazon S3** and any other S3-compatible storage provider. It provides a clean API for standard file operations and includes a powerful, state-based DTO system (`StorageFile`) designed to make handling file uploads/updates from modern frontend frameworks (React, Angular, Vue) incredibly easy.

---

##  What it Provides

1. **Standard S3 Operations:** Upload, Delete, and retrieve Metadata using standard Streams.
2. **Pre-Signed URLs:** Securely serve private files to users with expiring URLs.
3. **Smart State-Based File Handling:** The `StorageFile` DTO tracks file state (`New`, `Unchanged`, `Delete`). You can pass a Base64 string from your frontend, and the library automatically figures out whether to upload a new file, delete an old one, or do nothing.
4. **Batch Processing:** Easily process lists or dictionaries of files concurrently.
5. **Built-in Dependency Injection:** Ready to be plugged into any modern .NET Core / .NET 5+ application.

---

## Installation

Install the package via NuGet Package Manager Console:

```bash
Install-Package Yusr.Storage
```

Or via the .NET CLI:

```bash
dotnet add package Yusr.Storage
```

---

## Configuration & Setup

### 1. `appsettings.json`
Add your storage credentials to your configuration file:

```json
{
  "FilesStorage": {
    "AccessKey": "YOUR_ACCESS_KEY",
    "SecretKey": "YOUR_SECRET_KEY",
    "BucketName": "your-bucket-name",
    "ServiceURL": "https://s3.eu-central-1.wasabisys.com" // e.g., Wasabi, MinIO, or AWS URL
  }
}
```

### 2. Dependency Injection (`Program.cs` or `Startup.cs`)
Register the configuration and the service in your DI container:

```csharp
using Yusr.Storage.Abstractions.Options;
using Yusr.Storage.Abstractions.Services;
using Yusr.Storage.Providers;

var builder = WebApplication.CreateBuilder(args);

// 1. Bind Options
builder.Services.Configure<FilesStorageOptions>(
    builder.Configuration.GetSection(FilesStorageOptions.SectionName));

// 2. Register the Service
builder.Services.AddScoped<IFilesStorage, WasabiService>();
```

---

## Usage Examples

Inject `IFilesStorage` into your controllers or services to start managing files.

```csharp
public class DocumentService
{
    private readonly IFilesStorage _storage;

    public DocumentService(IFilesStorage storage)
    {
        _storage = storage;
    }
}
```

### 1. Uploading a File (Stream)
Upload a standard file stream directly to your bucket.

```csharp
public async Task UploadProfilePicture(Stream fileStream)
{
    string path = "users/profiles/user-123.jpg";
    string contentType = "image/jpeg";
    
    var result = await _storage.UploadFileAsync(fileStream, path, contentType);

    if (result.Success)
    {
        Console.WriteLine($"File uploaded successfully to: {result.Path}");
    }
    else
    {
        Console.WriteLine($"Upload failed: {result.Error}");
    }
}
```

### 2. Generating Pre-Signed URLs
Generate a secure, temporary URL to allow clients to view or download private files.

```csharp
public string GetDownloadLink(string fileKey)
{
    // Generates a URL valid for 60 minutes (default is 1440 mins / 24 hours)
    string? signedUrl = _storage.GenerateSignedUrl(fileKey, expiresInMinutes: 60);
    
    return signedUrl ?? "File not found";
}
```

### 3. Deleting a File
Remove a file from the bucket using its key/path.

```csharp
public async Task DeleteDocument(string fileKey)
{
    var result = await _storage.DeleteFileAsync(fileKey);
    
    if (result.Success)
        Console.WriteLine("File deleted!");
}
```

---

### 4. The "Smart" Update System (Frontend to Backend)

This library shines when handling updates from a frontend application. Instead of writing complex logic to check if a user uploaded a new file, deleted an existing one, or kept it the same, you use the `StorageFile` DTO.

**Frontend JSON Payload Example:**
```json
{
  "status": 1, // 0 = Unchanged, 1 = New, 2 = Delete
  "base64File": "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkYAAAAAYAAjCB0C8AAAAASUVORK5CYII=",
  "extension": ".png",
  "contentType": "image/png"
}
```

**Backend Processing:**
```csharp
public async Task<string?> UpdateUserProfileImage(StorageFile incomingFile)
{
    // This single method handles everything:
    // - If Status == New: Decodes Base64 and uploads it.
    // - If Status == Delete: Deletes the old file from S3.
    // - If Status == Unchanged: Does nothing and returns the existing path.
    
    string? finalPath = await _storage.HandleUpdatingFile(
        storageFiles: incomingFile, 
        pathPrefix: "users/avatars/"
    );

    return finalPath; // Save this path to your database
}
```

### 5. Batch Processing (Multiple Files)
If a user uploads a gallery of images, you can process them concurrently.

**Processing a List:**
```csharp
public async Task<List<string>> UploadGallery(List<StorageFile> galleryFiles)
{
    // Uploads/Updates/Deletes all files concurrently
    List<string> savedPaths = await _storage.HandleUpdatingFiles(galleryFiles, "galleries/summer-trip/");
    
    return savedPaths;
}
```

**Processing a Dictionary (Useful for mapping files to specific Entity IDs):**
```csharp
public async Task UpdateDocuments(Dictionary<long, StorageFile> documentsWithIds)
{
    // Key: Document ID, Value: StorageFile DTO
    Dictionary<long, string> results = await _storage.HandleUpdatingFilesWithIds(documentsWithIds, "docs/");
    
    foreach(var result in results)
    {
        Console.WriteLine($"Document ID {result.Key} saved to path: {result.Value}");
    }
}
```

### 6. Working with Metadata
You can attach custom metadata to files during upload and retrieve it later.

**Uploading with Metadata:**
```csharp
var metadata = new Dictionary<string, string>
{
    { "UploadedBy", "User-789" },
    { "Department", "HR" }
};

await _storage.UploadFileAsync(stream, "hr/doc1.pdf", "application/pdf", metadata);
```

**Retrieving Metadata:**
```csharp
var metadata = await _storage.GetFileMetadataAsync("hr/doc1.pdf");

if (metadata != null && metadata.ContainsKey("UploadedBy"))
{
    Console.WriteLine($"Uploader: {metadata["UploadedBy"]}");
}
```

### 7. Extracting Keys from URLs
If you have a full S3 URL and need just the database key (path), use this utility:

```csharp
string fullUrl = "https://s3.wasabisys.com/my-bucket/users/profiles/pic.jpg";
string? key = _storage.ExtractKeyFromUrl(fullUrl);

// Result: "users/profiles/pic.jpg"
```

---

## Architecture & Enums Reference

### `StorageFileStatus` Enum
Used by the frontend to tell the backend what to do with the file.
* `0` - **Unchanged**: The file wasn't modified.
* `1` - **New**: A new file is provided in `Base64File`.
* `2` - **Delete**: The existing file should be removed.

### `StorageFile` Class
When sending data to the client, you can easily initialize this class to generate a signed URL automatically:

```csharp
// In your GET endpoint:
var fileDto = new StorageFile("users/profiles/pic.jpg", _storage);
// fileDto.Url is now a secure, pre-signed URL!
// fileDto.Status is automatically set to Unchanged.
```

---

## Contributing
Contributions, issues, and feature requests are welcome! Feel free to check the issues page.

## License
This project is licensed under the MIT License.