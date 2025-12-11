namespace EatWork.Mobile.Models.DataObjects
{
    public class FileUploadResponse
    {
        public Xamarin.Essentials.FileResult FileResult { get; set; }
        public Plugin.FilePicker.Abstractions.FileData FileData { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public string FileSize { get; set; }
        public string FileType { get; set; }
        public int RawFileSize { get; set; }
        public byte[] FileDataArray { get; set; }
        public string Base64String { get; set; }
    }
}