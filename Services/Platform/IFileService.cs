namespace MauiHybridApp.Services.Platform;

public interface IFileService
{
    Task<string> SaveFileAsync(string filename, byte[] data);
    Task<byte[]> ReadFileAsync(string filename);
    Task<bool> DeleteFileAsync(string filename);
    Task<bool> FileExistsAsync(string filename);
    string GetLocalFilePath(string filename);
}

public class FileService : IFileService
{
    public async Task<string> SaveFileAsync(string filename, byte[] data)
    {
        var filePath = GetLocalFilePath(filename);
        await File.WriteAllBytesAsync(filePath, data);
        return filePath;
    }

    public async Task<byte[]> ReadFileAsync(string filename)
    {
        var filePath = GetLocalFilePath(filename);
        if (File.Exists(filePath))
        {
            return await File.ReadAllBytesAsync(filePath);
        }
        return Array.Empty<byte>();
    }

    public Task<bool> DeleteFileAsync(string filename)
    {
        var filePath = GetLocalFilePath(filename);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> FileExistsAsync(string filename)
    {
        var filePath = GetLocalFilePath(filename);
        return Task.FromResult(File.Exists(filePath));
    }

    public string GetLocalFilePath(string filename)
    {
        return Path.Combine(FileSystem.AppDataDirectory, filename);
    }
}

