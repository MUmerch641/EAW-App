using MauiHybridApp.Services.Data;
using MauiHybridApp.Utils;
using System.Text;

namespace MauiHybridApp.Services;

public interface IFileUploadService
{
    Task<FileUploadResult> UploadProfilePhotoAsync(Stream fileStream, string fileName);
    Task<FileUploadResult> UploadDocumentAsync(Stream fileStream, string fileName, string documentType);
    Task<FileUploadResult> UploadFileAsync(Stream fileStream, string fileName, string endpoint);
    Task<bool> DeleteFileAsync(string fileId);
    Task<Stream?> DownloadFileAsync(string fileId);
    Task<List<string>> GetSupportedImageFormatsAsync();
    Task<List<string>> GetSupportedDocumentFormatsAsync();
    Task<long> GetMaxFileSizeAsync();
}

public class FileUploadService : IFileUploadService
{
    private readonly IGenericRepository _repository;
    private readonly List<string> _supportedImageFormats = new() { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
    private readonly List<string> _supportedDocumentFormats = new() { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt" };
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB

    public FileUploadService(IGenericRepository repository)
    {
        _repository = repository;
    }

    public async Task<FileUploadResult> UploadProfilePhotoAsync(Stream fileStream, string fileName)
    {
        try
        {
            // Validate file
            var validation = ValidateImageFile(fileStream, fileName);
            if (!validation.IsValid)
            {
                return new FileUploadResult
                {
                    Success = false,
                    ErrorMessage = validation.ErrorMessage
                };
            }

            return await UploadFileAsync(fileStream, fileName, ApiEndpoints.UploadProfilePhoto);
        }
        catch (Exception ex)
        {
            return new FileUploadResult
            {
                Success = false,
                ErrorMessage = $"Error uploading profile photo: {ex.Message}"
            };
        }
    }

    public async Task<FileUploadResult> UploadDocumentAsync(Stream fileStream, string fileName, string documentType)
    {
        try
        {
            // Validate file
            var validation = ValidateDocumentFile(fileStream, fileName);
            if (!validation.IsValid)
            {
                return new FileUploadResult
                {
                    Success = false,
                    ErrorMessage = validation.ErrorMessage
                };
            }

            var endpoint = string.Format(ApiEndpoints.UploadDocument, documentType);
            return await UploadFileAsync(fileStream, fileName, endpoint);
        }
        catch (Exception ex)
        {
            return new FileUploadResult
            {
                Success = false,
                ErrorMessage = $"Error uploading document: {ex.Message}"
            };
        }
    }

    public async Task<FileUploadResult> UploadFileAsync(Stream fileStream, string fileName, string endpoint)
    {
        try
        {
            // Convert stream to byte array
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            // Create multipart form data
            using var content = new MultipartFormDataContent();
            using var fileContent = new ByteArrayContent(fileBytes);
            
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(GetContentType(fileName));
            content.Add(fileContent, "file", fileName);

            // Upload file
            var response = await _repository.PostMultipartAsync<FileUploadApiResponse>(endpoint, content);

            if (response?.Success == true)
            {
                return new FileUploadResult
                {
                    Success = true,
                    FileId = response.FileId,
                    FileUrl = response.FileUrl,
                    FileName = response.FileName
                };
            }

            return new FileUploadResult
            {
                Success = false,
                ErrorMessage = response?.ErrorMessage ?? "File upload failed"
            };
        }
        catch (Exception ex)
        {
            return new FileUploadResult
            {
                Success = false,
                ErrorMessage = $"Network error during file upload: {ex.Message}"
            };
        }
    }

    public async Task<bool> DeleteFileAsync(string fileId)
    {
        try
        {
            var endpoint = string.Format(ApiEndpoints.DeleteFile, fileId);
            var response = await _repository.DeleteAsync<DeleteFileResponse>(endpoint);
            return response?.Success ?? false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting file: {ex.Message}");
            return false;
        }
    }

    public async Task<Stream?> DownloadFileAsync(string fileId)
    {
        try
        {
            var endpoint = string.Format(ApiEndpoints.DownloadFile, fileId);
            return await _repository.GetStreamAsync(endpoint);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error downloading file: {ex.Message}");
            return null;
        }
    }

    public async Task<List<string>> GetSupportedImageFormatsAsync()
    {
        await Task.CompletedTask;
        return _supportedImageFormats;
    }

    public async Task<List<string>> GetSupportedDocumentFormatsAsync()
    {
        await Task.CompletedTask;
        return _supportedDocumentFormats;
    }

    public async Task<long> GetMaxFileSizeAsync()
    {
        try
        {
            var response = await _repository.GetAsync<MaxFileSizeResponse>(ApiEndpoints.GetMaxFileSize);
            return response?.MaxSize ?? MaxFileSize;
        }
        catch
        {
            return MaxFileSize;
        }
    }

    private FileValidationResult ValidateImageFile(Stream fileStream, string fileName)
    {
        // Check file extension
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!_supportedImageFormats.Contains(extension))
        {
            return new FileValidationResult
            {
                IsValid = false,
                ErrorMessage = $"Unsupported image format. Supported formats: {string.Join(", ", _supportedImageFormats)}"
            };
        }

        // Check file size
        if (fileStream.Length > MaxFileSize)
        {
            return new FileValidationResult
            {
                IsValid = false,
                ErrorMessage = $"File size exceeds maximum limit of {MaxFileSize / (1024 * 1024)}MB"
            };
        }

        return new FileValidationResult { IsValid = true };
    }

    private FileValidationResult ValidateDocumentFile(Stream fileStream, string fileName)
    {
        // Check file extension
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!_supportedDocumentFormats.Contains(extension))
        {
            return new FileValidationResult
            {
                IsValid = false,
                ErrorMessage = $"Unsupported document format. Supported formats: {string.Join(", ", _supportedDocumentFormats)}"
            };
        }

        // Check file size
        if (fileStream.Length > MaxFileSize)
        {
            return new FileValidationResult
            {
                IsValid = false,
                ErrorMessage = $"File size exceeds maximum limit of {MaxFileSize / (1024 * 1024)}MB"
            };
        }

        return new FileValidationResult { IsValid = true };
    }

    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".txt" => "text/plain",
            _ => "application/octet-stream"
        };
    }

    // Response models
    private class FileUploadApiResponse
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string? FileId { get; set; }
        public string? FileUrl { get; set; }
        public string? FileName { get; set; }
    }

    private class DeleteFileResponse
    {
        public bool Success { get; set; }
    }

    private class MaxFileSizeResponse
    {
        public long MaxSize { get; set; }
    }

    private class FileValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

public class FileUploadResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? FileId { get; set; }
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
}
