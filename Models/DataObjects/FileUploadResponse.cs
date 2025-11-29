namespace MauiHybridApp.Models.DataObjects;

public class FileUploadResponse
{
    public FileUploadResponse()
    {
        FileName = string.Empty;
        MimeType = string.Empty;
        FileSize = string.Empty;
        FileType = string.Empty;
        Base64String = string.Empty;
        FileDataArray = Array.Empty<byte>();
    }

    public string FileName { get; set; }
    public string MimeType { get; set; }
    public string FileSize { get; set; }
    public string FileType { get; set; }
    public int RawFileSize { get; set; }
    public byte[] FileDataArray { get; set; }
    public string Base64String { get; set; }
}
